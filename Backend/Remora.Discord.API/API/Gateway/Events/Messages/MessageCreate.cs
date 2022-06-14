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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Gateway.Events;

/// <inheritdoc cref="IMessageCreate"/>
[PublicAPI]
public record MessageCreate
(
    Optional<Snowflake> GuildID,
    Optional<IPartialGuildMember> Member,
    IReadOnlyList<IUserMention> Mentions,
    Snowflake ID,
    Snowflake ChannelID,
    IUser Author,
    string Content,
    DateTimeOffset Timestamp,
    DateTimeOffset? EditedTimestamp,
    bool IsTTS,
    bool MentionsEveryone,
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
    Optional<IReadOnlyList<IStickerItem>> StickerItems = default
) : Message
(
    ID,
    ChannelID,
    Author,
    Content,
    Timestamp,
    EditedTimestamp,
    IsTTS,
    MentionsEveryone,
    MentionedRoles,
    MentionedChannels,
    Attachments,
    Embeds,
    Reactions,
    Nonce,
    IsPinned,
    WebhookID,
    Type,
    Activity,
    Application,
    ApplicationID,
    MessageReference,
    Flags,
    ReferencedMessage,
    Interaction,
    Thread,
    Components,
    StickerItems
), IMessageCreate;
