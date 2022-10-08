//
//  TimestampStyle.cs
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

namespace Remora.Discord.Extensions.Formatting;

/// <summary>
/// Represents the possible display formats for Timestamp Markdown.
/// </summary>
public enum TimestampStyle
{
    /// <summary>
    /// 16:20.
    /// </summary>
    ShortTime,

    /// <summary>
    /// 16:20:30.
    /// </summary>
    LongTime,

    /// <summary>
    /// 20/04/2021.
    /// </summary>
    ShortDate,

    /// <summary>
    /// 20 April 2021.
    /// </summary>
    LongDate,

    /// <summary>
    /// 20 April 2021 16:20.
    /// </summary>
    ShortDateTime,

    /// <summary>
    /// Tuesday, 20 April 2021 16:20.
    /// </summary>
    LongDateTime,

    /// <summary>
    /// 2 months ago.
    /// </summary>
    RelativeTime
}
