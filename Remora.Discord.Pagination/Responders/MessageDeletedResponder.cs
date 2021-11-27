//
//  MessageDeletedResponder.cs
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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Pagination.Responders;

/// <summary>
/// Responds to deleted messages, deleting persistent pagination data.
/// </summary>
internal sealed class MessageDeletedResponder : IResponder<IMessageDelete>, IResponder<IMessageDeleteBulk>
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDeletedResponder"/> class.
    /// </summary>
    /// <param name="cache">The memory cache.</param>
    public MessageDeletedResponder(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc />
    public Task<Result> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
    {
        var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(gatewayEvent.ID.ToString());
        _cache.Remove(cacheKey);

        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc />
    public Task<Result> RespondAsync(IMessageDeleteBulk gatewayEvent, CancellationToken ct = default)
    {
        foreach (var id in gatewayEvent.IDs)
        {
            var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(id.ToString());
            _cache.Remove(cacheKey);
        }

        return Task.FromResult(Result.FromSuccess());
    }
}
