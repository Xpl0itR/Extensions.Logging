// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Extensions.Logging.Console.BetterFormatter;

internal sealed class BetterConsoleFormatter : ConsoleFormatter, IDisposable
{
    internal new const string Name = "better";

    private readonly string       _paddedNewLine = Environment.NewLine + "   ";
    private readonly IDisposable? _optionsMonitor;

    private ConsoleFormatterOptions _options;

#if !NET5_0_OR_GREATER
#pragma warning disable CS8618
#endif
    public BetterConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options) : base(Name)
    {
        ReloadOptions(options.CurrentValue);
        _optionsMonitor = options.OnChange(ReloadOptions);
    }

    /// <inheritdoc />
    public void Dispose() =>
        _optionsMonitor?.Dispose();

    /// <inheritdoc />
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (string.IsNullOrEmpty(message) && logEntry.Exception == null)
            return;

        WriteConsoleColour(textWriter, logEntry.LogLevel);

        // Timestamp
        string? timestampFormat = (string?)_options.TimestampFormat;
        if (timestampFormat != null)
        {
            string timeStamp = (_options.UseUtcTimestamp
                    ? DateTimeOffset.UtcNow
                    : DateTimeOffset.Now)
               .ToString(timestampFormat);

            textWriter.Write(timeStamp);
            textWriter.Write(" | ");
        }

        // Log level
        textWriter.Write(logEntry.LogLevel switch
        {
            LogLevel.Trace       => "TRCE | ",
            LogLevel.Debug       => "DBUG | ",
            LogLevel.Information => "INFO | ",
            LogLevel.Warning     => "WARN | ",
            LogLevel.Error       => "FAIL | ",
            LogLevel.Critical    => "CRIT | ",
            _                    => "???? | "
        });

        // Category
        textWriter.Write(logEntry.Category);

        // Event
        if (logEntry.EventId.Id != 0 || logEntry.EventId.Name != null)
        {
            textWriter.Write(" | ");
            textWriter.Write(logEntry.EventId);
        }

        // Scope
        if (_options.IncludeScopes && scopeProvider != null)
        {
            scopeProvider.ForEachScope((scope, writer) =>
            {
                writer.Write(_paddedNewLine);
                writer.Write("=> ");
                writer.Write(scope);
            },
            textWriter);
        }

        // Message
        textWriter.Write(_paddedNewLine);
        textWriter.Write(ReplaceLineEndings(message!, _paddedNewLine));

        // Exception
        if (logEntry.Exception != null)
        {
            textWriter.Write(_paddedNewLine);
            textWriter.Write(ReplaceLineEndings(logEntry.Exception.ToString(), _paddedNewLine));
        }

        // End
        WriteConsoleColour(textWriter, LogLevel.None);
        textWriter.Write(Environment.NewLine);
    }

#if NET5_0_OR_GREATER
    [MemberNotNull(nameof(_options))]
#endif
    private void ReloadOptions(ConsoleFormatterOptions options) =>
        _options = options;

    private static void WriteConsoleColour(TextWriter textWriter, LogLevel logLevel) =>
        textWriter.Write(logLevel switch
        {
            LogLevel.Critical    => "\x1B[1m\x1B[37m\x1B[41m",
            LogLevel.Error       => "\x1B[1m\x1B[31m",
            LogLevel.Warning     => "\x1B[1m\x1B[33m",
            LogLevel.Information => "\x1B[1m\x1B[36m",
            LogLevel.Debug       => "\x1B[1m\x1B[37m",
            LogLevel.Trace       => "\x1B[37m",
            _                    => "\x1B[22m\x1B[39m\x1B[49m"
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ReplaceLineEndings(string inputText, string replacementText) =>
#if NET6_0_OR_GREATER
        inputText.ReplaceLineEndings(replacementText);
#else
        inputText.Replace(Environment.NewLine, replacementText);
#endif
}