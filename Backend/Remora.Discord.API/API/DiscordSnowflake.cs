//
//  DiscordSnowflake.cs
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

using System.Diagnostics.CodeAnalysis;
using Remora.Rest.Core;

namespace Remora.Discord.API;

/// <summary>
/// Contains methods for initializing a <see cref="Snowflake"/> with the <see cref="Constants.DiscordEpoch"/>.
/// </summary>
public static class DiscordSnowflake
{
    /// <summary>
    /// Initializes a new instance of a Snowflake with the Discord epoch.
    /// </summary>
    /// <param name="value">The snowflake value.</param>
    /// <returns>A snowflake.</returns>
    public static Snowflake New(ulong value)
        => new(value, Constants.DiscordEpoch);

    /// <inheritdoc cref="Snowflake.TryParse(string, out Snowflake?, ulong)"/>
    public static bool TryParse(string value, [NotNullWhen(true)] out Snowflake? result)
        => Snowflake.TryParse(value, out result, Constants.DiscordEpoch);
}
