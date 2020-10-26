//
//  PartialMessage.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class PartialMessage : IPartialMessage
    {
        /// <inheritdoc />
        public Optional<Snowflake> ID { get; }

        /// <inheritdoc />
        public Optional<Snowflake> ChannelID { get; }

        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc />
        public Optional<IUser> Author { get; }

        /// <inheritdoc />
        public Optional<IPartialGuildMember> Member { get; }

        /// <inheritdoc />
        public Optional<string> Content { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> Timestamp { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset?> EditedTimestamp { get; }

        /// <inheritdoc />
        public Optional<bool> IsTTS { get; }

        /// <inheritdoc />
        public Optional<bool> MentionsEveryone { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IUserMention>> Mentions { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<Snowflake>> MentionedRoles { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IChannelMention>> MentionedChannels { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IAttachment>> Attachments { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IEmbed>> Embeds { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IReaction>> Reactions { get; }

        /// <inheritdoc />
        public Optional<string> Nonce { get; }

        /// <inheritdoc />
        public Optional<bool> IsPinned { get; }

        /// <inheritdoc />
        public Optional<Snowflake> WebhookID { get; }

        /// <inheritdoc />
        public Optional<MessageType> Type { get; }

        /// <inheritdoc />
        public Optional<IMessageActivity> Activity { get; }

        /// <inheritdoc />
        public Optional<IMessageApplication> Application { get; }

        /// <inheritdoc />
        public Optional<IMessageReference> MessageReference { get; }

        /// <inheritdoc />
        public Optional<MessageFlags> Flags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialMessage"/> class.
        /// </summary>
        /// <param name="id">The ID of the PartialMessage.</param>
        /// <param name="channelID">The ID of the channel the PartialMessage was sent in.</param>
        /// <param name="guildID">The ID of the guild the PartialMessage was sent in.</param>
        /// <param name="author">The ID of the PartialMessage author.</param>
        /// <param name="member">The member information of the author.</param>
        /// <param name="content">The contents of the PartialMessage.</param>
        /// <param name="timestamp">The time when the PartialMessage was sent.</param>
        /// <param name="editedTimestamp">The time when the PartialMessage was last edited.</param>
        /// <param name="isTTS">Whether the PartialMessage is a TTS PartialMessage.</param>
        /// <param name="mentionsEveryone">Whether the PartialMessage mentions everyone.</param>
        /// <param name="mentions">The people the PartialMessage mentions.</param>
        /// <param name="mentionedRoles">The roles the PartialMessage mentions.</param>
        /// <param name="mentionedChannels">The channels the PartialMessage mentions.</param>
        /// <param name="attachments">The PartialMessage attachments.</param>
        /// <param name="embeds">The PartialMessage embeds.</param>
        /// <param name="reactions">The reactions to the PartialMessage.</param>
        /// <param name="nonce">The PartialMessage nonce.</param>
        /// <param name="isPinned">Whether the PartialMessage is pinned.</param>
        /// <param name="webhookID">The ID of the webhook that sent this PartialMessage.</param>
        /// <param name="type">The PartialMessage type.</param>
        /// <param name="activity">The activity that the PartialMessage is associated with.</param>
        /// <param name="application">The application that the PartialMessage is associated with.</param>
        /// <param name="messageReference">The PartialMessage that this PartialMessage refers to.</param>
        /// <param name="flags">The PartialMessage flags.</param>
        public PartialMessage
        (
            Optional<Snowflake> id,
            Optional<Snowflake> channelID,
            Optional<Snowflake> guildID,
            Optional<IUser> author,
            Optional<IPartialGuildMember> member,
            Optional<string> content,
            Optional<DateTimeOffset> timestamp,
            Optional<DateTimeOffset?> editedTimestamp,
            Optional<bool> isTTS,
            Optional<bool> mentionsEveryone,
            Optional<IReadOnlyList<IUserMention>> mentions,
            Optional<IReadOnlyList<Snowflake>> mentionedRoles,
            Optional<IReadOnlyList<IChannelMention>> mentionedChannels,
            Optional<IReadOnlyList<IAttachment>> attachments,
            Optional<IReadOnlyList<IEmbed>> embeds,
            Optional<IReadOnlyList<IReaction>> reactions,
            Optional<string> nonce,
            Optional<bool> isPinned,
            Optional<Snowflake> webhookID,
            Optional<MessageType> type,
            Optional<IMessageActivity> activity,
            Optional<IMessageApplication> application,
            Optional<IMessageReference> messageReference,
            Optional<MessageFlags> flags
        )
        {
            this.ID = id;
            this.ChannelID = channelID;
            this.GuildID = guildID;
            this.Author = author;
            this.Member = member;
            this.Content = content;
            this.Timestamp = timestamp;
            this.EditedTimestamp = editedTimestamp;
            this.IsTTS = isTTS;
            this.MentionsEveryone = mentionsEveryone;
            this.Mentions = mentions;
            this.MentionedRoles = mentionedRoles;
            this.MentionedChannels = mentionedChannels;
            this.Attachments = attachments;
            this.Embeds = embeds;
            this.Reactions = reactions;
            this.Nonce = nonce;
            this.IsPinned = isPinned;
            this.WebhookID = webhookID;
            this.Type = type;
            this.Activity = activity;
            this.Application = application;
            this.MessageReference = messageReference;
            this.Flags = flags;
        }
    }
}
