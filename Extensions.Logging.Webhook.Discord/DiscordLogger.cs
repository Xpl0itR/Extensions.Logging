// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.Webhook.Discord;

internal sealed class DiscordLogger : ILogger
{
    private readonly string                                          _category;
    private readonly BlockingCollection<ValueTuple<string, string?>> _logQueue;

    internal DiscordLogger(string category, BlockingCollection<ValueTuple<string, string?>> logQueue) =>
        (_category, _logQueue) = (category, logQueue);

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        StringBuilder sb = new();

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        sb.Append(logLevel switch
        {
            LogLevel.Trace       => "🔍 trce",
            LogLevel.Debug       => "🕸️ dbug",
            LogLevel.Information => "ℹ️ info",
            LogLevel.Warning     => "⚠️ warn",
            LogLevel.Error       => "❌ fail",
            LogLevel.Critical    => "☢️ crit",
            _                    => throw new ArgumentOutOfRangeException(nameof(logLevel))
        });
        sb.Append(": ");
        sb.Append(_category);
        sb.Append('[');
        sb.Append(eventId.Id);
        sb.Append(']');
        sb.AppendLine();
        sb.Append("       ");
        sb.Append(formatter(state, exception));

        string  logMessage = sb.ToString();
        string? exMessage  = exception?.ToString();

        if (!_logQueue.IsAddingCompleted)
        {
            try
            {
                _logQueue.Add((logMessage, exMessage));
            }
            catch { /* ignored */ }
        }
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) =>
        logLevel != LogLevel.None;

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) =>
        throw new NotImplementedException();
}