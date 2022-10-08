//
//  IDiscordPermissionSet.cs
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

using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a Discord permission set.
/// </summary>
[PublicAPI]
public interface IDiscordPermissionSet
{
    /// <summary>
    /// Gets the raw value of the permission set.
    /// </summary>
    BigInteger Value { get; }

    /// <summary>
    /// Determines whether the set allows the given permission.
    /// </summary>
    /// <param name="permission">The permission.</param>
    /// <returns>true if the given permission is in the set; otherwise, false.</returns>
    bool HasPermission(DiscordPermission permission);

    /// <summary>
    /// Determines whether the set allows the given permission.
    /// </summary>
    /// <param name="permission">The permission.</param>
    /// <returns>true if the given permission is in the set; otherwise, false.</returns>
    bool HasPermission(DiscordTextPermission permission);

    /// <summary>
    /// Determines whether the set allows the given permission.
    /// </summary>
    /// <param name="permission">The permission.</param>
    /// <returns>true if the given permission is in the set; otherwise, false.</returns>
    bool HasPermission(DiscordVoicePermission permission);

    /// <summary>
    /// Gets a list of the <see cref="DiscordPermission"/> values contained within the set.
    /// </summary>
    /// <returns>A list of <see cref="DiscordPermission"/> value.</returns>
    IReadOnlyList<DiscordPermission> GetPermissions();
}
