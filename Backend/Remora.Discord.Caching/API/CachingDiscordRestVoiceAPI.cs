//
//  CachingDiscordRestVoiceAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Rest;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered voice API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestVoiceAPI : IDiscordRestVoiceAPI, IRestCustomizable
{
    private readonly IDiscordRestVoiceAPI _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestVoiceAPI"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestVoiceAPI
    (
        IDiscordRestVoiceAPI actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IVoiceRegion>>> ListVoiceRegionsAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateVoiceRegionsCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IVoiceRegion>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var listRegions = await _actual.ListVoiceRegionsAsync(ct);
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
