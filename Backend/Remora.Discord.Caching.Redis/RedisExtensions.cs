//
//  RedisExtensions.cs
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

using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Caching.Extensions;

namespace Remora.Discord.Caching
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class RedisExtensions
    {
        /// <summary>
        /// Adds the memory cache client.
        /// </summary>
        /// <param name="builder">The cache builder.</param>
        /// <param name="connectionString">The connection string of the Redis host.</param>
        /// <returns>Same instance of the builder.</returns>
        public static CacheBuilder UseRedis(this CacheBuilder builder, string? connectionString = null)
        {
            if (connectionString != null)
            {
                builder.Services.Configure<RedisOptions>(options => { options.ConnectionString = connectionString; });
            }

            builder.Services.AddSingleton<ICacheClient, RedisCacheClient>();
            return builder;
        }
    }
}
