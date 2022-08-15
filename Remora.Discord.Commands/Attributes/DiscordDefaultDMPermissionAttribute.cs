//
//  DiscordDefaultDMPermissionAttribute.cs
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

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a command or group as being either accessible or inaccessible in a DM.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DiscordDefaultDMPermissionAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether the command group is executable in a DM.
    /// </summary>
    public bool IsExecutableInDMs { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordDefaultDMPermissionAttribute"/> class.
    /// </summary>
    /// <param name="isExecutableInDMs">Whether this command group is executable in a DM.</param>
    public DiscordDefaultDMPermissionAttribute(bool isExecutableInDMs = true)
    {
        this.IsExecutableInDMs = isExecutableInDMs;
    }
}
