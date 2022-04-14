namespace Extensions.Logging.Webhook.Discord;

/// <summary>
///     Options for a <see cref="DiscordLogger" />.
/// </summary>
public class DiscordLoggerOptions
{
    /// <summary>
    ///     The URL of the Discord webhook where the logs will be POSTed to.
    /// </summary>
    public string? WebhookUrl { get; set; }
}