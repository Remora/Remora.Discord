//
//  IChannelPinsUpdate.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents a pin or unpin of a message in a channel. This is not sent when a pinned message is deleted.
/// </summary>
[PublicAPI]
public interface IChannelPinsUpdate : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the ID of the channel.
    /// </summary>
    Snowflake ChannelID { get; }

    /// <summary>
    /// Gets the time at which the most recent pinned message was pinned.
    /// </summary>
    Optional<DateTimeOffset?> LastPinTimestamp { get; }
}
