//
//  Markdown.Timestamps.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;

namespace Remora.Discord.Extensions.Formatting;

public partial class Markdown
{
    /// <summary>
    /// Formats a Unix timestamp value into Discord Markdown Timestamp.
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp to format.</param>
    /// <param name="timestampStyle">The style to format into.</param>
    /// <returns>
    /// A Discord markdown-formatted Timestamp string.
    /// </returns>
    public static string Timestamp(long unixTimestamp, TimestampStyle? timestampStyle = default)
    {
        return timestampStyle.HasValue
            ? $"<t:{unixTimestamp}:{TimestampStyleToCode(timestampStyle)}>"
            : $"<t:{unixTimestamp}>";
    }

    /// <summary>
    /// Formats a Unix timestamp value into Discord Markdown Timestamp.
    /// </summary>
    /// <param name="dateTimeOffset">The time to format.</param>
    /// <param name="timestampStyle">The style to format into.</param>
    /// <returns>
    /// A Discord markdown-formatted Timestamp string.
    /// </returns>
    public static string Timestamp(DateTimeOffset dateTimeOffset, TimestampStyle? timestampStyle = default)
        => Timestamp(dateTimeOffset.ToUnixTimeSeconds(), timestampStyle);

    /// <summary>
    /// Formats a Unix timestamp value into Discord Markdown Timestamp.
    /// </summary>
    /// <param name="dateTime">The time to format.</param>
    /// <param name="timestampStyle">The style to format into.</param>
    /// <returns>
    /// A Discord markdown-formatted Timestamp string.
    /// </returns>
    public static string Timestamp(DateTime dateTime, TimestampStyle? timestampStyle = default)
        => Timestamp(((DateTimeOffset)dateTime).ToUnixTimeSeconds(), timestampStyle);

    /// <summary>
    /// Converts a <see cref="TimestampStyle"/> to its Discord Markdown code.
    /// </summary>
    /// <param name="timestampStyle">The style to convert.</param>
    /// <returns>
    /// The Discord Markdown code for the given style.
    /// </returns>
    private static char TimestampStyleToCode(TimestampStyle? timestampStyle)
    {
        return timestampStyle switch
        {
            TimestampStyle.ShortTime => 't',
            TimestampStyle.LongTime => 'T',
            TimestampStyle.ShortDate => 'd',
            TimestampStyle.LongDate => 'D',
            TimestampStyle.ShortDateTime => 'f',
            TimestampStyle.LongDateTime => 'F',
            TimestampStyle.RelativeTime => 'R',
            null => TimestampStyleToCode(TimestampStyle.ShortDateTime),
            _ => throw new ArgumentOutOfRangeException(nameof(timestampStyle), timestampStyle, "The specified timestamp style was invalid.")
        };
    }
}
