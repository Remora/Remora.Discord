//
//  IGuildScheduledEventUserRemove.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Sent when a user unsubscribes from a scheduled event.
/// </summary>
[PublicAPI]
public interface IGuildScheduledEventUserRemove : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the event.
    /// </summary>
    Snowflake GuildScheduledEventID { get; }

    /// <summary>
    /// Gets the ID of the user that unsubscribed from the event.
    /// </summary>
    Snowflake UserID { get; }

    /// <summary>
    /// Gets the ID of the guild the event is in.
    /// </summary>
    Snowflake GuildID { get; }
}
