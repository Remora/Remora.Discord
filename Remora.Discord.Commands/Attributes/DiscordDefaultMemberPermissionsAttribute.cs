//
//  DiscordDefaultMemberPermissionsAttribute.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a group with a set of permissions, that is the permissions required
/// for the user to be able to execute the commands within the group. Permissions
/// can be overridden by moderators.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DiscordDefaultMemberPermissionsAttribute : Attribute
{
    /// <summary>
    /// Gets the required permissions to execute the command.
    /// </summary>
    public DiscordPermission Permissions { get; }
}
