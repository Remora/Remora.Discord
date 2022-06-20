//
//  IMessage.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a message.
/// </summary>
[PublicAPI]
public interface IMessage : IPartialMessage
{
    /// <summary>
    /// Gets the message ID.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the channel the message was sent in.
    /// </summary>
    new Snowflake ChannelID { get; }

    /// <summary>
    /// Gets the author of the message. This author is not guaranteed to be a valid user; in the case of a webhook
    /// message, the object corresponds to the webhook's ID, username, and avatar - this is the case when
    /// <see cref="WebhookID"/> contains a valid value.
    /// </summary>
    new IUser Author { get; }

    /// <summary>
    /// Gets the contents of the message.
    /// </summary>
    new string Content { get; }

    /// <summary>
    /// Gets the time when the messages was sent.
    /// </summary>
    new DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the time when the message was last edited.
    /// </summary>
    new DateTimeOffset? EditedTimestamp { get; }

    /// <summary>
    /// Gets a value indicating whether this was a TTS message.
    /// </summary>
    new bool IsTTS { get; }

    /// <summary>
    /// Gets a value indicating whether this message mentions everyone.
    /// </summary>
    new bool MentionsEveryone { get; }

    /// <summary>
    /// Gets a list of mentioned roles.
    /// </summary>
    new IReadOnlyList<Snowflake> MentionedRoles { get; }

    /// <summary>
    /// Gets a list of channel mentions.
    /// <remarks>
    /// Not all channel mentions in a message will appear in mention_channels. Only
    /// textual channels that are visible to everyone in a lurkable guild will ever be included. Only crossposted
    /// messages (via Channel Following) currently include mention_channels at all. If no mentions in the message
    /// meet these requirements, this field will not be sent.
    /// </remarks>
    /// </summary>
    new Optional<IReadOnlyList<IChannelMention>> MentionedChannels { get; }

    /// <summary>
    /// Gets a list of attached files.
    /// </summary>
    new IReadOnlyList<IAttachment> Attachments { get; }

    /// <summary>
    /// Gets a list of embeds.
    /// </summary>
    new IReadOnlyList<IEmbed> Embeds { get; }

    /// <summary>
    /// Gets an array of reaction objects.
    /// </summary>
    new Optional<IReadOnlyList<IReaction>> Reactions { get; }

    /// <summary>
    /// Gets a nonce, used for validating a message was sent. Technically, this can be either an integer or a
    /// string.
    /// </summary>
    new Optional<string> Nonce { get; }

    /// <summary>
    /// Gets a value indicating whether the messages is pinned.
    /// </summary>
    new bool IsPinned { get; }

    /// <summary>
    /// Gets the ID of the webhook that sent this message.
    /// </summary>
    new Optional<Snowflake> WebhookID { get; }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    new MessageType Type { get; }

    /// <summary>
    /// Gets the activity the message belongs to. Sent with rich presence-related chat embeds.
    /// </summary>
    new Optional<IMessageActivity> Activity { get; }

    /// <summary>
    /// Gets the application the message belongs to. Sent with rich presence-related chat embeds.
    /// </summary>
    new Optional<IPartialApplication> Application { get; }

    /// <summary>
    /// Gets the ID of the application the message's interaction belongs to. Sent with interactions or application-owned
    /// webhooks.
    /// </summary>
    new Optional<Snowflake> ApplicationID { get; }

    /// <summary>
    /// Gets the message reference. Sent with cross-posted messages.
    /// </summary>
    new Optional<IMessageReference> MessageReference { get;  }

    /// <summary>
    /// Gets a set of bitwise flags describing extra features of the message.
    /// </summary>
    new Optional<MessageFlags> Flags { get; }

    /// <summary>
    /// Gets the referenced message, if any. A null value in this context refers to a deleted message.
    /// </summary>
    new Optional<IMessage?> ReferencedMessage { get; }

    /// <summary>
    /// Gets the interaction associated with this message, if any.
    /// </summary>
    new Optional<IMessageInteraction> Interaction { get; }

    /// <summary>
    /// Gets the thread that was started from this message, if any.
    /// </summary>
    new Optional<IChannel> Thread { get; }

    /// <summary>
    /// Gets the components in the message.
    /// </summary>
    new Optional<IReadOnlyList<IMessageComponent>> Components { get; }

    /// <summary>
    /// Gets the stickers sent with the message.
    /// </summary>
    new Optional<IReadOnlyList<IStickerItem>> StickerItems { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialMessage.ID => this.ID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialMessage.ChannelID => this.ChannelID;

    /// <inheritdoc/>
    Optional<IUser> IPartialMessage.Author => new(this.Author);

    /// <inheritdoc/>
    Optional<string> IPartialMessage.Content => this.Content;

    /// <inheritdoc/>
    Optional<DateTimeOffset> IPartialMessage.Timestamp => this.Timestamp;

    /// <inheritdoc/>
    Optional<DateTimeOffset?> IPartialMessage.EditedTimestamp => this.EditedTimestamp;

    /// <inheritdoc/>
    Optional<bool> IPartialMessage.IsTTS => this.IsTTS;

    /// <inheritdoc/>
    Optional<bool> IPartialMessage.MentionsEveryone => this.MentionsEveryone;

    /// <inheritdoc/>
    Optional<IReadOnlyList<Snowflake>> IPartialMessage.MentionedRoles => new(this.MentionedRoles);

    /// <inheritdoc/>
    Optional<IReadOnlyList<IChannelMention>> IPartialMessage.MentionedChannels => this.MentionedChannels;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IAttachment>> IPartialMessage.Attachments => new(this.Attachments);

    /// <inheritdoc/>
    Optional<IReadOnlyList<IEmbed>> IPartialMessage.Embeds => new(this.Embeds);

    /// <inheritdoc/>
    Optional<IReadOnlyList<IReaction>> IPartialMessage.Reactions => this.Reactions;

    /// <inheritdoc/>
    Optional<string> IPartialMessage.Nonce => this.Nonce;

    /// <inheritdoc/>
    Optional<bool> IPartialMessage.IsPinned => this.IsPinned;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialMessage.WebhookID => this.WebhookID;

    /// <inheritdoc/>
    Optional<MessageType> IPartialMessage.Type => this.Type;

    /// <inheritdoc/>
    Optional<IMessageActivity> IPartialMessage.Activity => this.Activity;

    /// <inheritdoc/>
    Optional<IPartialApplication> IPartialMessage.Application => this.Application;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialMessage.ApplicationID => this.ApplicationID;

    /// <inheritdoc/>
    Optional<IMessageReference> IPartialMessage.MessageReference => this.MessageReference;

    /// <inheritdoc/>
    Optional<MessageFlags> IPartialMessage.Flags => this.Flags;

    /// <inheritdoc/>
    Optional<IMessage?> IPartialMessage.ReferencedMessage => this.ReferencedMessage;

    /// <inheritdoc/>
    Optional<IMessageInteraction> IPartialMessage.Interaction => this.Interaction;

    /// <inheritdoc/>
    Optional<IChannel> IPartialMessage.Thread => this.Thread;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IMessageComponent>> IPartialMessage.Components => this.Components;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IStickerItem>> IPartialMessage.StickerItems => this.StickerItems;
}
