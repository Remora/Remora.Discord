//
//  IVoiceReady.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Voice.Gateway.Events
{
    /// <summary>
    /// Indicates a successful authentication with the voice gateway, and provides initial connection information.
    /// </summary>
    [PublicAPI]
    public interface IVoiceReady : IVoiceGatewayEvent
    {
        /// <summary>
        /// Gets the synchronization source ID. This acts as an identifier for this voice connection.
        /// </summary>
        uint SSRC { get; }

        /// <summary>
        /// Gets the IP that the voice server is available on.
        /// </summary>
        string IP { get; }

        /// <summary>
        /// Gets the UDP port that the voice server is available on.
        /// </summary>
        ushort Port { get; }

        /// <summary>
        /// Gets the encryption modes the server supports.
        /// <remarks>
        /// At the moment, Discord supports the following modes:
        ///     - xsalsa20_poly1305
        ///     - xsalsa20_poly1305_suffix
        ///     - xsalsa20_poly1305_lite
        /// </remarks>
        /// </summary>
        IReadOnlyList<string> Modes { get; }
    }
}
