//
//  CacheResponder.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;

namespace Remora.Discord.Caching.Responders
{
    /// <summary>
    /// Caches incoming data from the gateway.
    /// </summary>
    public class CacheResponder :
        IResponder<IChannelCreate>,
        IResponder<IChannelDelete>,
        IResponder<IChannelUpdate>,
        IResponder<IGuildBanAdd>,
        IResponder<IGuildBanRemove>,
        IResponder<IGuildCreate>,
        IResponder<IGuildDelete>,
        IResponder<IGuildEmojisUpdate>,
        IResponder<IGuildMemberAdd>,
        IResponder<IGuildMemberRemove>,
        IResponder<IGuildMembersChunk>,
        IResponder<IGuildMemberUpdate>,
        IResponder<IGuildRoleCreate>,
        IResponder<IGuildRoleDelete>,
        IResponder<IGuildRoleUpdate>,
        IResponder<IInviteDelete>,
        IResponder<IMessageCreate>,
        IResponder<IMessageDelete>,
        IResponder<IMessageDeleteBulk>,
        IResponder<IMessageReactionAdd>,
        IResponder<IUserUpdate>
    {
        private readonly CacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheResponder"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        public CacheResponder(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IChannelCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IChannel>(key, gatewayEvent);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IChannelDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IChannelUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IChannel>(key, gatewayEvent);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildBanAdd gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildBanCacheKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
            _cacheService.Cache<IBan>(key, new Ban(null, gatewayEvent.User));

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildBanRemove gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildBanCacheKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IGuild>(key, gatewayEvent);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildCacheKey(gatewayEvent.GuildID);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildEmojisUpdate gatewayEvent, CancellationToken ct = default)
        {
            foreach (var emoji in gatewayEvent.Emojis)
            {
                if (emoji.ID is null)
                {
                    continue;
                }

                var key = KeyHelpers.CreateEmojiCacheKey(gatewayEvent.GuildID, emoji.ID.Value);
                _cacheService.Cache(key, emoji);
            }

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.User.HasValue)
            {
                return Task.FromResult(EventResponseResult.FromSuccess());
            }

            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.Value!.ID);
            _cacheService.Cache<IGuildMember>(key, gatewayEvent);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildMembersChunk gatewayEvent, CancellationToken ct = default)
        {
            foreach (var member in gatewayEvent.Members)
            {
                if (!member.User.HasValue)
                {
                    continue;
                }

                var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, member.User.Value!.ID);
                _cacheService.Cache(key, member);
            }

            if (!gatewayEvent.Presences.HasValue)
            {
                return Task.FromResult(EventResponseResult.FromSuccess());
            }

            foreach (var presence in gatewayEvent.Presences.Value!)
            {
                if (!presence.User.ID.HasValue)
                {
                    continue;
                }

                var key = KeyHelpers.CreatePresenceCacheKey(gatewayEvent.GuildID, presence.User.ID.Value);
                _cacheService.Cache(key, presence);
            }

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildMemberUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.ID);

            // Since this event isn't playing nice, we'll have to update by creating an object of our own.
            if (_cacheService.TryGetValue<IGuildMember>(key, out var cachedInstance))
            {
                cachedInstance = new GuildMember
                (
                    new Optional<IUser>(gatewayEvent.User),
                    gatewayEvent.Nickname.HasValue ? gatewayEvent.Nickname : cachedInstance.Nickname,
                    gatewayEvent.Roles,
                    gatewayEvent.JoinedAt,
                    gatewayEvent.PremiumSince.HasValue ? gatewayEvent.PremiumSince.Value : cachedInstance.PremiumSince,
                    cachedInstance.IsDeafened,
                    cachedInstance.IsMuted,
                    cachedInstance.IsPending,
                    cachedInstance.Permissions
                );
            }
            else if (gatewayEvent.PremiumSince.HasValue)
            {
                cachedInstance = new GuildMember
                (
                    new Optional<IUser>(gatewayEvent.User),
                    gatewayEvent.Nickname,
                    gatewayEvent.Roles,
                    gatewayEvent.JoinedAt,
                    gatewayEvent.PremiumSince.Value,
                    false,
                    false,
                    default,
                    default
                );
            }
            else
            {
                return Task.FromResult(EventResponseResult.FromSuccess());
            }

            _cacheService.Cache(key, cachedInstance);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildRoleCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.Role.ID);
            _cacheService.Cache(key, gatewayEvent.Role);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildRoleDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.RoleID);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IGuildRoleUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.Role.ID);
            _cacheService.Cache(key, gatewayEvent.Role);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IInviteDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateInviteCacheKey(gatewayEvent.Code);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, gatewayEvent.ID);
            _cacheService.Cache<IMessage>(key, gatewayEvent);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, gatewayEvent.ID);
            _cacheService.Evict(key);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IMessageDeleteBulk gatewayEvent, CancellationToken ct = default)
        {
            foreach (var messageID in gatewayEvent.MessageIDs)
            {
                var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, messageID);
                _cacheService.Evict(key);
            }

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IMessageReactionAdd gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.GuildID.HasValue)
            {
                return Task.FromResult(EventResponseResult.FromSuccess());
            }

            if (!gatewayEvent.Member.HasValue)
            {
                return Task.FromResult(EventResponseResult.FromSuccess());
            }

            var member = gatewayEvent.Member.Value;
            if (!member!.User.HasValue)
            {
                return Task.FromResult(EventResponseResult.FromSuccess());
            }

            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID.Value, member.User.Value!.ID);
            _cacheService.Cache(key, member);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<EventResponseResult> RespondAsync(IUserUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateUserCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IUser>(key, gatewayEvent);

            return Task.FromResult(EventResponseResult.FromSuccess());
        }
    }
}
