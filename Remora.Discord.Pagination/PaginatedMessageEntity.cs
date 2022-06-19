//
//  PaginatedMessageEntity.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Pagination;

/// <summary>
/// Handles operations on a paginated message.
/// </summary>
internal sealed class PaginatedMessageEntity :
    InMemoryPersistentInteractiveEntity<PaginatedMessageData>,
    IButtonInteractiveEntity
{
    private readonly InteractionContext _context;
    private readonly FeedbackService _feedback;
    private readonly IDiscordRestChannelAPI _channelAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedMessageEntity"/> class.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    /// <param name="feedback">The feedback service.</param>
    /// <param name="channelAPI">The channel API.</param>
    public PaginatedMessageEntity
    (
        InteractionContext context,
        FeedbackService feedback,
        IDiscordRestChannelAPI channelAPI
    )
    {
        _context = context;
        _feedback = feedback;
        _channelAPI = channelAPI;
    }

    /// <inheritdoc />
    public override string Nonce => _context.Message.IsDefined(out var message)
        ? message.ID.ToString()
        : throw new InvalidOperationException();

    /// <inheritdoc/>
    public override Task<Result<bool>> IsInterestedAsync
    (
        ComponentType? componentType,
        string customID,
        CancellationToken ct = default
    )
    {
        if (componentType is not ComponentType.Button)
        {
            return Task.FromResult<Result<bool>>(false);
        }

        return Task.FromResult<Result<bool>>(this.Data.Appearance.Buttons.Select(b => b.CustomID).Contains(customID));
    }

    /// <inheritdoc />
    public async Task<Result> HandleInteractionAsync(IUser user, string customID, CancellationToken ct = default)
    {
        if (this.Data.SourceUserID != user.ID)
        {
            // Only the source user may change the page
            return Result.FromSuccess();
        }

        if (!_context.Message.IsDefined(out var message))
        {
            return new InvalidOperationError("The context did not contain a message.");
        }

        switch (customID)
        {
            case var _ when customID == this.Data.Appearance.First.CustomID:
            {
                if (!this.Data.MoveFirst())
                {
                    return Result.FromSuccess();
                }

                break;
            }
            case var _ when customID == this.Data.Appearance.Previous.CustomID:
            {
                if (!this.Data.MovePrevious())
                {
                    return Result.FromSuccess();
                }

                break;
            }
            case var _ when customID == this.Data.Appearance.Next.CustomID:
            {
                if (!this.Data.MoveNext())
                {
                    return Result.FromSuccess();
                }

                break;
            }
            case var _ when customID == this.Data.Appearance.Last.CustomID:
            {
                if (!this.Data.MoveLast())
                {
                    return Result.FromSuccess();
                }

                break;
            }
            case var _ when customID == this.Data.Appearance.Close.CustomID:
            {
                this.DeleteData = true;
                return await _channelAPI.DeleteMessageAsync(message.ChannelID, message.ID, ct: ct);
            }
            case var _ when customID == this.Data.Appearance.Help.CustomID:
            {
                var sendHelp = await _feedback.SendContextualInfoAsync
                (
                    this.Data.Appearance.HelpText,
                    this.Data.SourceUserID,
                    new FeedbackMessageOptions(MessageFlags: MessageFlags.Ephemeral),
                    ct
                );

                return (Result)sendHelp;
            }
            default:
            {
                return new InvalidOperationError("The pressed button was not recognized.");
            }
        }

        var newPage = this.Data.GetCurrentPage();
        var newComponents = this.Data.GetCurrentComponents();

        var editOriginal = await _channelAPI.EditMessageAsync
        (
            message.ChannelID,
            message.ID,
            embeds: new[] { newPage },
            components: new Optional<IReadOnlyList<IMessageComponent>?>
            (
                message.Components.IsDefined(out var existingComponents)
                     ? newComponents.Concat(existingComponents.Skip(newComponents.Count)).ToList()
                     : newComponents
            ),
            ct: ct
        );

        return (Result)editOriginal;
    }
}
