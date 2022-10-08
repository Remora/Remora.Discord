//
//  DateTimeOffsetExtensions.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Rest.Extensions;

/// <summary>
/// Defines extensions to the <see cref="DateTimeOffset"/> struct.
/// </summary>
[PublicAPI]
public static class DateTimeOffsetExtensions
{
    /// <summary>
    /// Converts <see cref="DateTimeOffset"/> to ISO8601 string representation.
    /// </summary>
    /// <param name="dateTimeOffset">The date time offset.</param>
    /// <returns>The ISO8601 string representation of date time offset.</returns>
    public static string ToISO8601String(this DateTimeOffset dateTimeOffset)
    {
        var offset = dateTimeOffset.Offset;
        return dateTimeOffset.ToString($"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'+'{offset.Hours:D2}':'{offset.Minutes:D2}");
    }
}
