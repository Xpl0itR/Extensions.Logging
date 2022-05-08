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

        sb.Append(logLevel switch
        {
            LogLevel.Trace       => "🔍 TRCE | ",
            LogLevel.Debug       => "🕸️ DBUG | ",
            LogLevel.Information => "ℹ️ INFO | ",
            LogLevel.Warning     => "⚠️ WARN | ",
            LogLevel.Error       => "❌ FAIL | ",
            LogLevel.Critical    => "☢️ CRIT | ",
            _                    => "???? | "
        });

        sb.Append(_category);

        if (eventId.Id != 0 || eventId.Name != null)
        {
            sb.Append(" | ");
            sb.Append(eventId);
        }

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

    /// <summary>
    ///     This method does nothing.
    /// </summary>
    public IDisposable BeginScope<TState>(TState state) =>
        NullScope.Instance;

    private class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        void IDisposable.Dispose() { }
    }
}