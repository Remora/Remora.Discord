//
//  MessageUpdateExtensions.cs
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

using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IMessageUpdate"/> interface.
/// </summary>
public static class MessageUpdateExtensions
{
    /// <summary>
    /// Creates a message context from the given message update.
    /// </summary>
    /// <param name="messageUpdate">The message update.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    public static Result<MessageContext> CreateContext(this IMessageUpdate messageUpdate)
    {
        if (!messageUpdate.ID.IsDefined(out var messageID))
        {
            return new InvalidOperationError("The message update did not contain the message ID.");
        }

        if (!messageUpdate.ChannelID.IsDefined(out var channelID))
        {
            return new InvalidOperationError("The message update did not contain the channel ID.");
        }

        if (!messageUpdate.Author.IsDefined(out var author))
        {
            return new InvalidOperationError("The message update did not the author.");
        }

        return new MessageContext
        (
            messageUpdate.GuildID,
            channelID,
            author,
            messageID,
            messageUpdate
        );
    }
}
