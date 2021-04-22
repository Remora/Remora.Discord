//
//  CacheBuilder.cs
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

namespace Remora.Discord.Caching.Extensions
{
    /// <summary>
    /// Utility class used by <see cref="ServiceCollectionExtensions.AddDiscordCaching"/> to configure the cache.
    /// </summary>
    public class CacheBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBuilder"/> class.
        /// </summary>
        /// <param name="services">Services of the application.</param>
        public CacheBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Gets the application services.
        /// </summary>
        public IServiceCollection Services { get; }
    }
}
