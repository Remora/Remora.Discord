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
using Remora.Discord.API.Abstractions.Channels;
using Remora.Discord.API.Abstractions.Events;
using Remora.Discord.API.Abstractions.Guilds;
using Remora.Discord.API.Abstractions.Messages;
using Remora.Discord.API.Abstractions.Reactions;
using Remora.Discord.API.Abstractions.Users;
using Remora.Discord.API.Objects.Messages;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events.Messages
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
            Optional<IGuildMember> member,
            string content,
            DateTime timestamp,
            DateTime? editedTimestamp,
            bool isTTS,
            bool mentionsEveryone,
            IReadOnlyList<(IUser User, Optional<IGuildMember> Member)> mentions,
            IReadOnlyList<Snowflake> mentionedRoles,
            Optional<IReadOnlyList<IChannelMention>> mentionedChannels,
            IReadOnlyList<IAttachment> attachments,
            IReadOnlyList<IEmbed> embeds,
            Optional<IReaction> reactions,
            Optional<string> nonce,
            bool isPinned,
            Optional<Snowflake> webhookID,
            MessageType type,
            Optional<IMessageActivity> activity,
            Optional<IMessageApplication> application,
            Optional<IMessageReference> messageReference,
            Optional<MessageFlags> flags
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
