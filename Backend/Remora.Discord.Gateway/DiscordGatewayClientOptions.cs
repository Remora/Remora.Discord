//
//  DiscordGatewayClientOptions.cs
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
using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.API.API.Commands;

namespace Remora.Discord.Gateway
{
    /// <summary>
    /// Holds various client options for use in the gateway client.
    /// </summary>
    public class DiscordGatewayClientOptions
    {
        /// <summary>
        /// Gets or sets the safety margin for the heartbeat interval. The client will aim to send a heartbeat within
        /// this time before the actual interval. The actual safety margin will never exceed 10% of the total interval,
        /// and will never be less than <see cref="MinimumSafetyMargin"/>.
        /// </summary>
        public TimeSpan HeartbeatSafetyMargin { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Gets or sets the minimum safety margin.
        /// </summary>
        public TimeSpan MinimumSafetyMargin { get; set; } = TimeSpan.FromMilliseconds(20);

        /// <summary>
        /// Gets or sets the shard identification information. This is used to connect the client as a sharded
        /// connection, where events are distributed over a set of active connections.
        /// </summary>
        public IShardIdentification? ShardIdentification { get; set; } = null;

        /// <summary>
        /// Gets or sets the gateway intents to subscribe to. By default, this is a limited set of intents (guilds and
        /// their messages).
        /// </summary>
        public GatewayIntents Intents { get; set; } = GatewayIntents.Guilds | GatewayIntents.GuildMessages;

        /// <summary>
        /// Gets or sets the connection properties that identify the connecting client or library code. By default,
        /// this is the information about Remora.Discord itself. You may, optionally, override this to present your
        /// own information.
        /// </summary>
        public IConnectionProperties ConnectionProperties { get; set; } = new ConnectionProperties("Remora.Discord");

        /// <summary>
        /// Calculates the true heartbeat safety margin, based on the heartbeat interval.
        /// </summary>
        /// <param name="heartbeatInterval">The heartbeat interval.</param>
        /// <returns>The true safety margin.</returns>
        public TimeSpan GetTrueHeartbeatSafetyMargin(TimeSpan heartbeatInterval)
        {
            var safetyMargin = TimeSpan.FromMilliseconds
            (
                Math.Clamp
                (
                    this.HeartbeatSafetyMargin.TotalMilliseconds,
                    this.MinimumSafetyMargin.TotalMilliseconds,
                    heartbeatInterval.TotalMilliseconds / 10
                )
            );

            return safetyMargin;
        }
    }
}
