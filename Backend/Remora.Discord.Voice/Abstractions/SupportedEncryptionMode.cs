//
//  SupportedEncryptionMode.cs
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

namespace Remora.Discord.Voice.Abstractions
{
    /// <summary>
    /// Specifies how nonces should be used within the encryption algorithm, and attached to RTP packets for communication with Discord.
    /// </summary>
    internal enum SupportedEncryptionMode
    {
        /// <summary>
        /// The nonce consists of the 12-byte RTP header and a further 12 null bytes. The RTP packet does not need to be modified.
        /// </summary>
        XSalsa20_Poly1305,

        /// <summary>
        /// The nonce consists of random bytes, and appended to the end of the RTP packet.
        /// </summary>
        XSalsa20_Poly1305_Suffix,

        /// <summary>
        /// An incrementing uint32 value, encoded as a big endian value and placed at the beginning of the nonce buffer which is otherwise null.
        /// The 4-byte nonce value is appended to the end of the RTP packet.
        /// </summary>
        XSalsa20_Poly1305_Lite
    }
}
