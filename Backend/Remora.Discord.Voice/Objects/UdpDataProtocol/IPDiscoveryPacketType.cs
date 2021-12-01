//
//  IPDiscoveryPacketType.cs
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

namespace Remora.Discord.Voice.Objects.UdpDataProtocol
{
    /// <summary>
    /// Enumerates the possible OP codes for IP discovery packets.
    /// </summary>
    [PublicAPI]
    public enum IPDiscoveryPacketType : ushort
    {
        /// <summary>
        /// Defines the OP code of an IP discovery request packet.
        /// </summary>
        Request = 0x1,

        /// <summary>
        /// Defines the OP code of an IP discovery response packet.
        /// </summary>
        Response = 0x2,
    }
}
