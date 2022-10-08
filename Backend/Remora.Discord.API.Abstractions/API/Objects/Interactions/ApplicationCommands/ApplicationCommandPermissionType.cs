//
//  ApplicationCommandPermissionType.cs
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
/// Enumerates application permission types.
/// </summary>
[PublicAPI]
public enum ApplicationCommandPermissionType
{
    /// <summary>
    /// The permission applies to a role.
    /// </summary>
    Role = 1,

    /// <summary>
    /// The permission applies to a user.
    /// </summary>
    User = 2,

    /// <summary>
    /// The permission applies to a channel.
    /// </summary>
    Channel = 3
}
