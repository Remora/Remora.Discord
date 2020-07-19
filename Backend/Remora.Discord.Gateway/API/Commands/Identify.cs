//
//  Identify.cs
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
using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents an identification command sent to the Discord gateway.
    /// </summary>
    public sealed class Identify : IIdentify
    {
        /// <inheritdoc />
        public string Token { get; }

        /// <inheritdoc />
        public IConnectionProperties Properties { get; }

        /// <inheritdoc />
        public Optional<bool> Compress { get; }

        /// <inheritdoc />
        public Optional<byte> LargeThreshold { get; }

        /// <inheritdoc />
        public Optional<IShardIdentification> Shard { get; }

        /// <inheritdoc />
        public Optional<IUpdateStatus> Presence { get; }

        /// <inheritdoc />
        [JsonProperty("guild_subscriptions")]
        public Optional<bool> DispatchGuildSubscriptions { get; }

        /// <inheritdoc />
        public Optional<GatewayIntents> Intents { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identify"/> class.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="properties">The connection properties.</param>
        /// <param name="compress">Whether compression is supported.</param>
        /// <param name="largeThreshold">The large guild threshold.</param>
        /// <param name="shard">The sharding ID.</param>
        /// <param name="presence">The initial presence.</param>
        /// <param name="dispatchGuildSubscriptions">Whether to receive subscription events.</param>
        /// <param name="intents">The gateway intents.</param>
        public Identify
        (
            string token,
            IConnectionProperties properties,
            Optional<bool> compress = default,
            Optional<byte> largeThreshold = default,
            Optional<IShardIdentification> shard = default,
            Optional<IUpdateStatus> presence = default,
            [JsonProperty("guild_subscriptions")] Optional<bool> dispatchGuildSubscriptions = default,
            Optional<GatewayIntents> intents = default
        )
        {
            this.Token = token;
            this.Properties = properties;
            this.Compress = compress;
            this.LargeThreshold = largeThreshold;
            this.Shard = shard;
            this.Presence = presence;
            this.DispatchGuildSubscriptions = dispatchGuildSubscriptions;
            this.Intents = intents;
        }
    }
}
