//
//  LateCacheResponder.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Caching.Responders;

/// <summary>
/// Evicts explicitly deleted data from the cache.
/// </summary>
[PublicAPI]
public class LateCacheResponder :
    IResponder<IChannelDelete>,
    IResponder<IGuildBanRemove>,
    IResponder<IGuildDelete>,
    IResponder<IGuildMemberRemove>,
    IResponder<IGuildRoleDelete>,
    IResponder<IInviteDelete>,
    IResponder<IMessageDelete>,
    IResponder<IMessageDeleteBulk>
{
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LateCacheResponder"/> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    public LateCacheResponder(CacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IChannelDelete gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
        await _cacheService.EvictAsync<IChannel>(key, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildBanRemove gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateGuildBanCacheKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
        await _cacheService.EvictAsync<IBan>(key, ct).AsTask();

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildDelete gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateGuildCacheKey(gatewayEvent.ID);
        await _cacheService.EvictAsync<IGuild>(key, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
        await _cacheService.EvictAsync<IGuildMember>(key, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildRoleDelete gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.RoleID);
        await _cacheService.EvictAsync<IRole>(key, ct);

        var collectionKey = KeyHelpers.CreateGuildRolesCacheKey(gatewayEvent.GuildID);
        var getCachedList = await _cacheService.TryGetValueAsync<IReadOnlyList<IRole>>(collectionKey, ct);
        if (getCachedList.IsSuccess)
        {
            var oldList = getCachedList.Entity;

            var newList = oldList.ToList();
            var existingRoleIndex = newList.FindIndex(r => r.ID == gatewayEvent.RoleID);
            if (existingRoleIndex > -1)
            {
                // It's in the list; remove it
                newList.RemoveAt(existingRoleIndex);
            }
            else
            {
                // already gone
                return Result.FromSuccess();
            }

            await _cacheService.CacheAsync(collectionKey, newList, ct);
        }
        else if (getCachedList.Error is not NotFoundError)
        {
            // Some other problem; NotFound is fine
            return (Result)getCachedList;
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IInviteDelete gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateInviteCacheKey(gatewayEvent.Code);
        await _cacheService.EvictAsync<IInvite>(key, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, gatewayEvent.ID);
        await _cacheService.EvictAsync<IMessage>(key, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IMessageDeleteBulk gatewayEvent, CancellationToken ct = default)
    {
        foreach (var messageID in gatewayEvent.IDs)
        {
            var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, messageID);
            await _cacheService.EvictAsync<IMessage>(key, ct);
        }

        return Result.FromSuccess();
    }
}
