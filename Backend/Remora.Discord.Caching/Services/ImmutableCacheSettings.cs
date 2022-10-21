//
//  ImmutableCacheSettings.cs
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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Remora.Discord.Caching.Abstractions;

namespace Remora.Discord.Caching.Services;

/// <summary>
/// Allows for accessing settings for cache objects.
/// </summary>
public class ImmutableCacheSettings
{
    private readonly Dictionary<Type, CacheEntryOptions> _cacheEntryOptions = new();
    private readonly Dictionary<Type, CacheEntryOptions> _evictionCacheEntryOptions = new();
    private readonly CacheEntryOptions _defaultCacheEntryOptions;
    private readonly CacheEntryOptions _defaultEvictionCacheEntryOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableCacheSettings"/> class.
    /// </summary>
    /// <param name="backingSettingsOptions">
    /// The backing <see cref="CacheSettings"/> options that will be used to construct this instance.
    /// </param>
    public ImmutableCacheSettings(IOptions<CacheSettings> backingSettingsOptions)
    {
        var settings = backingSettingsOptions.Value;

        _defaultCacheEntryOptions = new CacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = settings.DefaultAbsoluteExpiration,
            SlidingExpiration = settings.DefaultSlidingExpiration
        };

        _defaultEvictionCacheEntryOptions = new CacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = settings.DefaultEvictionAbsoluteExpiration,
            SlidingExpiration = settings.DefaultEvictionSlidingExpiration
        };

        foreach (var type in settings.ConfiguredTypes)
        {
            _cacheEntryOptions[type] = new CacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    settings.AbsoluteCacheExpirations.TryGetValue(type, out var absoluteExpiration)
                        ? absoluteExpiration
                        : settings.DefaultAbsoluteExpiration,

                SlidingExpiration =
                    settings.SlidingCacheExpirations.TryGetValue(type, out var slidingExpiration)
                        ? slidingExpiration
                        : settings.DefaultSlidingExpiration,
            };
        }

        foreach (var type in settings.ConfiguredEvictionTypes)
        {
            _evictionCacheEntryOptions[type] = new CacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    settings.AbsoluteEvictionCacheExpirations.TryGetValue(type, out var absoluteExpiration)
                        ? absoluteExpiration
                        : settings.DefaultEvictionAbsoluteExpiration,

                SlidingExpiration =
                    settings.SlidingEvictionCacheExpirations.TryGetValue(type, out var slidingExpiration)
                        ? slidingExpiration
                        : settings.DefaultEvictionSlidingExpiration,
            };
        }
    }

    /// <summary>
    /// Gets a set of cache options, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    public CacheEntryOptions GetEntryOptions<T>()
    {
        return _cacheEntryOptions.TryGetValue(typeof(T), out var cacheEntryOptions)
            ? cacheEntryOptions
            : _defaultCacheEntryOptions;
    }

    /// <summary>
    /// Gets a set of cache options for an evicted value, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    public CacheEntryOptions GetEvictionEntryOptions<T>()
    {
        return _evictionCacheEntryOptions.TryGetValue(typeof(T), out var cacheEntryOptions)
            ? cacheEntryOptions
            : _defaultEvictionCacheEntryOptions;
    }
}
