//
//  CachingDiscordRestUserAPI.cs
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

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Core;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;
using Remora.Results;

namespace Remora.Discord.Caching.API
{
    /// <inheritdoc />
    public class CachingDiscordRestUserAPI : DiscordRestUserAPI
    {
        private readonly CacheService _cacheService;

        /// <inheritdoc cref="DiscordRestUserAPI(DiscordHttpClient, IOptions{JsonSerializerOptions})" />
        public CachingDiscordRestUserAPI
        (
            DiscordHttpClient discordHttpClient,
            IOptions<JsonSerializerOptions> jsonOptions,
            CacheService cacheService
        )
            : base(discordHttpClient, jsonOptions)
        {
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public override async Task<Result<IUser>> GetUserAsync
        (
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateUserCacheKey(userID);
            if (_cacheService.TryGetValue<IUser>(key, out var cachedInstance))
            {
                return Result<IUser>.FromSuccess(cachedInstance);
            }

            var getUser = await base.GetUserAsync(userID, ct);
            if (!getUser.IsSuccess)
            {
                return getUser;
            }

            var user = getUser.Entity;
            _cacheService.Cache(key, user);

            return getUser;
        }

        /// <inheritdoc />
        public override async Task<Result<IChannel>> CreateDMAsync
        (
            Snowflake recipientID,
            CancellationToken ct = default)
        {
            var createDM = await base.CreateDMAsync(recipientID, ct);
            if (!createDM.IsSuccess)
            {
                return createDM;
            }

            var dm = createDM.Entity;
            var key = KeyHelpers.CreateChannelCacheKey(dm.ID);

            _cacheService.Cache(key, dm);

            return createDM;
        }

        /// <inheritdoc />
        public override async Task<Result<IUser>> GetCurrentUserAsync(CancellationToken ct = default)
        {
            var key = KeyHelpers.CreateCurrentUserCacheKey();
            if (_cacheService.TryGetValue<IUser>(key, out var cachedInstance))
            {
                return Result<IUser>.FromSuccess(cachedInstance);
            }

            var getUser = await base.GetCurrentUserAsync(ct);
            if (!getUser.IsSuccess)
            {
                return getUser;
            }

            var user = getUser.Entity;
            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);

            // Cache this as both a normal user and our current user
            _cacheService.Cache(key, user);
            _cacheService.Cache(userKey, user);

            return getUser;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IConnection>>> GetUserConnectionsAsync
        (
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateCurrentUserConnectionsCacheKey();
            if (_cacheService.TryGetValue<IReadOnlyList<IConnection>>(key, out var cachedInstance))
            {
                return Result<IReadOnlyList<IConnection>>.FromSuccess(cachedInstance);
            }

            var getUserConnections = await base.GetUserConnectionsAsync(ct);
            if (!getUserConnections.IsSuccess)
            {
                return getUserConnections;
            }

            var connections = getUserConnections.Entity;
            _cacheService.Cache(key, connections);

            foreach (var connection in connections)
            {
                var connectionKey = KeyHelpers.CreateConnectionCacheKey(connection.ID);
                _cacheService.Cache(connectionKey, connection);
            }

            return getUserConnections;
        }

        /// <inheritdoc />
        public override async Task<Result<IUser>> ModifyCurrentUserAsync
        (
            Optional<string> username,
            Optional<Stream?> avatar = default,
            CancellationToken ct = default
        )
        {
            var modifyUser = await base.ModifyCurrentUserAsync(username, avatar, ct);
            if (!modifyUser.IsSuccess)
            {
                return modifyUser;
            }

            var user = modifyUser.Entity;
            var key = KeyHelpers.CreateCurrentUserCacheKey();
            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);

            _cacheService.Cache(key, user);
            _cacheService.Cache(userKey, user);

            return modifyUser;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IChannel>>> GetUserDMsAsync
        (
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateCurrentUserDMsCacheKey();
            if (_cacheService.TryGetValue<IReadOnlyList<IChannel>>(key, out var cachedInstance))
            {
                return Result<IReadOnlyList<IChannel>>.FromSuccess(cachedInstance);
            }

            var getUserDMs = await base.GetUserDMsAsync(ct);
            if (!getUserDMs.IsSuccess)
            {
                return getUserDMs;
            }

            var userDMs = getUserDMs.Entity;
            _cacheService.Cache(key, userDMs);

            foreach (var dm in userDMs)
            {
                var channelKey = KeyHelpers.CreateChannelCacheKey(dm.ID);
                _cacheService.Cache(channelKey, dm);
            }

            return getUserDMs;
        }
    }
}
