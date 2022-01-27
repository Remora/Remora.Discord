//
//  MessageCreateExtensions.cs
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

using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IMessageCreate"/> interface.
/// </summary>
public static class MessageCreateExtensions
{
    /// <summary>
    /// Creates a message context from the given message creation.
    /// </summary>
    /// <param name="messageCreate">The message creation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    public static Result<MessageContext> CreateContext(this IMessageCreate messageCreate)
    {
        return new MessageContext
        (
            messageCreate.ChannelID,
            messageCreate.Author,
            messageCreate.ID,
            new PartialMessage
            (
                messageCreate.ID,
                messageCreate.ChannelID,
                messageCreate.GuildID,
                new Optional<IUser>(messageCreate.Author),
                messageCreate.Member,
                messageCreate.Content,
                messageCreate.Timestamp,
                messageCreate.EditedTimestamp,
                messageCreate.IsTTS,
                messageCreate.MentionsEveryone,
                new Optional<IReadOnlyList<IUserMention>>(messageCreate.Mentions),
                new Optional<IReadOnlyList<Snowflake>>(messageCreate.MentionedRoles),
                messageCreate.MentionedChannels,
                new Optional<IReadOnlyList<IAttachment>>(messageCreate.Attachments),
                new Optional<IReadOnlyList<IEmbed>>(messageCreate.Embeds),
                messageCreate.Reactions,
                messageCreate.Nonce,
                messageCreate.IsPinned,
                messageCreate.WebhookID,
                messageCreate.Type,
                messageCreate.Activity,
                messageCreate.Application,
                messageCreate.ApplicationID,
                messageCreate.MessageReference,
                messageCreate.Flags,
                messageCreate.ReferencedMessage,
                messageCreate.Interaction,
                messageCreate.Thread,
                messageCreate.Components,
                messageCreate.StickerItems
            )
        );
    }
}
