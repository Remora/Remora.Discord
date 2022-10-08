//
//  InteractionCreate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Gateway.Events;

/// <inheritdoc cref="IInteractionCreate" />
[PublicAPI]
public record InteractionCreate
(
    Snowflake ID,
    Snowflake ApplicationID,
    InteractionType Type,
    Optional<OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>> Data,
    Optional<Snowflake> GuildID,
    Optional<Snowflake> ChannelID,
    Optional<IGuildMember> Member,
    Optional<IUser> User,
    string Token,
    int Version,
    Optional<IMessage> Message = default,
    Optional<IDiscordPermissionSet> AppPermissions = default,
    Optional<string> Locale = default,
    Optional<string> GuildLocale = default
) : IInteractionCreate;
