//
//  Message.cs
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
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IMessage" />
[PublicAPI]
public record Message
(
    Snowflake ID,
    Snowflake ChannelID,
    IUser Author,
    string Content,
    DateTimeOffset Timestamp,
    DateTimeOffset? EditedTimestamp,
    bool IsTTS,
    bool MentionsEveryone,
    IReadOnlyList<IUser> Mentions,
    IReadOnlyList<Snowflake> MentionedRoles,
    Optional<IReadOnlyList<IChannelMention>> MentionedChannels,
    IReadOnlyList<IAttachment> Attachments,
    IReadOnlyList<IEmbed> Embeds,
    Optional<IReadOnlyList<IReaction>> Reactions,
    Optional<string> Nonce,
    bool IsPinned,
    Optional<Snowflake> WebhookID,
    MessageType Type,
    Optional<IMessageActivity> Activity = default,
    Optional<IPartialApplication> Application = default,
    Optional<Snowflake> ApplicationID = default,
    Optional<IMessageReference> MessageReference = default,
    Optional<MessageFlags> Flags = default,
    Optional<IMessage?> ReferencedMessage = default,
    Optional<IMessageInteraction> Interaction = default,
    Optional<IChannel> Thread = default,
    Optional<IReadOnlyList<IMessageComponent>> Components = default,
    Optional<IReadOnlyList<IStickerItem>> StickerItems = default,
    Optional<int> Position = default,
    Optional<IApplicationCommandInteractionDataResolved> Resolved = default,
    Optional<OneOf<IApplicationCommandInteractionMetadata, IMessageComponentInteractionMetadata, IModalSubmitInteractionMetadata>> InteractionMetadata = default
) : IMessage;
