//
//  CachingDiscordRestVoiceAPI.cs
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <inheritdoc />
[PublicAPI]
public class CachingDiscordRestVoiceAPI : DiscordRestVoiceAPI
{
    private readonly CacheService _cacheService;

    /// <inheritdoc cref="DiscordRestVoiceAPI(IRestHttpClient, JsonSerializerOptions, IMemoryCache)" />
    public CachingDiscordRestVoiceAPI
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
    public override async Task<Result<IReadOnlyList<IVoiceRegion>>> ListVoiceRegionsAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateVoiceRegionsCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IVoiceRegion>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return Result<IReadOnlyList<IVoiceRegion>>.FromSuccess(cacheResult.Entity);
        }

        var listRegions = await base.ListVoiceRegionsAsync(ct);
        if (!listRegions.IsSuccess)
        {
            return listRegions;
        }

        var regions = listRegions.Entity;
        await _cacheService.CacheAsync(key, regions, ct);

        foreach (var voiceRegion in regions)
        {
            var regionKey = KeyHelpers.CreateVoiceRegionCacheKey(voiceRegion.ID);
            await _cacheService.CacheAsync(regionKey, voiceRegion, ct);
        }

        return listRegions;
    }
}
