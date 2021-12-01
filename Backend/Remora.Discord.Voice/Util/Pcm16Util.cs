//
//  Pcm16Util.cs
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

using System.Runtime.CompilerServices;

namespace Remora.Discord.Voice.Util
{
    /// <summary>
    /// Defines utilities for working with PCM-16 data.
    /// </summary>
    public static class Pcm16Util
    {
        /*
         * Implementation note:
         * Calculations are multipled/divided by 'sizeof(short)'.
         * This is to factor in that PCM-16 samples are represented as short values,
         * and our calculations are dealing with byte lengths.
         */

        /// <summary>
        /// Calculates the frame size (number of audio samples in the packet) of a PCM-16 sample in bytes.
        /// </summary>
        /// <param name="sampleSize">The byte-size of the sample.</param>
        /// <returns>The frame size of the sample in bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateFrameSize(int sampleSize)
            => sampleSize / VoiceConstants.DiscordChannelCount / sizeof(short);

        /// <summary>
        /// Calculates the duration in milliseconds of a PCM-16 sample.
        /// </summary>
        /// <param name="sampleSize">The byte-size of the sample.</param>
        /// <returns>The duration in milliseconds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateSampleDuration(int sampleSize)
            => sampleSize / (VoiceConstants.DiscordSampleRate / 1000) / VoiceConstants.DiscordChannelCount / sizeof(short);

        /// <summary>
        /// Calculates the size of a PCM-16 sample in bytes.
        /// </summary>
        /// <param name="sampleDurationMS">The duration of the sample in milliseconds.</param>
        /// <returns>The size of the sample in bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateSampleSize(int sampleDurationMS)
            => sampleDurationMS * VoiceConstants.DiscordChannelCount * (VoiceConstants.DiscordSampleRate / 1000) * sizeof(short);
    }
}
