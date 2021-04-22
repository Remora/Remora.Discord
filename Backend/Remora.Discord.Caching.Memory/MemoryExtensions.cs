//
//  MemoryExtensions.cs
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
    public static class MemoryExtensions
    {
        /// <summary>
        /// Adds the memory cache client.
        /// </summary>
        /// <param name="builder">The cache builder.</param>
        /// <returns>Same instance of the builder.</returns>
        public static CacheBuilder UseMemory(this CacheBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheClient, MemoryCacheClient>();
            return builder;
        }
    }
}
