// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Extensions.Logging.Console;

namespace Extensions.Logging.Console.BetterFormatter;

/// <summary>
///     Options for the <see cref="BetterConsoleFormatter" />.
/// </summary>
public class BetterConsoleFormatterOptions : ConsoleFormatterOptions
{
    /// <summary>
    ///     Writes an additional newline string after every log message when <see langword="true" />.
    /// </summary>
    public bool WriteAdditionalNewline { get; set; }
}