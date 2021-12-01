//
//  OpusApplicationDefinition.cs
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
    /// Enumerates application definitions for the reference libopus implementation.
    /// </summary>
    [PublicAPI]
    public enum OpusApplicationDefinition
    {
        /// <summary>
        /// Encoded audio is optimised for VoIP/videoconference applications where listening quality and intelligibility matter most.
        /// </summary>
        Voip = 2048,

        /// <summary>
        /// Encoded audio is optimised for broadcast/high-fidelity application where the decoded audio should be as close as possible to the input.
        /// </summary>
        Audio = 2049,

        /// <summary>
        /// Encoded audio is optimised for when lowest-achievable latency is what matters most. Voice-optimized modes cannot be used.
        /// </summary>
        RestrictedLowDelay = 2051
    }
}
