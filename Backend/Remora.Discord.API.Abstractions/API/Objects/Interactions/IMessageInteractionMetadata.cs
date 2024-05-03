//
//  IMessageInteractionMetadata.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents the metadata of an application command interaction.
/// </summary>
[PublicAPI]
public interface IMessageInteractionMetadata
{
    /// <summary>
    /// Gets the ID of the interaction.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the user who triggered the interaction.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    Optional<string> Name { get; }

    /// <summary>
    /// Gets the type of the interaction.
    /// </summary>
    InteractionType Type { get; }

    /// <summary>
    /// Gets the ID of the original response message. Only applicable to followup messages.
    /// </summary>
    Optional<Snowflake> OriginalResponseMessageID { get; }

    /// <summary>
    /// Gets the ID of the message containing the interactive component; only applicable to component interactions.
    /// </summary>
    Optional<Snowflake> InteractedMessageID { get; }

    /// <summary>
    /// Gets the integrations that authorized the interaction.
    /// </summary>
    /// <remarks>
    /// This is a mapping of the integration type to the ID of its resource.
    /// <para>
    /// The dictionary contains the following, given the circumstances: <br/>
    /// - If the integration is installed to a user, a key of <see cref="ApplicationIntegrationType.UserInstallable"/> and the value is the user ID. <br/>
    /// - If the integration is installed to a guild, a key of <see cref="ApplicationIntegrationType.GuildInstallable"/> and the value is the guild ID.
    /// If the interaction is sent outside the context of a guild, the value is always zero.<br/>
    /// </para>
    /// </remarks>
    IReadOnlyDictionary<ApplicationIntegrationType, Snowflake> AuthorizingIntegrationOwners { get; }

    /// <summary>
    /// Gets the interaction metadata responsible, if this is a response to a modal.
    /// </summary>
    Optional<IMessageInteractionMetadata> TriggeringInteractionMetadata { get; }
}
