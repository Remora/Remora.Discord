//
//  TypeDetectionExtensions.cs
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

namespace Remora.Discord.Rest.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="byte"/> arrays, used to detect file types.
    /// </summary>
    internal static class TypeDetectionExtensions
    {
        private const ulong PNGSignature = 9894494448401390090;

        /// <summary>
        /// Determines whether the array contains PNG data.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>true if the array contains PNG data; otherwise, false.</returns>
        public static bool IsPNG(this byte[] array)
        {
            if (array.Length < 8)
            {
                return false;
            }

            var signature = BitConverter.ToUInt64(array[..7]);
            return signature == PNGSignature;
        }

        /// <summary>
        /// Determines whether the array contains GIF data.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>true if the array contains GIF data; otherwise, false.</returns>
        public static bool IsGIF(this byte[] array)
        {
            if (array.Length < 3)
            {
                return false;
            }

            return array[0] == 0x47 && array[1] == 0x49 && array[2] == 0x46;
        }

        /// <summary>
        /// Determines whether the array contains JPG data.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>true if the array contains JPG data; otherwise, false.</returns>
        public static bool IsJPG(this byte[] array)
        {
            if (array.Length < 3)
            {
                return false;
            }

            return array[0] == 0xFF && array[1] == 0xD8 && array[2] == 0xFF;
        }
    }
}
