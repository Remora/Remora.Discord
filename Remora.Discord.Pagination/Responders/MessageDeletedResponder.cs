//
//  MessageDeletedResponder.cs
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

using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Interactivity.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Pagination.Responders;

/// <summary>
/// Responds to deleted messages, deleting persistent pagination data.
/// </summary>
internal sealed class MessageDeletedResponder : IResponder<IMessageDelete>, IResponder<IMessageDeleteBulk>
{
    private readonly InMemoryDataService<Snowflake, PaginatedMessageData> _paginationData;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDeletedResponder"/> class.
    /// </summary>
    /// <param name="paginationData">The pagination data service.</param>
    public MessageDeletedResponder(InMemoryDataService<Snowflake, PaginatedMessageData> paginationData)
    {
        _paginationData = paginationData;
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
    {
        _ = await _paginationData.TryRemoveDataAsync(gatewayEvent.ID);
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IMessageDeleteBulk gatewayEvent, CancellationToken ct = default)
    {
        foreach (var id in gatewayEvent.IDs)
        {
            _ = await _paginationData.TryRemoveDataAsync(id);
        }

        return Result.FromSuccess();
    }
}
