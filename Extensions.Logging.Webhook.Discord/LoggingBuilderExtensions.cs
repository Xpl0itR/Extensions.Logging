// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Extensions.Logging.Webhook.Discord;

/// <summary>
///     A set of methods to extend classes which implement the <see cref="ILoggingBuilder" /> interface.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    ///     Adds a Discord webhook logger named 'Discord' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode")]
#endif
    public static ILoggingBuilder AddDiscord(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(ILoggerProvider), typeof(DiscordLoggerProvider), ServiceLifetime.Singleton));
        LoggerProviderOptions.RegisterProviderOptions<DiscordLoggerOptions, DiscordLoggerProvider>(builder.Services);

        return builder;
    }

    /// <summary>
    ///     Adds a Discord webhook logger named 'Discord' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="DiscordLogger" />.</param>
    public static ILoggingBuilder AddDiscord(this ILoggingBuilder builder, Action<DiscordLoggerOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        builder.AddDiscord();
        builder.Services.Configure(configure);

        return builder;
    }
}