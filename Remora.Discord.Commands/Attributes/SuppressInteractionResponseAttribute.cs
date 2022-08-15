//
//  SuppressInteractionResponseAttribute.cs
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
/// Marks a command group or individual command with a desired response behaviour for interactions.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SuppressInteractionResponseAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether an automatic response should be suppressed.
    /// </summary>
    public bool Suppress { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuppressInteractionResponseAttribute"/> class.
    /// </summary>
    /// <param name="suppress">true of the response should be suppressed; otherwise, false.</param>
    public SuppressInteractionResponseAttribute(bool suppress)
    {
        this.Suppress = suppress;
    }
}
