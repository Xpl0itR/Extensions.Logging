// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Extensions.Logging.Webhook.Discord;

/// <summary>
///     A provider of <see cref="DiscordLogger" /> instances.
/// </summary>
[ProviderAlias("Discord")]
public class DiscordLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, DiscordLogger>     _loggers;
    private readonly BlockingCollection<ValueTuple<string, string?>> _logQueue;
    private readonly HttpClient                                      _httpClient;
    private readonly IDisposable                                     _optionsMonitor;
    private readonly Thread                                          _processLogQueueThread;

    private Uri _webhookUrl;

    /// <summary>
    ///     Creates an instance of <see cref="DiscordLoggerProvider" />.
    /// </summary>
#if !NET5_0_OR_GREATER
#pragma warning disable CS8618
#endif
    public DiscordLoggerProvider(IOptionsMonitor<DiscordLoggerOptions> optionsMonitor)
    {
        _loggers        = new ConcurrentDictionary<string, DiscordLogger>();
        _logQueue       = new BlockingCollection<ValueTuple<string, string?>>();
        _httpClient     = new HttpClient();
        _optionsMonitor = optionsMonitor.OnChange(SetWebhookUrl);

        SetWebhookUrl(optionsMonitor.CurrentValue);

        _processLogQueueThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name         = "Discord logger queue processing thread"
        };
        _processLogQueueThread.Start();
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        return _loggers.GetOrAdd(category, CreateDiscordLogger);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _logQueue.CompleteAdding();
        _optionsMonitor.Dispose();
        _loggers.Clear();
        _processLogQueueThread.Join();
        _logQueue.Dispose();
    }

#if NET5_0_OR_GREATER
    [MemberNotNull(nameof(_webhookUrl))]
#endif
    private void SetWebhookUrl(DiscordLoggerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.WebhookUrl))
            throw new Exception($"The configuration option, Logging:Discord:{nameof(options.WebhookUrl)}, must be set.");

        _webhookUrl = new Uri(options.WebhookUrl!, UriKind.Absolute);
    }

    private DiscordLogger CreateDiscordLogger(string category) =>
        new(category, _logQueue);

    private void ProcessLogQueue()
    {
        string remaining = string.Empty;
        string resetTime = string.Empty;
        int    secToWait;

        foreach ((string message, string? exception) in _logQueue.GetConsumingEnumerable())
        {
            using MultipartFormDataContent requestContent = new() { { new StringContent(message), "content" } };

            if (exception != null)
                requestContent.Add(new StringContent(exception), "file[0]", "exception.txt");

            using HttpRequestMessage request = new(HttpMethod.Post, _webhookUrl) { Content = requestContent };

            if (remaining == "0" && (secToWait = (int)(long.Parse(resetTime) - DateTimeOffset.UtcNow.ToUnixTimeSeconds())) > 0)
                Thread.Sleep(secToWait * 1000);

            using HttpResponseMessage response = SendSync(_httpClient, request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            response.EnsureSuccessStatusCode();

            remaining = response.Headers.GetValues("X-RateLimit-Remaining").Single();
            resetTime = response.Headers.GetValues("X-RateLimit-Reset").Single();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HttpResponseMessage SendSync(HttpClient client, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken) =>
#if NET5_0_OR_GREATER
        client.Send(request, completionOption, cancellationToken);
#else
        client.SendAsync(request, completionOption, cancellationToken).GetAwaiter().GetResult();
#endif
}