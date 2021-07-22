//
//  VoiceGatewayCloseStatus.cs
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
    /// Enumerates various close status codes for the voice gateway.
    /// </summary>
    [PublicAPI]
    public enum VoiceGatewayCloseStatus
    {
        /// <summary>
        /// You sent an invalid operation code.
        /// </summary>
        UnknownOperationCode = 4001,

        /// <summary>
        /// You sent an invalid payload when identifying.
        /// </summary>
        FailedToDecodePayload = 4002,

        /// <summary>
        /// You sent a payload before authenticating with the gateway.
        /// </summary>
        NotAuthenticated = 4003,

        /// <summary>
        /// The token you sent in your identify payload is invalid.
        /// </summary>
        AuthenticationFailed = 4004,

        /// <summary>
        /// You sent more than one identify payload.
        /// </summary>
        AlreadyAuthenticated = 4005,

        /// <summary>
        /// Your session is no longer valid.
        /// </summary>
        SessionNoLongerValid = 4006,

        /// <summary>
        /// Your session timed out.
        /// </summary>
        SessionTimeout = 4009,

        /// <summary>
        /// We can't find the server you're trying to connect to.
        /// </summary>
        ServerNotFound = 4011,

        /// <summary>
        /// We didn't recognize the protocol you sent.
        /// </summary>
        UnknownProtocol = 4012,

        /// <summary>
        /// The channel was deleted, you were kicked, or the main gateway session was dropped.
        /// </summary>
        Disconnected = 4014,

        /// <summary>
        /// The voice server crashed.
        /// </summary>
        VoiceServerCrash = 4015,

        /// <summary>
        /// We didn't recognize your encryption.
        /// </summary>
        UnknownEncryptionMode = 4016
    }
}
