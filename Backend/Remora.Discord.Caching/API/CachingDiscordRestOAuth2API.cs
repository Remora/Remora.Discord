//
//  CachingDiscordRestOAuth2API.cs
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

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <inheritdoc />
[PublicAPI]
public class CachingDiscordRestOAuth2API : DiscordRestOAuth2API
{
    private readonly ICacheService _cacheService;

    /// <inheritdoc cref="DiscordRestOAuth2API(IRestHttpClient, JsonSerializerOptions)" />
    public CachingDiscordRestOAuth2API
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheService cacheService
    )
        : base(restHttpClient, jsonOptions)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public override async Task<Result<IApplication>> GetCurrentBotApplicationInformationAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentApplicationCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IApplication>(key);

        if (cacheResult.IsSuccess)
        {
            return Result<IApplication>.FromSuccess(cacheResult.Entity);
        }

        var getCurrent = await base.GetCurrentBotApplicationInformationAsync(ct);
        if (!getCurrent.IsSuccess)
        {
            return getCurrent;
        }

        var application = getCurrent.Entity;
        await _cacheService.CacheAsync(key, application);

        return getCurrent;
    }

    /// <inheritdoc />
    public override async Task<Result<IAuthorizationInformation>> GetCurrentAuthorizationInformationAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentAuthorizationInformationCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IAuthorizationInformation>(key);

        if (cacheResult.IsSuccess)
        {
            return Result<IAuthorizationInformation>.FromSuccess(cacheResult.Entity);
        }

        var result = await base.GetCurrentAuthorizationInformationAsync(ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _cacheService.CacheAsync(key, result.Entity);

        return result;
    }
}
