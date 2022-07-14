// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.LogMethods;

/// <summary>
///     ILogger extension methods for logging a string without unnecessary allocations.
/// </summary>
public static class LogMethods
{
    private static readonly EventId                          NullEventId    = new(0);
    private static readonly Func<string, Exception?, string> MessageAdapter = (message, _) => message;

    /// <summary>
    ///     Writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void Log(this ILogger logger, LogLevel logLevel, string message, EventId eventId, Exception? exception = null) =>
        logger.Log(logLevel, eventId, message, exception, MessageAdapter);

    /// <summary>
    ///     Writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void Log(this ILogger logger, LogLevel logLevel, string message, Exception? exception = null) =>
        logger.Log(logLevel, NullEventId, message, exception, MessageAdapter);

    //------------------------------------------CRITICAL------------------------------------------//

    /// <summary>
    ///     Writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogCritical(this ILogger logger, string message, EventId eventId, Exception? exception = null) =>
        logger.Log(LogLevel.Critical, eventId, message, exception, MessageAdapter);

    /// <summary>
    ///     Writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogCritical(this ILogger logger, string message, Exception? exception = null) =>
        logger.Log(LogLevel.Critical, NullEventId, message, exception, MessageAdapter);

    //------------------------------------------ERROR------------------------------------------//

    /// <summary>
    ///     Writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogError(this ILogger logger, string message, EventId eventId, Exception? exception = null) =>
        logger.Log(LogLevel.Error, eventId, message, exception, MessageAdapter);

    /// <summary>
    ///     Writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogError(this ILogger logger, string message, Exception? exception = null) =>
        logger.Log(LogLevel.Error, NullEventId, message, exception, MessageAdapter);

    //------------------------------------------WARNING------------------------------------------//

    /// <summary>
    ///     Writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogWarning(this ILogger logger, string message, EventId eventId, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Warning))
            logger.Log(LogLevel.Warning, eventId, message, exception, MessageAdapter);
    }

    /// <summary>
    ///     Writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogWarning(this ILogger logger, string message, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Warning))
            logger.Log(LogLevel.Warning, NullEventId, message, exception, MessageAdapter);
    }

    //------------------------------------------INFORMATION------------------------------------------//

    /// <summary>
    ///     Writes an information log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogInformation(this ILogger logger, string message, EventId eventId, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.Log(LogLevel.Information, eventId, message, exception, MessageAdapter);
    }

    /// <summary>
    ///     Writes an information log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogInformation(this ILogger logger, string message, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.Log(LogLevel.Information, NullEventId, message, exception, MessageAdapter);
    }

    //------------------------------------------DEBUG------------------------------------------//

    /// <summary>
    ///     Writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogDebug(this ILogger logger, string message, EventId eventId, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.Log(LogLevel.Debug, eventId, message, exception, MessageAdapter);
    }

    /// <summary>
    ///     Writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogDebug(this ILogger logger, string message, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.Log(LogLevel.Debug, NullEventId, message, exception, MessageAdapter);
    }

    //------------------------------------------TRACE------------------------------------------//

    /// <summary>
    ///     Writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogTrace(this ILogger logger, string message, EventId eventId, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Trace))
            logger.Log(LogLevel.Trace, eventId, message, exception, MessageAdapter);
    }

    /// <summary>
    ///     Writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static void LogTrace(this ILogger logger, string message, Exception? exception = null)
    {
        if (logger.IsEnabled(LogLevel.Trace))
            logger.Log(LogLevel.Trace, NullEventId, message, exception, MessageAdapter);
    }
}