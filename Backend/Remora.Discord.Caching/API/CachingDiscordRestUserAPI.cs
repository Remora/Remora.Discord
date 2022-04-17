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
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <inheritdoc />
[PublicAPI]
public class CachingDiscordRestUserAPI : DiscordRestUserAPI
{
    private readonly CacheService _cacheService;

    /// <inheritdoc cref="DiscordRestUserAPI(IRestHttpClient, JsonSerializerOptions, IMemoryCache)" />
    public CachingDiscordRestUserAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        IMemoryCache rateLimitCache,
        CacheService cacheService
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
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
        var cacheResult = await _cacheService.TryGetValueAsync<IUser>(key);

        if (cacheResult.IsSuccess)
        {
            return Result<IUser>.FromSuccess(cacheResult.Entity);
        }

        var getUser = await base.GetUserAsync(userID, ct);
        if (!getUser.IsSuccess)
        {
            return getUser;
        }

        var user = getUser.Entity;
        await _cacheService.CacheAsync(key, user);

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

        await _cacheService.CacheAsync(key, dm);

        return createDM;
    }

    /// <inheritdoc />
    public override async Task<Result<IUser>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateCurrentUserCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IUser>(key);

        if (cacheResult.IsSuccess)
        {
            return Result<IUser>.FromSuccess(cacheResult.Entity);
        }

        var getUser = await base.GetCurrentUserAsync(ct);
        if (!getUser.IsSuccess)
        {
            return getUser;
        }

        var user = getUser.Entity;
        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);

        // Cache this as both a normal user and our current user
        await _cacheService.CacheAsync(key, user);
        await _cacheService.CacheAsync(userKey, user);

        return getUser;
    }

    /// <inheritdoc />
    public override async Task<Result<IReadOnlyList<IConnection>>> GetUserConnectionsAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentUserConnectionsCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IConnection>>(key);

        if (cacheResult.IsSuccess)
        {
            return Result<IReadOnlyList<IConnection>>.FromSuccess(cacheResult.Entity);
        }

        var getUserConnections = await base.GetUserConnectionsAsync(ct);
        if (!getUserConnections.IsSuccess)
        {
            return getUserConnections;
        }

        var connections = getUserConnections.Entity;
        await _cacheService.CacheAsync(key, connections);

        foreach (var connection in connections)
        {
            var connectionKey = KeyHelpers.CreateConnectionCacheKey(connection.ID);
            await _cacheService.CacheAsync(connectionKey, connection);
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

        await _cacheService.CacheAsync(key, user);
        await _cacheService.CacheAsync(userKey, user);

        return modifyUser;
    }

    /// <inheritdoc />
    public override async Task<Result<IReadOnlyList<IChannel>>> GetUserDMsAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentUserDMsCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IChannel>>(key);

        if (cacheResult.IsSuccess)
        {
            return Result<IReadOnlyList<IChannel>>.FromSuccess(cacheResult.Entity);
        }

        var getUserDMs = await base.GetUserDMsAsync(ct);
        if (!getUserDMs.IsSuccess)
        {
            return getUserDMs;
        }

        var userDMs = getUserDMs.Entity;
        await _cacheService.CacheAsync(key, userDMs);

        foreach (var dm in userDMs)
        {
            var channelKey = KeyHelpers.CreateChannelCacheKey(dm.ID);
            await _cacheService.CacheAsync(channelKey, dm);
        }

        return getUserDMs;
    }

    /// <inheritdoc />
    public override async Task<Result<IGuildMember>> GetCurrentUserGuildMemberAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var result = await base.GetCurrentUserGuildMemberAsync(guildID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        var member = result.Entity;
        if (!member.User.IsDefined(out var user))
        {
            return result;
        }

        var key = KeyHelpers.CreateGuildMemberKey(guildID, user.ID);
        await _cacheService.CacheAsync(key, member);

        return result;
    }
}
