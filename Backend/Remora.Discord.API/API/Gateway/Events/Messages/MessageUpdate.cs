//
//  MessageUpdate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc cref="IMessageUpdate"/>
    [PublicAPI]
    public record MessageUpdate : PartialMessage, IMessageUpdate
    {
        /// <inheritdoc cref="PartialMessage"/>
        public MessageUpdate
        (
            Optional<Snowflake> id = default,
            Optional<Snowflake> channelID = default,
            Optional<Snowflake> guildID = default,
            Optional<IUser> author = default,
            Optional<IPartialGuildMember> member = default,
            Optional<string> content = default,
            Optional<DateTimeOffset> timestamp = default,
            Optional<DateTimeOffset?> editedTimestamp = default,
            Optional<bool> isTTS = default,
            Optional<bool> mentionsEveryone = default,
            Optional<IReadOnlyList<IUserMention>> mentions = default,
            Optional<IReadOnlyList<Snowflake>> mentionedRoles = default,
            Optional<IReadOnlyList<IChannelMention>> mentionedChannels = default,
            Optional<IReadOnlyList<IAttachment>> attachments = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IReadOnlyList<IReaction>> reactions = default,
            Optional<string> nonce = default,
            Optional<bool> isPinned = default,
            Optional<Snowflake> webhookID = default,
            Optional<MessageType> type = default,
            Optional<IMessageActivity> activity = default,
            Optional<IMessageApplication> application = default,
            Optional<IMessageReference> messageReference = default,
            Optional<MessageFlags> flags = default
        )
            : base
            (
                id,
                channelID,
                guildID,
                author,
                member,
                content,
                timestamp,
                editedTimestamp,
                isTTS,
                mentionsEveryone,
                mentions,
                mentionedRoles,
                mentionedChannels,
                attachments,
                embeds,
                reactions,
                nonce,
                isPinned,
                webhookID,
                type,
                activity,
                application,
                messageReference,
                flags
            )
        {
        }
    }
}
