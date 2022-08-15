//
//  IMessageCreate.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents the creation of a message.
/// </summary>
[PublicAPI]
public interface IMessageCreate : IGatewayEvent, IMessage
{
    /// <summary>
    /// Gets the ID of the guild the message was sent in.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the member properties for the author. The member object exists in MESSAGE_CREATE and
    /// MESSAGE_UPDATE events from text-based guild channels. This allows bots to obtain real-time member data
    /// without requiring bots to keep member state in memory.
    /// </summary>
    Optional<IPartialGuildMember> Member { get; }

    /// <summary>
    /// Gets a list of users mentioned in the message.
    /// </summary>
    IReadOnlyList<IUserMention> Mentions { get; }
}
