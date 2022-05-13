// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.Console.BetterFormatter;

/// <summary>
///     A set of methods to extend classes which implement the <see cref="ILoggingBuilder" /> interface.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    ///     Add a console log formatter named 'better' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode")]
#endif
    public static ILoggingBuilder AddBetterConsoleFormatter(this ILoggingBuilder builder) =>
        builder.AddConsoleFormatter<BetterConsoleFormatter, BetterConsoleFormatterOptions>();

    /// <summary>
    ///     Adds a console logger named 'Console' and a console log formatter named 'better' to the factory with default
    ///     properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
    public static ILoggingBuilder AddBetterConsole(this ILoggingBuilder builder) =>
        builder.AddBetterConsoleFormatter()
               .AddConsole(options => options.FormatterName = BetterConsoleFormatter.Name);

    /// <summary>
    ///     Adds a console logger named 'Console' and a console log formatter named 'better' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
    /// <param name="configure">
    ///     A delegate to configure the <see cref="BetterConsoleFormatterOptions" /> for the <see cref="BetterConsoleFormatter" />.
    /// </param>
    public static ILoggingBuilder AddBetterConsole(this ILoggingBuilder builder, Action<BetterConsoleFormatterOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        builder.AddBetterConsole();
        builder.Services.Configure(configure);

        return builder;
    }
}