//
//  IInteraction.cs
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

using JetBrains.Annotations;
using OneOf;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a user interaction.
/// </summary>
[PublicAPI]
public interface IInteraction
{
    /// <summary>
    /// Gets the interaction ID.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the application ID that the interaction is for.
    /// </summary>
    Snowflake ApplicationID { get; }

    /// <summary>
    /// Gets the type of the interaction.
    /// </summary>
    InteractionType Type { get; }

    /// <summary>
    /// Gets the data payload.
    /// </summary>
    Optional<OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>> Data { get; }

    /// <summary>
    /// Gets the ID of the guild the interaction was sent from.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the ID of the channel the interaction was sent from.
    /// </summary>
    Optional<Snowflake> ChannelID { get; }

    /// <summary>
    /// Gets the guild member that invoked the command.
    /// </summary>
    Optional<IGuildMember> Member { get; }

    /// <summary>
    /// Gets the user that invoked the command.
    /// </summary>
    Optional<IUser> User { get; }

    /// <summary>
    /// Gets a continuation token for responding to the interaction.
    /// </summary>
    /// <remarks>This token is valid for 15 minutes.</remarks>
    string Token { get; }

    /// <summary>
    /// Gets the version of the interaction API in use. Currently 1.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Gets the message the component was attached to.
    /// </summary>
    Optional<IMessage> Message { get; }

    /// <summary>
    /// Gets the computed permissions for the application in the context of the interaction's execution.
    /// </summary>
    Optional<IDiscordPermissionSet> AppPermissions { get; }

    /// <summary>
    /// Gets the locale of the invoking user.
    /// </summary>
    /// <remarks>
    /// This is sent for all interaction types, except <see cref="InteractionType.Ping"/>.
    /// </remarks>
    Optional<string> Locale { get; }

    /// <summary>
    /// Gets the locale of the guild the interaction was sent from.
    /// </summary>
    Optional<string> GuildLocale { get; }
}
