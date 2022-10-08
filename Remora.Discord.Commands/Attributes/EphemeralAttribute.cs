//
//  EphemeralAttribute.cs
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
/// Marks a command as requiring an ephemeral response, when invoked by an interaction.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EphemeralAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether this command should send ephemeral responses.
    /// </summary>
    public bool IsEphemeral { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EphemeralAttribute"/> class.
    /// </summary>
    /// <param name="isEphemeral">A value indicating whether this command should send ephemeral responses. Set this to override group-level <see cref="EphemeralAttribute"/>s.</param>
    public EphemeralAttribute(bool isEphemeral = true)
    {
        this.IsEphemeral = isEphemeral;
    }
}
