using System;
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