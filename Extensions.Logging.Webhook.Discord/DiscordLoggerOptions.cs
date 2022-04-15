// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

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