//
//  VoiceConstants.cs
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

namespace Remora.Discord.Voice.Util
{
    /// <summary>
    /// Defines constants for voice data transmission.
    /// </summary>
    [PublicAPI]
    public static class VoiceConstants
    {
        /// <summary>
        /// Gets the sample rate at which Discord transmits and receives audio.
        /// </summary>
        public const int DiscordSampleRate = 48000;

        /// <summary>
        /// Gets the number of channels in audio transmitted/received by Discord.
        /// </summary>
        public const int DiscordChannelCount = 2;

        /// <summary>
        /// Gets the maximum allowed bitrate in a Discord channel. Currently 128kbps.
        /// </summary>
        public const int DiscordMaxBitrate = 131072;
    }
}
