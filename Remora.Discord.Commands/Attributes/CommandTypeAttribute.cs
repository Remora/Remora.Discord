//
//  CommandTypeAttribute.cs
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
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a command as being of a specific type (chat input, context menu, etc).
/// </summary>
[PublicAPI, AttributeUsage(AttributeTargets.Method)]
public class CommandTypeAttribute : Attribute
{
    /// <summary>
    /// Gets the command type.
    /// </summary>
    public ApplicationCommandType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandTypeAttribute"/> class.
    /// </summary>
    /// <param name="type">The command type.</param>
    public CommandTypeAttribute(ApplicationCommandType type)
    {
        this.Type = type;
    }
}
