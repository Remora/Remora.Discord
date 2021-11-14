//
//  EarlyCacheResponder.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.Responders
{
    /// <summary>
    /// Caches incoming data from the gateway.
    /// </summary>
    [PublicAPI]
    public class EarlyCacheResponder :
        IResponder<IChannelCreate>,
        IResponder<IChannelUpdate>,
        IResponder<IGuildBanAdd>,
        IResponder<IGuildCreate>,
        IResponder<IGuildEmojisUpdate>,
        IResponder<IGuildMemberAdd>,
        IResponder<IGuildMembersChunk>,
        IResponder<IGuildMemberUpdate>,
        IResponder<IGuildRoleCreate>,
        IResponder<IGuildRoleUpdate>,
        IResponder<IMessageCreate>,
        IResponder<IMessageReactionAdd>,
        IResponder<IUserUpdate>,
        IResponder<IInteractionCreate>
    {
        private readonly CacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyCacheResponder"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        public EarlyCacheResponder(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IChannelCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IChannel>(key, gatewayEvent);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IChannelUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateChannelCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IChannel>(key, gatewayEvent);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildBanAdd gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildBanCacheKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
            _cacheService.Cache<IBan>(key, new Ban(null, gatewayEvent.User));

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IGuild>(key, gatewayEvent);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildEmojisUpdate gatewayEvent, CancellationToken ct = default)
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

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.User.IsDefined(out var user))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, user.ID);
            _cacheService.Cache<IGuildMember>(key, gatewayEvent);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildMembersChunk gatewayEvent, CancellationToken ct = default)
        {
            foreach (var member in gatewayEvent.Members)
            {
                if (!member.User.IsDefined(out var user))
                {
                    continue;
                }

                var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, user.ID);
                _cacheService.Cache(key, member);
            }

            if (!gatewayEvent.Presences.IsDefined(out var presences))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            foreach (var presence in presences)
            {
                if (!presence.User.ID.IsDefined(out var userID))
                {
                    continue;
                }

                var key = KeyHelpers.CreatePresenceCacheKey(gatewayEvent.GuildID, userID);
                _cacheService.Cache(key, presence);
            }

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildMemberUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.ID);

            // Since this event isn't playing nice, we'll have to update by creating an object of our own.
            if (_cacheService.TryGetValue<IGuildMember>(key, out var cachedInstance))
            {
                cachedInstance = new GuildMember
                (
                    new Optional<IUser>(gatewayEvent.User),
                    gatewayEvent.Nickname.IsDefined(out var nickname) ? nickname : cachedInstance.Nickname,
                    gatewayEvent.Avatar,
                    gatewayEvent.Roles,
                    gatewayEvent.JoinedAt ?? cachedInstance.JoinedAt,
                    gatewayEvent.PremiumSince.IsDefined(out var premiumSince) ? premiumSince : cachedInstance.PremiumSince,
                    gatewayEvent.IsDeafened.IsDefined(out var isDeafened) ? isDeafened : cachedInstance.IsDeafened,
                    gatewayEvent.IsMuted.IsDefined(out var isMuted) ? isMuted : cachedInstance.IsMuted,
                    gatewayEvent.IsPending.IsDefined(out var isPending) ? isPending : cachedInstance.IsPending,
                    cachedInstance.Permissions
                );
            }
            else if (gatewayEvent.JoinedAt.HasValue)
            {
                cachedInstance = new GuildMember
                (
                    new Optional<IUser>(gatewayEvent.User),
                    gatewayEvent.Nickname,
                    gatewayEvent.Avatar,
                    gatewayEvent.Roles,
                    gatewayEvent.JoinedAt.Value,
                    gatewayEvent.PremiumSince,
                    gatewayEvent.IsDeafened.IsDefined(out var isDeafened) && isDeafened,
                    gatewayEvent.IsMuted.IsDefined(out var isMuted) && isMuted,
                    gatewayEvent.IsPending.IsDefined(out var isPending) && isPending
                );
            }
            else
            {
                return Task.FromResult(Result.FromSuccess());
            }

            _cacheService.Cache(key, cachedInstance);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildRoleCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.Role.ID);
            _cacheService.Cache(key, gatewayEvent.Role);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IGuildRoleUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.Role.ID);
            _cacheService.Cache(key, gatewayEvent.Role);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateMessageCacheKey(gatewayEvent.ChannelID, gatewayEvent.ID);
            _cacheService.Cache<IMessage>(key, gatewayEvent);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IMessageReactionAdd gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.GuildID.IsDefined(out var guildID))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            if (!gatewayEvent.Member.IsDefined(out var member))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            if (!member.User.IsDefined(out var user))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            var key = KeyHelpers.CreateGuildMemberKey(guildID, user.ID);
            _cacheService.Cache(key, member);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(IUserUpdate gatewayEvent, CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateUserCacheKey(gatewayEvent.ID);
            _cacheService.Cache<IUser>(key, gatewayEvent);

            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc />
        public Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.Data.IsDefined(out var data))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            if (!data.Resolved.IsDefined(out var resolved))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            if (resolved.Users.IsDefined(out var users))
            {
                foreach (var (key, value) in users)
                {
                    var cacheKey = KeyHelpers.CreateUserCacheKey(key);
                    _cacheService.Cache(cacheKey, value);
                }
            }

            if (!resolved.Roles.IsDefined(out var roles) || !gatewayEvent.GuildID.IsDefined(out var guildID))
            {
                return Task.FromResult(Result.FromSuccess());
            }

            foreach (var (key, value) in roles)
            {
                var cacheKey = KeyHelpers.CreateGuildRoleCacheKey(guildID, key);
                _cacheService.Cache(cacheKey, value);
            }

            return Task.FromResult(Result.FromSuccess());
        }
    }
}
