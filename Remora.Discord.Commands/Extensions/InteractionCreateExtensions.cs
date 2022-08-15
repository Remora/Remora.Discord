//
//  InteractionCreateExtensions.cs
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
/// Defines extension methods for the <see cref="IInteractionCreate"/> interface.
/// </summary>
public static class InteractionCreateExtensions
{
    /// <summary>
    /// Creates an interaction context from the given interaction.
    /// </summary>
    /// <param name="interactionCreate">The interaction.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    public static Result<InteractionContext> CreateContext(this IInteractionCreate interactionCreate)
    {
        if (!interactionCreate.Data.IsDefined(out var interactionData))
        {
            return new InvalidOperationError("The interaction has no data.");
        }

        if (!interactionCreate.ChannelID.IsDefined(out var channelID))
        {
            return new InvalidOperationError("The interaction has no channel ID.");
        }

        var user = interactionCreate.User.HasValue
            ? interactionCreate.User.Value
            : interactionCreate.Member.HasValue
                ? interactionCreate.Member.Value.User.HasValue
                    ? interactionCreate.Member.Value.User.Value
                    : null
                : null;

        if (user is null)
        {
            return new InvalidOperationError("No user could be resolved from the interaction.");
        }

        return new InteractionContext
        (
            interactionCreate.GuildID,
            channelID,
            user,
            interactionCreate.Member,
            interactionCreate.Token,
            interactionCreate.ID,
            interactionCreate.ApplicationID,
            interactionData,
            interactionCreate.Message,
            interactionCreate.Locale
        );
    }
}
