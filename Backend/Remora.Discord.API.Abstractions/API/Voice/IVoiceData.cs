//
//  IVoiceData.cs
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

namespace Remora.Discord.API.Abstractions.Voice
{
    /// <summary>
    /// Represents real-time voice data from Discord. This technically represents an RFC3550 RTP packet, but takes a lot
    /// of shortcuts to avoid implementing stuff Discord doesn't use or provide.
    /// </summary>
    public interface IVoiceData
    {
        /// <summary>
        /// Gets the combined version+flags from the RTP header.
        /// </summary>
        byte VersionAndFlags { get; }

        /// <summary>
        /// Gets the payload type from the RTP header.
        /// </summary>
        byte PayloadType { get; }

        /// <summary>
        /// Gets the sequence number from the RTP header.
        /// </summary>
        ushort Sequence { get; }

        /// <summary>
        /// Gets the timestamp from the RTP header.
        /// </summary>
        uint Timestamp { get; }

        /// <summary>
        /// Gets the synchronization source ID from the RTP header.
        /// </summary>
        uint SSRC { get; }

        /// <summary>
        /// Gets the encrypted binary audio data.
        /// </summary>
        IReadOnlyList<byte> EncryptedAudio { get; }
    }
}
