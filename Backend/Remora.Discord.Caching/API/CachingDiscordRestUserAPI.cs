//
//  CachingDiscordRestUserAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered user API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestUserAPI : IDiscordRestUserAPI, IRestCustomizable
{
    private readonly IDiscordRestUserAPI _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestUserAPI"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestUserAPI
    (
        IDiscordRestUserAPI actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IUser>> GetUserAsync
    (
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateUserCacheKey(userID);
        var cacheResult = await _cacheService.TryGetValueAsync<IUser>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getUser = await _actual.GetUserAsync(userID, ct);
        if (!getUser.IsSuccess)
        {
            return getUser;
        }

        var user = getUser.Entity;
        await _cacheService.CacheAsync(key, user, ct);

        return getUser;
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> CreateDMAsync
    (
        Snowflake recipientID,
        CancellationToken ct = default)
    {
        var createDM = await _actual.CreateDMAsync(recipientID, ct);
        if (!createDM.IsSuccess)
        {
            return createDM;
        }

        var dm = createDM.Entity;
        var key = KeyHelpers.CreateChannelCacheKey(dm.ID);

        await _cacheService.CacheAsync(key, dm, ct);

        return createDM;
    }

    /// <inheritdoc />
    public async Task<Result<IUser>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var key = KeyHelpers.CreateCurrentUserCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IUser>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getUser = await _actual.GetCurrentUserAsync(ct);
        if (!getUser.IsSuccess)
        {
            return getUser;
        }

        var user = getUser.Entity;
        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);

        // Cache this as both a normal user and our current user
        await _cacheService.CacheAsync(key, user, ct);
        await _cacheService.CacheAsync(userKey, user, ct);

        return getUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IConnection>>> GetUserConnectionsAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentUserConnectionsCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IConnection>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getUserConnections = await _actual.GetUserConnectionsAsync(ct);
        if (!getUserConnections.IsSuccess)
        {
            return getUserConnections;
        }

        var connections = getUserConnections.Entity;
        await _cacheService.CacheAsync(key, connections, ct);

        foreach (var connection in connections)
        {
            var connectionKey = KeyHelpers.CreateConnectionCacheKey(connection.ID);
            await _cacheService.CacheAsync(connectionKey, connection, ct);
        }

        return getUserConnections;
    }

    /// <inheritdoc />
    public async Task<Result<IUser>> ModifyCurrentUserAsync
    (
        Optional<string> username,
        Optional<Stream?> avatar = default,
        CancellationToken ct = default
    )
    {
        var modifyUser = await _actual.ModifyCurrentUserAsync(username, avatar, ct);
        if (!modifyUser.IsSuccess)
        {
            return modifyUser;
        }

        var user = modifyUser.Entity;
        var key = KeyHelpers.CreateCurrentUserCacheKey();
        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);

        await _cacheService.CacheAsync(key, user, ct);
        await _cacheService.CacheAsync(userKey, user, ct);

        return modifyUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IChannel>>> GetUserDMsAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentUserDMsCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IChannel>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getUserDMs = await _actual.GetUserDMsAsync(ct);
        if (!getUserDMs.IsSuccess)
        {
            return getUserDMs;
        }

        var userDMs = getUserDMs.Entity;
        await _cacheService.CacheAsync(key, userDMs, ct);

        foreach (var dm in userDMs)
        {
            var channelKey = KeyHelpers.CreateChannelCacheKey(dm.ID);
            await _cacheService.CacheAsync(channelKey, dm, ct);
        }

        return getUserDMs;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildMember>> GetCurrentUserGuildMemberAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var result = await _actual.GetCurrentUserGuildMemberAsync(guildID, ct);
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
        await _cacheService.CacheAsync(key, member, ct);

        return result;
    }
}
