//
//  RedisCacheClient.cs
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

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remora.Discord.Caching.Services;
using StackExchange.Redis;

namespace Remora.Discord.Caching
{
    /// <summary>
    /// Stores the cache into Redis.
    /// </summary>
    public class RedisCacheClient : ICacheClient, IDisposable
    {
        private readonly IOptions<RedisOptions> _options;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly CacheSettings _cacheSettings;
        private IConnectionMultiplexer? _connection;
        private IDatabase? _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheClient"/> class.
        /// </summary>
        /// <param name="jsonOptions">The JSON options.</param>
        /// <param name="options">The Redis client options.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        public RedisCacheClient(IOptions<JsonSerializerOptions> jsonOptions, IOptions<RedisOptions> options, IOptions<CacheSettings> cacheSettings)
        {
            _options = options;
            _jsonOptions = jsonOptions.Value;
            _cacheSettings = cacheSettings.Value;
        }

        /// <inheritdoc />
        public async Task StoreAsync<TInstance>(CacheKey key, TInstance value)
        {
            var expirationSetting = _cacheSettings.GetAbsoluteExpirationOrDefault<TInstance>();
            var slidingSetting = _cacheSettings.GetSlidingExpirationOrDefault<TInstance>();

            if (expirationSetting <= TimeSpan.Zero)
            {
                return;
            }

            TimeSpan? expiration;
            DateTimeOffset? startSlidingAt;

            if (expirationSetting != TimeSpan.MaxValue)
            {
                expiration = expirationSetting;
                startSlidingAt = DateTimeOffset.Now.Add(expirationSetting).Subtract(slidingSetting);
            }
            else
            {
                expiration = null;
                startSlidingAt = null;
            }

            var cachedValue = new CachedValue<TInstance>(value, startSlidingAt, (int)slidingSetting.TotalSeconds);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(cachedValue, _jsonOptions);
            var database = await GetDatabaseAsync();

            await database.StringSetAsync(key.Key, bytes, expiration);
        }

        /// <inheritdoc />
        public async Task<CacheResult<TInstance>> RetrieveAsync<TInstance>(CacheKey key) where TInstance : notnull
        {
            var database = await GetDatabaseAsync();
            var cacheValue = await database.StringGetAsync(key.Key);

            if (!cacheValue.HasValue)
            {
                return default;
            }

            var data = (ReadOnlyMemory<byte>)cacheValue;
            var (instance, startSlidingAt, slidingExpiration) = JsonSerializer.Deserialize<CachedValue<TInstance>>(data.Span, _jsonOptions)!;

            if (startSlidingAt.HasValue && DateTime.UtcNow >= startSlidingAt)
            {
                await database.KeyExpireAsync(key.Key, TimeSpan.FromSeconds(slidingExpiration));
            }

            return instance;
        }

        /// <inheritdoc />
        public async Task EvictAsync(CacheKey key)
        {
            var database = await GetDatabaseAsync();

            await database.KeyDeleteAsync(key.Key);
        }

        private async ValueTask<IDatabase> GetDatabaseAsync()
        {
            if (_database != null)
            {
                return _database;
            }

            if (string.IsNullOrEmpty(_options.Value.ConnectionString))
            {
                throw new InvalidOperationException("No configuration string is provided");
            }

            _connection = await ConnectionMultiplexer.ConnectAsync(ConfigurationOptions.Parse(_options.Value.ConnectionString));
            _database = _connection.GetDatabase();

            return _database;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _connection?.Dispose();
        }
    }
}
