//
//  CachingDiscordRestRestOAuth2API.cs
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
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered OAuth2 API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestOAuth2API : IDiscordRestOAuth2API, IRestCustomizable
{
    private readonly IDiscordRestOAuth2API _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestOAuth2API"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestOAuth2API
    (
        IDiscordRestOAuth2API actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IApplication>> GetCurrentBotApplicationInformationAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentApplicationCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IApplication>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getCurrent = await _actual.GetCurrentBotApplicationInformationAsync(ct);
        if (!getCurrent.IsSuccess)
        {
            return getCurrent;
        }

        var application = getCurrent.Entity;
        await _cacheService.CacheAsync(key, application, ct);

        return getCurrent;
    }

    /// <inheritdoc />
    public async Task<Result<IAuthorizationInformation>> GetCurrentAuthorizationInformationAsync
    (
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateCurrentAuthorizationInformationCacheKey();
        var cacheResult = await _cacheService.TryGetValueAsync<IAuthorizationInformation>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var result = await _actual.GetCurrentAuthorizationInformationAsync(ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _cacheService.CacheAsync(key, result.Entity, ct);

        return result;
    }
}
