//
//  CachingDiscordRestInviteAPI.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered invite API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestInviteAPI : IDiscordRestInviteAPI, IRestCustomizable
{
    private readonly IDiscordRestInviteAPI _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestInviteAPI"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestInviteAPI
    (
        IDiscordRestInviteAPI actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IInvite>> GetInviteAsync
    (
        string inviteCode,
        Optional<bool> withCounts = default,
        Optional<bool> withExpiration = default,
        Optional<Snowflake> guildScheduledEventID = default,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateInviteCacheKey(inviteCode);
        var cacheResult = await _cacheService.TryGetValueAsync<IInvite>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getInvite = await _actual.GetInviteAsync
        (
            inviteCode,
            withCounts,
            withExpiration,
            guildScheduledEventID,
            ct
        );

        if (!getInvite.IsSuccess)
        {
            return getInvite;
        }

        var invite = getInvite.Entity;
        await _cacheService.CacheAsync(key, invite, ct);

        return getInvite;
    }

    /// <inheritdoc />
    public async Task<Result<IInvite>> DeleteInviteAsync
    (
        string inviteCode,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteInvite = await _actual.DeleteInviteAsync(inviteCode, reason, ct);
        if (!deleteInvite.IsSuccess)
        {
            return deleteInvite;
        }

        var key = KeyHelpers.CreateInviteCacheKey(inviteCode);
        await _cacheService.EvictAsync<IInvite>(key, ct);

        return deleteInvite;
    }
}
