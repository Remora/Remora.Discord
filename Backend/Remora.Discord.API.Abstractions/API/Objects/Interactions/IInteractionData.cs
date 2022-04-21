//
//  IInteractionData.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents payload data for a command.
/// </summary>
[PublicAPI]
public interface IInteractionData
{
    /// <summary>
    /// Gets the ID of the invoked command.
    /// </summary>
    Optional<Snowflake> ID { get; }

    /// <summary>
    /// Gets the name of the invoked command.
    /// </summary>
    Optional<string> Name { get; }

    /// <summary>
    /// Gets any entities that were resolved while executing the command serverside.
    /// </summary>
    Optional<IApplicationCommandInteractionDataResolved> Resolved { get; }

    /// <summary>
    /// Gets the parameters and values supplied by the user.
    /// </summary>
    Optional<IReadOnlyList<IApplicationCommandInteractionDataOption>> Options { get; }

    /// <summary>
    /// Gets the ID of the guild the command is registered to.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the custom ID associated with this interaction.
    /// </summary>
    Optional<string> CustomID { get; }

    /// <summary>
    /// Gets the type of component that the data originated from.
    /// </summary>
    Optional<ComponentType> ComponentType { get; }

    /// <summary>
    /// Gets the values selected by the user.
    /// </summary>
    Optional<IReadOnlyList<string>> Values { get; }

    /// <summary>
    /// Gets the ID of the user or message targeted by an interaction.
    /// </summary>
    Optional<Snowflake> TargetID { get; }

    /// <summary>
    /// Gets the components for this interaction.
    /// </summary>
    Optional<IReadOnlyList<IPartialMessageComponent>> Components { get; }
}
