//
//  IPDiscoveryRequest.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Voice.Objects.UdpDataProtocol
{
    /// <summary>
    /// Represents an IP discovery request packet.
    /// </summary>
    /// <param name="SSRC">The SSRC.</param>
    /// <param name="Length">The length of the packet, not including the <paramref name="Length"/> and <paramref name="Type"/> fields.</param>
    /// <param name="Type">The type of the discovery packet.</param>
    [PublicAPI]
    public record IPDiscoveryRequest
    (
        uint SSRC,
        ushort Length,
        IPDiscoveryPacketType Type = IPDiscoveryPacketType.Request
    )
    {
        /// <summary>
        /// Packs the packet.
        /// </summary>
        /// <param name="packTo">The buffer to pack the packet into.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pack(Span<byte> packTo)
        {
            BinaryPrimitives.WriteUInt16BigEndian(packTo, (ushort)Type);
            BinaryPrimitives.WriteUInt16BigEndian(packTo[2..], Length);
            BinaryPrimitives.WriteUInt32BigEndian(packTo[4..], SSRC);
        }

        /// <summary>
        /// Calculates the value of the <see cref="Length"/> property from the given buffer size.
        /// </summary>
        /// <param name="bufferSize">The size of the buffer that will be used to store the packet.</param>
        /// <returns>The value that should be used for the <see cref="Length"/> property.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort CalculateEmbeddedLength(int bufferSize)
            => (ushort)(bufferSize - sizeof(ushort) - sizeof(ushort));
    }
}
