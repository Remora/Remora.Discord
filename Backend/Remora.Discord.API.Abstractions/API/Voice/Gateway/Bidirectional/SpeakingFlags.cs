//
//  SpeakingFlags.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Voice.Gateway.Bidirectional
{
    /// <summary>
    /// Enumerates various speaker flags to indicate how voice data is being sent.
    /// </summary>
    [PublicAPI, Flags]
    public enum SpeakingFlags
    {
        /// <summary>
        /// Normal transmission of voice audio.
        /// </summary>
        Microphone = 1 << 0,

        /// <summary>
        /// Transmission of context audio for video. No speaking indicator will be shown.
        /// </summary>
        Soundshare = 1 << 1,

        /// <summary>
        /// Priority speech. Audio of other speakers will be lowered.
        /// </summary>
        Priority = 1 << 2
    }
}
