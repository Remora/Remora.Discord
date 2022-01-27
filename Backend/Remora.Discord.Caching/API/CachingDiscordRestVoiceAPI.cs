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

    /// <inheritdoc cref="DiscordRestVoiceAPI(IRestHttpClient, JsonSerializerOptions)" />
    public CachingDiscordRestVoiceAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        CacheService cacheService
    )
        : base(restHttpClient, jsonOptions)
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
        if (_cacheService.TryGetValue<IReadOnlyList<IVoiceRegion>>(key, out var cachedInstance))
        {
            return Result<IReadOnlyList<IVoiceRegion>>.FromSuccess(cachedInstance);
        }

        var listRegions = await base.ListVoiceRegionsAsync(ct);
        if (!listRegions.IsSuccess)
        {
            return listRegions;
        }

        var regions = listRegions.Entity;
        _cacheService.Cache(key, regions);

        foreach (var voiceRegion in regions)
        {
            var regionKey = KeyHelpers.CreateVoiceRegionCacheKey(voiceRegion.ID);
            _cacheService.Cache(regionKey, voiceRegion);
        }

        return listRegions;
    }
}
