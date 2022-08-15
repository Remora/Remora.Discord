//
//  DiscordGatewayClientOptions.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Gateway.Commands;

namespace Remora.Discord.Gateway;

/// <summary>
/// Holds various client options for use in the gateway client.
/// </summary>
[PublicAPI]
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
    /// Gets or sets the connection properties that identify the connecting client or library code. By default,
    /// this is the information about Remora.Discord itself. You may, optionally, override this to present your
    /// own information.
    /// </summary>
    public IConnectionProperties ConnectionProperties { get; set; } = new ConnectionProperties("Remora.Discord");

    /// <summary>
    /// Gets or sets the large threshold for the gateway.
    /// For guilds with members that exceed the threshold, offline members will be omitted.
    /// </summary>
    /// <remarks>
    /// Defaults to 50, max of 250.
    /// </remarks>
    public byte LargeThreshold { get; set; } = 50;

    /// <summary>
    /// Gets or sets the shard identification information. This is used to connect the client as a sharded
    /// connection, where events are distributed over a set of active connections.
    /// </summary>
    public IShardIdentification? ShardIdentification { get; set; }

    /// <summary>
    /// Gets or sets the initial presence when the bot connects.
    /// </summary>
    public IUpdatePresence? Presence { get; set; }

    /// <summary>
    /// Gets or sets the gateway intents to subscribe to. By default, this is a limited set of intents (guilds and
    /// their messages).
    /// </summary>
    public GatewayIntents Intents { get; set; } = GatewayIntents.Guilds | GatewayIntents.GuildMessages;

    /// <summary>
    /// Gets or sets the number of commands that may be sent as a burst within the gateway's rate limit window. A burst
    /// of 60, for example, would allow 60 commands to be sent within the first time slot in the window while the
    /// remaining commands allowed within the window would be evenly spaced across the remaining slots.
    /// </summary>
    /// <remarks>
    /// The default value is relatively conservative, and can most likely be increased to at least 100. You should,
    /// however, avoid setting this at or near the actual rate limit.
    /// </remarks>
    public byte CommandBurstRate { get; set; } = 100;

    /// <summary>
    /// Gets or sets the number of extra rate limit slots that should be taken up by the heartbeat function (beyond the
    /// ones strictly required based on the heartbeat interval and rate limit window).
    ///
    /// For example, if the rate limit window is 60 seconds, and the heartbeat interval is 2 seconds, the number of
    /// strictly required slots would be 30. Adding a headroom of 2 to this would result in 32 reserved slots within the
    /// rate limit window for heartbeats, leaving 88 slots for user-submitted events.
    ///
    /// This value is primarily used to take up the slack of any rounding errors in the calculations, as well as
    /// potential timing issues since few if any bots run on hard realtime systems. The default value of 2 has been
    /// arbitrarily picked, but should suffice for most use cases.
    /// </summary>
    public byte HeartbeatHeadroom { get; set; } = 2;

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
