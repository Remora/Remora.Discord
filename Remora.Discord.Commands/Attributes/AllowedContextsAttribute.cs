//
//  AllowedContextsAttribute.cs
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
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Defines the contexts in which a command can be invoked.
/// </summary>
/// <param name="contexts">The contexts the command can be invoked.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowedContextsAttribute(params InteractionContextType[] contexts) : Attribute
{
    /// <summary>
    /// Gets a value specifying the allowed contexts.
    /// </summary>
    public IReadOnlyList<InteractionContextType> Contexts { get; } = contexts.ToArray();
}
