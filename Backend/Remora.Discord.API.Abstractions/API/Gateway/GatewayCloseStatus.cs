//
//  GatewayCloseStatus.cs
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

namespace Remora.Discord.API.Abstractions.Gateway;

/// <summary>
/// Enumerates the various close codes that the Discord gateway can send.
/// </summary>
[PublicAPI]
public enum GatewayCloseStatus
{
    /// <summary>
    /// Unknown error. Attempt to reconnect.
    /// </summary>
    UnknownError = 4000,

    /// <summary>
    /// An invalid opcode or opcode/payload combination was sent to the gateway.
    /// </summary>
    UnknownOpcode = 4001,

    /// <summary>
    /// An invalid payload was sent to the gateway.
    /// </summary>
    DecodeError = 4002,

    /// <summary>
    /// A payload was sent before identifying.
    /// </summary>
    NotAuthenticated = 4003,

    /// <summary>
    /// The authentication token sent in the identify payload was invalid.
    /// </summary>
    AuthenticationFailed = 4004,

    /// <summary>
    /// An identify payload has already been sent.
    /// </summary>
    AlreadyAuthenticated = 4005,

    /// <summary>
    /// An invalid sequence number was provided when resuming. Reconnect and start a new session.
    /// </summary>
    InvalidSequence = 4007,

    /// <summary>
    /// You are being rate limited. Wait a while and reconnect.
    /// </summary>
    RateLimited = 4008,

    /// <summary>
    /// Your session timed out. Reconnect and start a new session.
    /// </summary>
    SessionTimedOut = 4009,

    /// <summary>
    /// An invalid shard was sent when identifying.
    /// </summary>
    InvalidShard = 4010,

    /// <summary>
    /// Sharding is required by this session.
    /// </summary>
    ShardingRequired = 4011,

    /// <summary>
    /// An invalid API version was sent to the gateway.
    /// </summary>
    InvalidAPIVersion = 4012,

    /// <summary>
    /// An invalid set of gateway intents were sent.
    /// </summary>
    InvalidIntents = 4013,

    /// <summary>
    /// A disallowed set of gateway intents were sent.
    /// </summary>
    DisallowedIntent = 4014
}
