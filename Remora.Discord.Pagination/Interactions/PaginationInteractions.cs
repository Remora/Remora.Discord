//
//  PaginationInteractions.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Discord.Interactivity.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Pagination.Interactions;

/// <summary>
/// Defines interaction commands for pagination.
/// </summary>
internal class PaginationInteractions : InteractionGroup
{
    private readonly FeedbackService _feedback;
    private readonly IDiscordRestChannelAPI _channelAPI;
    private readonly IDiscordRestInteractionAPI _interactionAPI;
    private readonly InMemoryDataService<Snowflake, PaginatedMessageData> _paginationData;
    private readonly InteractionContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationInteractions"/> class.
    /// </summary>
    /// <param name="feedback">The feedback service.</param>
    /// <param name="channelAPI">The channel API.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    /// <param name="paginationData">The pagination data service.</param>
    /// <param name="context">The interaction context.</param>
    public PaginationInteractions
    (
        FeedbackService feedback,
        IDiscordRestChannelAPI channelAPI,
        IDiscordRestInteractionAPI interactionAPI,
        InMemoryDataService<Snowflake, PaginatedMessageData> paginationData,
        InteractionContext context
    )
    {
        _feedback = feedback;
        _channelAPI = channelAPI;
        _interactionAPI = interactionAPI;
        _paginationData = paginationData;
        _context = context;
    }

    /// <summary>
    /// Returns the paginated message to the first page.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("first")]
    public Task<Result> MoveFirstAsync() => UpdateAsync(d => d.MoveFirst(), this.CancellationToken);

    /// <summary>
    /// Moves the paginated message back by one page.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("previous")]
    public Task<Result> MovePreviousAsync() => UpdateAsync(d => d.MovePrevious(), this.CancellationToken);

    /// <summary>
    /// Advances the paginated message by one page.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("next")]
    public Task<Result> MoveNextAsync() => UpdateAsync(d => d.MoveNext(), this.CancellationToken);

    /// <summary>
    /// Moves the paginated message to the last page.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("last")]
    public Task<Result> MoveLastAsync() => UpdateAsync(d => d.MoveLast(), this.CancellationToken);

    /// <summary>
    /// Closes the paginated message.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("close")]
    public async Task<Result> CloseAsync()
    {
        if (!_context.Message.IsDefined(out var message))
        {
            return new InvalidOperationError("No message available for the interaction.");
        }

        var leaseData = await _paginationData.LeaseDataAsync(message.ID, this.CancellationToken);
        if (!leaseData.IsSuccess)
        {
            return (Result)leaseData;
        }

        await using var lease = leaseData.Entity;
        lease.Delete();

        if (lease.Data.IsInteractionDriven)
        {
            return await _interactionAPI.DeleteOriginalInteractionResponseAsync
            (
                _context.ApplicationID,
                _context.Token,
                this.CancellationToken
            );
        }

        return await _channelAPI.DeleteMessageAsync(message.ChannelID, message.ID, ct: this.CancellationToken);
    }

    /// <summary>
    /// Shows a help message.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("help")]
    public async Task<Result> HelpAsync()
    {
        if (!_context.Message.IsDefined(out var message))
        {
            return new InvalidOperationError("No message available for the interaction.");
        }

        var leaseData = await _paginationData.LeaseDataAsync(message.ID, this.CancellationToken);
        if (!leaseData.IsSuccess)
        {
            return (Result)leaseData;
        }

        await using var lease = leaseData.Entity;

        return (Result)await _feedback.SendContextualInfoAsync
        (
            lease.Data.Appearance.HelpText,
            lease.Data.SourceUserID,
            new FeedbackMessageOptions(MessageFlags: MessageFlags.Ephemeral),
            this.CancellationToken
        );
    }

    private async Task<Result> UpdateAsync(Action<PaginatedMessageData> action, CancellationToken ct)
    {
        if (!_context.Message.IsDefined(out var message))
        {
            return new InvalidOperationError("No message available for the interaction.");
        }

        var leaseData = await _paginationData.LeaseDataAsync(message.ID, this.CancellationToken);
        if (!leaseData.IsSuccess)
        {
            return (Result)leaseData;
        }

        await using var lease = leaseData.Entity;

        action(lease.Data);

        var newPage = lease.Data.GetCurrentPage();
        var newComponents = lease.Data.GetCurrentComponents();

        if (lease.Data.IsInteractionDriven)
        {
            return (Result)await _interactionAPI.EditOriginalInteractionResponseAsync
            (
                _context.ApplicationID,
                _context.Token,
                embeds: new[] { newPage },
                components: new Optional<IReadOnlyList<IMessageComponent>?>
                (
                    message.Components.IsDefined(out var existingComponents)
                        ? newComponents.Concat(existingComponents.Skip(newComponents.Count)).ToList()
                        : newComponents
                ),
                ct: ct
            );
        }
        else
        {
            return (Result)await _channelAPI.EditMessageAsync
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
        }
    }
}
