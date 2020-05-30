//
//  DiscordJsonService.cs
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

using Newtonsoft.Json;
using Remora.Discord.Gateway.API.Json.ContractResolvers;
using Remora.Discord.Gateway.API.Json.Converters;

namespace Remora.Discord.Gateway.Services
{
    /// <summary>
    /// Business logic for Discord's JSON de/serialization.
    /// </summary>
    public class DiscordJsonService
    {
        /// <summary>
        /// Gets the serializes in use by the service.
        /// </summary>
        public JsonSerializer Serializer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordJsonService"/> class.
        /// </summary>
        public DiscordJsonService()
        {
            this.Serializer = new JsonSerializer
            {
                ContractResolver = new DiscordContractResolver()
            };

            this.Serializer.Converters.Add(new PartySizeConverter());
            this.Serializer.Converters.Add(new HeartbeatConverter());
            this.Serializer.Converters.Add(new ShardIdentificationConverter());
            this.Serializer.Converters.Add(new SnowflakeConverter());
            this.Serializer.Converters.Add(new PayloadCreationConverter());
        }
    }
}
