//
//  DiscordDefaultMemberPermissionsAttribute.cs
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
using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a command as requiring the executor of the command to have the specified permissions. Specifying no
/// permissions will make the command or group inaccessible to anyone.
/// </summary>
/// <remarks>
/// Permissions serve as a base for command access, however moderators with the appropriate permissions can add
/// additional filters to the command, either adding or removing applicable users.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DiscordDefaultMemberPermissionsAttribute : Attribute
{
    /// <summary>
    /// Gets the required permissions to execute the command.
    /// </summary>
    public IReadOnlyList<DiscordPermission> Permissions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordDefaultMemberPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="permissions">The permissions required for executing the application command.</param>
    public DiscordDefaultMemberPermissionsAttribute(params DiscordPermission[] permissions)
    {
        this.Permissions = permissions;
    }
}
