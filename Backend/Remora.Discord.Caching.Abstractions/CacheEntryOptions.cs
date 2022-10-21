//
//  CacheEntryOptions.cs
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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Remora.Discord.Caching.Abstractions;

/// <summary>
/// Wrapper type for options for entries in <see cref="IMemoryCache"/> or <see cref="IDistributedCache"/> instances.
/// Implicitly convertible to <see cref="MemoryCacheEntryOptions"/> and/or <see cref="DistributedCacheEntryOptions"/>.
/// </summary>
/// <seealso cref="MemoryCacheEntryOptions"/>
/// <seealso cref="DistributedCacheEntryOptions"/>
public sealed class CacheEntryOptions
{
    /// <summary>
    /// Gets the absolute expiration time.
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; init; }

    /// <summary>
    /// Gets the absolute expiration time, relative to now.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }

    /// <summary>
    /// Gets the sliding expiration time.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; init; }

    private MemoryCacheEntryOptions? _memoryCacheOptions;
    private DistributedCacheEntryOptions? _distributedCacheOptions;

    /// <summary>
    /// Gets the cached <see cref="MemoryCacheEntryOptions"/> for this instance, or creates a new one and caches it.
    /// </summary>
    /// <param name="self">The current <see cref="CacheEntryOptions"/> instance.</param>
    /// <returns>
    /// An instance of <see cref="MemoryCacheEntryOptions"/> with settings corresponding to the current instance.
    /// </returns>
    public static implicit operator MemoryCacheEntryOptions(CacheEntryOptions self)
    {
        return self._memoryCacheOptions ??= MakeOptions(self);

        static MemoryCacheEntryOptions MakeOptions(CacheEntryOptions self)
        {
            var options = new MemoryCacheEntryOptions();

            if (self.AbsoluteExpiration.HasValue)
            {
                options.SetAbsoluteExpiration(self.AbsoluteExpiration.Value);
            }

            if (self.AbsoluteExpirationRelativeToNow.HasValue)
            {
                options.SetAbsoluteExpiration(self.AbsoluteExpirationRelativeToNow.Value);
            }

            if (self.SlidingExpiration.HasValue)
            {
                options.SetSlidingExpiration(self.SlidingExpiration.Value);
            }

            return options;
        }
    }

    /// <summary>
    /// Gets the cached <see cref="DistributedCacheEntryOptions"/> for this instance, or creates a new one and caches
    /// it.
    /// </summary>
    /// <param name="self">The current <see cref="CacheEntryOptions"/> instance.</param>
    /// <returns>
    /// An instance of <see cref="DistributedCacheEntryOptions"/> with settings corresponding to the current instance.
    /// </returns>
    public static implicit operator DistributedCacheEntryOptions(CacheEntryOptions self)
    {
        return self._distributedCacheOptions ??= MakeOptions(self);

        static DistributedCacheEntryOptions MakeOptions(CacheEntryOptions self)
        {
            var options = new DistributedCacheEntryOptions();

            if (self.AbsoluteExpiration.HasValue)
            {
                options.SetAbsoluteExpiration(self.AbsoluteExpiration.Value);
            }

            if (self.AbsoluteExpirationRelativeToNow.HasValue)
            {
                options.SetAbsoluteExpiration(self.AbsoluteExpirationRelativeToNow.Value);
            }

            if (self.SlidingExpiration.HasValue)
            {
                options.SetSlidingExpiration(self.SlidingExpiration.Value);
            }

            return options;
        }
    }
}
