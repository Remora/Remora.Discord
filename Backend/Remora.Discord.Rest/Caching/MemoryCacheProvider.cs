//
//  MemoryCacheProvider.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Results;

namespace Remora.Discord.Rest.Caching;

/// <summary>
/// An <see cref="IMemoryCache"/>-backed cache provider.
/// </summary>
[PublicAPI]
public class MemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheProvider"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    public MemoryCacheProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <inheritdoc cref="ICacheProvider.CacheAsync{TInstance}" />
    public ValueTask CacheAsync<TInstance>
    (
        string key,
        TInstance instance,
        DateTimeOffset? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null,
        CancellationToken ct = default
    )
        where TInstance : class
    {
        var options = new MemoryCacheEntryOptions();

        if (absoluteExpiration.HasValue)
        {
            options.SetAbsoluteExpiration(absoluteExpiration.Value);
        }

        if (slidingExpiration.HasValue)
        {
            options.SetSlidingExpiration(slidingExpiration.Value);
        }

        _memoryCache.Set(key, instance, options);

        return default;
    }

    /// <inheritdoc cref="ICacheProvider.RetrieveAsync{TInstance}"/>
    public ValueTask<Result<TInstance>> RetrieveAsync<TInstance>(string key, CancellationToken ct = default)
        where TInstance : class
    {
        if (_memoryCache.TryGetValue<TInstance>(key, out var instance))
        {
            return new(instance);
        }

        return new(new NotFoundError($"The key \"{key}\" did not contain a value in cache."));
    }

    /// <inheritdoc />
    public ValueTask<Result> EvictAsync(string key, CancellationToken ct = default)
    {
        if (!_memoryCache.TryGetValue(key, out _))
        {
            return new(new NotFoundError($"The key \"{key}\" did not contain a value in cache."));
        }

        _memoryCache.Remove(key);
        return new(Result.FromSuccess());
    }

    /// <inheritdoc cref="ICacheProvider.EvictAsync{TInstance}"/>
    public ValueTask<Result<TInstance>> EvictAsync<TInstance>(string key, CancellationToken ct = default)
        where TInstance : class
    {
        if (!_memoryCache.TryGetValue(key, out TInstance existingValue))
        {
            return new(new NotFoundError($"The key \"{key}\" did not contain a value in cache."));
        }

        _memoryCache.Remove(key);
        return new(existingValue);
    }
}
