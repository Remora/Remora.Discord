//
//  OpusErrorDefinition.cs
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

namespace Remora.Discord.Voice.Interop.Opus
{
    /// <summary>
    /// Enumerates error codes for the reference libopus implementation.
    /// </summary>
    [PublicAPI]
    public enum OpusErrorDefinition
    {
        /// <summary>
        /// Memory allocation has failed.
        /// </summary>
        AllocationFail = -7,

        /// <summary>
        /// An encoder or decoder state is invalid or already freed.
        /// </summary>
        InvalidState = -6,

        /// <summary>
        /// Invalid/unsupported request number.
        /// </summary>
        Unimplemented = -5,

        /// <summary>
        /// The compressed data passed is corrupted.
        /// </summary>
        InvalidPacket = -4,

        /// <summary>
        /// An internal error was detected.
        /// </summary>
        InternalError = -3,

        /// <summary>
        /// Not enough bytes allocated in the buffer.
        /// </summary>
        BufferTooSmall = -2,

        /// <summary>
        /// One or more invalid/out of range arguments.
        /// </summary>
        BadArgument = -1,

        /// <summary>
        /// No error.
        /// </summary>
        OK = 0
    }
}
