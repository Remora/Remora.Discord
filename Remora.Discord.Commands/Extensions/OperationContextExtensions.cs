//
//  OperationContextExtensions.cs
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

using JetBrains.Annotations;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IOperationContext"/> interface.
/// </summary>
[PublicAPI]
public static class OperationContextExtensions
{
    /// <summary>
    /// Attempts to extract the ID of the user the context is associated with.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>true if an ID was extracted; otherwise, false.</returns>
    public static bool TryGetUserID(this IOperationContext context, out Snowflake userID)
    {
        userID = default;

        switch (context)
        {
            case IInteractionContext interactionCommandContext:
            {
                if (interactionCommandContext.Interaction.User.TryGet(out var user))
                {
                    userID = user.ID;
                    return true;
                }

                if (interactionCommandContext.Interaction.Member.TryGet(out var member))
                {
                    if (member.User.TryGet(out user))
                    {
                        userID = user.ID;
                        return true;
                    }
                }

                break;
            }
            case IMessageContext { Message.Author.HasValue: true } messageContext:
            {
                userID = messageContext.Message.Author.Value.ID;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to extract the ID of the guild the context is associated with.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>true if an ID was extracted; otherwise, false.</returns>
    public static bool TryGetGuildID(this IOperationContext context, out Snowflake guildID)
    {
        guildID = default;

        switch (context)
        {
            case IInteractionContext interactionCommandContext:
            {
                if (interactionCommandContext.Interaction.GuildID.TryGet(out var id))
                {
                    guildID = id;
                    return true;
                }

                break;
            }
            case IMessageContext messageContext:
            {
                if (messageContext.GuildID.TryGet(out var id))
                {
                    guildID = id;
                    return true;
                }

                break;
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to extract the ID of the channel the context is associated with.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="channelID">The ID of the channel.</param>
    /// <returns>true if an ID was extracted; otherwise, false.</returns>
    public static bool TryGetChannelID(this IOperationContext context, out Snowflake channelID)
    {
        channelID = default;

        switch (context)
        {
            case IInteractionContext interactionCommandContext:
            {
                if (interactionCommandContext.Interaction.Channel.TryGet(out var channel) && channel.ID.TryGet(out var id))
                {
                    channelID = id;
                    return true;
                }

                break;
            }
            case IMessageContext messageContext:
            {
                if (messageContext.Message.ChannelID.TryGet(out var id))
                {
                    channelID = id;
                    return true;
                }

                break;
            }
        }

        return false;
    }
}
