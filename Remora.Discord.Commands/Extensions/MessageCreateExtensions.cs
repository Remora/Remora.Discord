//
//  MessageCreateExtensions.cs
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
            messageCreate.GuildID,
            messageCreate.ChannelID,
            messageCreate.Author,
            messageCreate.ID,
            messageCreate
        );
    }
}
