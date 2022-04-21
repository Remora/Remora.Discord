﻿//
//  ChannelTypesAttribute.cs
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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a channel parameter with type requirements for Discord slash commands, controlling what channel autocompletion is presented to the user.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public class ChannelTypesAttribute : Attribute
{
    /// <summary>
    /// Gets the types of channel that are allowed to be presented as an autocomplete option.
    /// </summary>
    public IReadOnlyList<ChannelType> Types { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelTypesAttribute"/> class.
    /// </summary>
    /// <param name="types">The types of channel that are allowed to be presented as an autocomplete option.</param>
    public ChannelTypesAttribute(params ChannelType[] types)
    {
        this.Types = types;
    }
}
