//
//  IPDiscoveryResponse.cs
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

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Remora.Discord.Voice.Errors;
using Remora.Results;

namespace Remora.Discord.Voice.Objects.UdpDataProtocol
{
    /// <summary>
    /// Represents an IP discovery response packet.
    /// </summary>
    /// <param name="Type">The type of the discovery packet.</param>
    /// <param name="Length">The length of the discovery packet, not including the 'type' and 'length' fields.</param>
    /// <param name="SSRC">The SSRC.</param>
    /// <param name="Address">The discovered external address.</param>
    /// <param name="Port">The discovered external port.</param>
    [PublicAPI]
    public record IPDiscoveryResponse
    (
        IPDiscoveryPacketType Type,
        ushort Length,
        uint SSRC,
        string Address,
        ushort Port
    )
    {
        /// <summary>
        /// Unpacks from raw data.
        /// </summary>
        /// <param name="unpackFrom">The data to unpack from.</param>
        /// <returns>An <see cref="IPDiscoveryResponse"/> object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IPDiscoveryResponse> Unpack(ReadOnlySpan<byte> unpackFrom)
        {
            IPDiscoveryPacketType type = (IPDiscoveryPacketType)BinaryPrimitives.ReadUInt16BigEndian(unpackFrom);
            if (type is not IPDiscoveryPacketType.Response)
            {
                return new VoiceUdpError($"The provided buffer does not appear to contain an ${nameof(IPDiscoveryResponse)} packet.");
            }

            ushort length = BinaryPrimitives.ReadUInt16BigEndian(unpackFrom[2..]);
            uint ssrc = BinaryPrimitives.ReadUInt32BigEndian(unpackFrom[4..]);
            string address = Encoding.UTF8.GetString(unpackFrom[8..72]);
            ushort port = BinaryPrimitives.ReadUInt16BigEndian(unpackFrom[72..]);

            return new IPDiscoveryResponse
            (
                type,
                length,
                ssrc,
                address,
                port
            );
        }
    }
}
