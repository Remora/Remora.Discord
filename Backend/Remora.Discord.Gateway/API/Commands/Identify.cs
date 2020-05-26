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

using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents an identification command sent to the Discord gateway.
    /// </summary>
    internal sealed class Identify
    {
        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the connection properties.
        /// </summary>
        public ConnectionProperties Properties { get; }

        /// <summary>
        /// Gets an optional field, containing a value that indicates whether the connection supports compressed
        /// packets.
        /// </summary>
        public Optional<bool> Compress { get; }

        /// <summary>
        /// Gets an optional field, containing the threshold value of total guild members before a guild is considered
        /// large, and offline members will not automatically be sent.
        /// </summary>
        public Optional<byte> LargeThreshold { get; }

        /// <summary>
        /// Gets an optional field, containing the sharding ID for this connection.
        /// </summary>
        public Optional<ShardIdentification> Shard { get; }

        /// <summary>
        /// Gets an optional field, containing initial presence information.
        /// </summary>
        public Optional<UpdateStatus> Presence { get; }

        /// <summary>
        /// Gets an optional field, containing a value that indicates whether guild subscription events (such as
        /// presence and typing) should be sent.
        /// </summary>
        public Optional<bool> GuildSubscriptions { get; }

        /// <summary>
        /// Gets an optional field, containing the gateway intents the connection wants to receive.
        /// </summary>
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
        /// <param name="guildSubscriptions">Whether to receive subscription events.</param>
        /// <param name="intents">The gateway intents.</param>
        public Identify
        (
            string token,
            ConnectionProperties properties,
            Optional<bool> compress = default,
            Optional<byte> largeThreshold = default,
            Optional<ShardIdentification> shard = default,
            Optional<UpdateStatus> presence = default,
            Optional<bool> guildSubscriptions = default,
            Optional<GatewayIntents> intents = default
        )
        {
            this.Token = token;
            this.Properties = properties;
            this.Compress = compress;
            this.LargeThreshold = largeThreshold;
            this.Shard = shard;
            this.Presence = presence;
            this.GuildSubscriptions = guildSubscriptions;
            this.Intents = intents;
        }
    }
}
