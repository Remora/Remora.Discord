//
//  MessageCreate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc cref="IMessageCreate"/>
    public class MessageCreate : Message, IMessageCreate
    {
        /// <inheritdoc cref="Message"/>
        public MessageCreate
        (
            Snowflake id,
            Snowflake channelID,
            Optional<Snowflake> guildID,
            IUser author,
            Optional<IPartialGuildMember> member,
            string content,
            DateTimeOffset timestamp,
            DateTimeOffset? editedTimestamp,
            bool isTTS,
            bool mentionsEveryone,
            IReadOnlyList<IUserMention> mentions,
            IReadOnlyList<Snowflake> mentionedRoles,
            Optional<IReadOnlyList<IChannelMention>> mentionedChannels,
            IReadOnlyList<IAttachment> attachments,
            IReadOnlyList<IEmbed> embeds,
            Optional<IReadOnlyList<IReaction>> reactions,
            Optional<string> nonce,
            bool isPinned,
            Optional<Snowflake> webhookID,
            MessageType type,
            Optional<IMessageActivity> activity,
            Optional<IMessageApplication> application,
            Optional<IMessageReference> messageReference,
            Optional<MessageFlags> flags,
            Optional<IMessage?> referencedMessage
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
                flags,
                referencedMessage
            )
        {
        }
    }
}
