//
//  OperationCode.cs
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
/// Enumerates operation codes sent to or received by the Discord gateway in payloads.
/// </summary>
[PublicAPI]
public enum OperationCode
{
    /// <summary>
    /// An event was dispatched.
    /// </summary>
    Dispatch = 0,

    /// <summary>
    /// Fired periodically by the client to keep the connection alive.
    /// </summary>
    Heartbeat = 1,

    /// <summary>
    /// Starts a new session during the initial handshake.
    /// </summary>
    Identify = 2,

    /// <summary>
    /// Update the client's presence.
    /// </summary>
    PresenceUpdate = 3,

    /// <summary>
    /// Used to join/leave or move between voice channels.
    /// </summary>
    VoiceStateUpdate = 4,

    /// <summary>
    /// This opcode is unknown.
    /// </summary>
    Unknown = 5,

    /// <summary>
    /// Resume a previous session that was disconnected.
    /// </summary>
    Resume = 6,

    /// <summary>
    /// You should attempt to reconnect and resume immediately.
    /// </summary>
    Reconnect = 7,

    /// <summary>
    /// Request information about offline guild members in a large guild.
    /// </summary>
    RequestGuildMembers = 8,

    /// <summary>
    /// The session has been invalidated. You should reconnect and identify/resume accordingly.
    /// </summary>
    InvalidSession = 9,

    /// <summary>
    /// Sent immediately after connecting. Contains the heartbeat interval to use.
    /// </summary>
    Hello = 10,

    /// <summary>
    /// Sent in response to receiving a heartbeat to acknowledge that it has been received.
    /// </summary>
    HeartbeatAcknowledge = 11
}
