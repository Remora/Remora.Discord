//
//  VoiceOperationCode.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Voice.Gateway
{
    /// <summary>
    /// Enumerates various operation codes for the voice gateway.
    /// </summary>
    [PublicAPI]
    public enum VoiceOperationCode
    {
        /// <summary>
        /// Begin a voice websocket connection.
        /// </summary>
        Identify = 0,

        /// <summary>
        /// Select the voice protocol.
        /// </summary>
        SelectProtocol = 1,

        /// <summary>
        /// Complete the websocket handshake.
        /// </summary>
        Ready = 2,

        /// <summary>
        /// Keep the websocket connection alive.
        /// </summary>
        Heartbeat = 3,

        /// <summary>
        /// Describe the session.
        /// </summary>
        SessionDescription = 4,

        /// <summary>
        /// Indicate which users are speaking.
        /// </summary>
        Speaking = 5,

        /// <summary>
        /// Acknowledge a received heartbeat.
        /// </summary>
        HeartbeatAcknowledgement = 6,

        /// <summary>
        /// Resume a connection.
        /// </summary>
        Resume = 7,

        /// <summary>
        /// Provided initial connection information.
        /// </summary>
        Hello = 8,

        /// <summary>
        /// Acknowledge a successful session resume.
        /// </summary>
        Resumed = 9,

        /// <summary>
        /// A client has disconnected from the voice channel.
        /// </summary>
        ClientDisconnect = 13
    }
}
