//
//  DiscordNsfwAttribute.cs
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

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a command or group as being age-restricted, disallowing its use in unrestricted channels. This attribute does
/// not perform any botside checks, and serves only to flag the command on Discord's end.
///
/// Due to Discord's current design, it is only supported on top-level groups or commands.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DiscordNsfwAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether the command or group is age-restricted.
    /// </summary>
    public bool IsNsfw { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordNsfwAttribute"/> class.
    /// </summary>
    /// <param name="isNSFW">Whether this command or group is age-restricted.</param>
    public DiscordNsfwAttribute(bool isNSFW = true)
    {
        this.IsNsfw = isNSFW;
    }
}
