//
//  GuildNSFWLevel.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various NSFW levels.
/// </summary>
[PublicAPI]
public enum GuildNSFWLevel
{
    /// <summary>
    /// The guild uses the default NSFW level.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The guild is an explicit guild.
    /// </summary>
    Explicit = 1,

    /// <summary>
    /// The guild is marked as safe.
    /// </summary>
    Safe = 2,

    /// <summary>
    /// The guild is age-restricted.
    /// </summary>
    AgeRestricted = 3
}
