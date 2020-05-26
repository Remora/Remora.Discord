//
//  OperationCode.cs
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

namespace Remora.Discord.Gateway.API
{
    /// <summary>
    /// Enumerates operation codes sent to or received by the Discord gateway in payloads.
    /// </summary>
    internal enum OperationCode
    {
        /// <summary>
        /// An event was dispatched.
        /// </summary>
        Dispatch,

        /// <summary>
        /// Fired periodically by the client to keep the connection alive.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// Starts a new session during the initial handshake.
        /// </summary>
        Identify,

        /// <summary>
        /// Update the client's presence.
        /// </summary>
        PresenceUpdate,

        /// <summary>
        /// Used to join/leave or move between voice channels.
        /// </summary>
        VoiceStateUpdate,

        /// <summary>
        /// Resume a previous session that was disconnected.
        /// </summary>
        Resume,

        /// <summary>
        /// You should attempt to reconnect and resume immediately.
        /// </summary>
        Reconnect,

        /// <summary>
        /// Request information about offline guild members in a large guild.
        /// </summary>
        RequestGuildMembers,

        /// <summary>
        /// The session has been invalidated. You should reconnect and identify/resume accordingly.
        /// </summary>
        InvalidSession,

        /// <summary>
        /// Sent immediately after connecting. Contains the heartbeat interval to use.
        /// </summary>
        Hello,

        /// <summary>
        /// Sent in response to receiving a heartbeat to acknowledge that it has been received.
        /// </summary>
        HeartbeatAcknowledge
    }
}
