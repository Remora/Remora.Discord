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

using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.Caching.Abstractions;
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
    private readonly string? _tokenHash;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheProvider"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    /// <param name="tokenStore">The token store, if one is available.</param>
    public MemoryCacheProvider(IMemoryCache memoryCache, ITokenStore? tokenStore = null)
    {
        _memoryCache = memoryCache;

        if (tokenStore is null)
        {
            _tokenHash = null;
            return;
        }

        using var hasher = SHA256.Create();
        var hashBuilder = new StringBuilder(64);
        var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(tokenStore.Token));

        foreach (var value in hash)
        {
            hashBuilder.Append(value.ToString("x2"));
        }

        _tokenHash = hashBuilder.ToString();
    }

    /// <inheritdoc cref="ICacheProvider.CacheAsync{TInstance}" />
    public ValueTask CacheAsync<TInstance>
    (
        CacheKey key,
        TInstance instance,
        CacheEntryOptions options,
        CancellationToken ct = default
    )
        where TInstance : class
    {
        _memoryCache.Set(CreateTokenScopedKey(key), instance, options);

        return default;
    }

    /// <inheritdoc cref="ICacheProvider.RetrieveAsync{TInstance}"/>
    public ValueTask<Result<TInstance>> RetrieveAsync<TInstance>(CacheKey key, CancellationToken ct = default)
        where TInstance : class
    {
        if (_memoryCache.TryGetValue<TInstance>(CreateTokenScopedKey(key), out var instance))
        {
            return new(instance);
        }

        return new(new NotFoundError($"The key \"{key}\" did not contain a value in cache."));
    }

    /// <inheritdoc cref="ICacheProvider.EvictAsync" />
    public ValueTask<Result> EvictAsync(CacheKey key, CancellationToken ct = default)
    {
        if (!_memoryCache.TryGetValue(CreateTokenScopedKey(key), out _))
        {
            return new(new NotFoundError($"The key \"{key}\" did not contain a value in cache."));
        }

        _memoryCache.Remove(CreateTokenScopedKey(key));
        return new(Result.FromSuccess());
    }

    /// <inheritdoc cref="ICacheProvider.EvictAsync{TInstance}"/>
    public ValueTask<Result<TInstance>> EvictAsync<TInstance>(CacheKey key, CancellationToken ct = default)
        where TInstance : class
    {
        if (!_memoryCache.TryGetValue(CreateTokenScopedKey(key), out TInstance? existingValue))
        {
            return new(new NotFoundError($"The key \"{key}\" did not contain a value in cache."));
        }

        _memoryCache.Remove(CreateTokenScopedKey(key));
        return new(existingValue);
    }

    /// <summary>
    /// Creates a cache key scoped to a specific token.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The scoped key.</returns>
    private object CreateTokenScopedKey(CacheKey key) => (_tokenHash, key);
}
