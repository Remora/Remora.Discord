//
//  LateCacheResponder.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Caching.Responders
{
    /// <summary>
    /// Evicts explicitly deleted data from the cache.
    /// </summary>
    [UsedImplicitly]
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
        public Task<Result> RespondAsync(IChannelDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildBanRemove gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildBanCacheKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildCacheKey(gatewayEvent.GuildID);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildRoleDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.RoleID);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IInviteDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateInviteCacheKey(gatewayEvent.Code);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, gatewayEvent.ID);
            _cacheService.Evict(key);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IMessageDeleteBulk gatewayEvent, CancellationToken ct = default)
        {
            foreach (var messageID in gatewayEvent.IDs)
            {
                var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, messageID);
                _cacheService.Evict(key);
            }

            return Task.FromResult(Result.FromSuccess());
        }
    }
}
