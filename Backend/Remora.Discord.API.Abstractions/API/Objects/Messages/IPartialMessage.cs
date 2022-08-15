//
//  IPartialMessage.cs
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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a message.
/// </summary>
[PublicAPI]
public interface IPartialMessage
{
    /// <inheritdoc cref="IMessage.ID" />
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IMessage.ChannelID" />
    Optional<Snowflake> ChannelID { get; }

    /// <inheritdoc cref="IMessage.Author" />
    Optional<IUser> Author { get; }

    /// <inheritdoc cref="IMessage.Content" />
    Optional<string> Content { get; }

    /// <inheritdoc cref="IMessage.Timestamp" />
    Optional<DateTimeOffset> Timestamp { get; }

    /// <inheritdoc cref="IMessage.EditedTimestamp" />
    Optional<DateTimeOffset?> EditedTimestamp { get; }

    /// <inheritdoc cref="IMessage.IsTTS" />
    Optional<bool> IsTTS { get; }

    /// <inheritdoc cref="IMessage.MentionsEveryone" />
    Optional<bool> MentionsEveryone { get; }

    /// <inheritdoc cref="IMessage.MentionedRoles" />
    Optional<IReadOnlyList<Snowflake>> MentionedRoles { get; }

    /// <inheritdoc cref="IMessage.MentionedChannels" />
    Optional<IReadOnlyList<IChannelMention>> MentionedChannels { get; }

    /// <inheritdoc cref="IMessage.Attachments" />
    Optional<IReadOnlyList<IAttachment>> Attachments { get; }

    /// <inheritdoc cref="IMessage.Embeds" />
    Optional<IReadOnlyList<IEmbed>> Embeds { get; }

    /// <inheritdoc cref="IMessage.Reactions" />
    Optional<IReadOnlyList<IReaction>> Reactions { get; }

    /// <inheritdoc cref="IMessage.Nonce" />
    Optional<string> Nonce { get; }

    /// <inheritdoc cref="IMessage.IsPinned" />
    Optional<bool> IsPinned { get; }

    /// <inheritdoc cref="IMessage.WebhookID" />
    Optional<Snowflake> WebhookID { get; }

    /// <inheritdoc cref="IMessage.Type" />
    Optional<MessageType> Type { get; }

    /// <inheritdoc cref="IMessage.Activity" />
    Optional<IMessageActivity> Activity { get; }

    /// <inheritdoc cref="IMessage.Application" />
    Optional<IPartialApplication> Application { get; }

    /// <inheritdoc cref="IMessage.ApplicationID" />
    Optional<Snowflake> ApplicationID { get; }

    /// <inheritdoc cref="IMessage.MessageReference" />
    Optional<IMessageReference> MessageReference { get;  }

    /// <inheritdoc cref="IMessage.Flags" />
    Optional<MessageFlags> Flags { get; }

    /// <inheritdoc cref="IMessage.ReferencedMessage" />
    Optional<IMessage?> ReferencedMessage { get; }

    /// <inheritdoc cref="IMessage.Interaction" />
    Optional<IMessageInteraction> Interaction { get; }

    /// <inheritdoc cref="IMessage.Thread" />
    Optional<IChannel> Thread { get; }

    /// <inheritdoc cref="IMessage.Components" />
    Optional<IReadOnlyList<IMessageComponent>> Components { get; }

    /// <inheritdoc cref="IMessage.StickerItems" />
    Optional<IReadOnlyList<IStickerItem>> StickerItems { get; }

    /// <inheritdoc cref="IMessage.Position" />
    Optional<int> Position { get; }
}
