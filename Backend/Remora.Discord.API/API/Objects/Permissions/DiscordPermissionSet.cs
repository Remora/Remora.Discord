//
//  DiscordPermissionSet.cs
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
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Generators.Support;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class DiscordPermissionSet : IDiscordPermissionSet
    {
        /// <inheritdoc />
        public BigInteger Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public DiscordPermissionSet(BigInteger value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
        /// </summary>
        /// <param name="permissions">The permissions in the set.</param>
        public DiscordPermissionSet(params DiscordPermission[] permissions)
        {
            var largestPermission = permissions.Max(p => (ulong)p);
            var largestByteIndex = (int)Math.Floor(largestPermission / 8.0);

            // Create a sufficiently sized byte area
            Span<byte> bytes = stackalloc byte[largestByteIndex + 1];

            // Convert the permission set to a byte array
            foreach (var permission in permissions)
            {
                // The value of the permission is defined as the zero-based bit index, that is, a Discord value of
                // 0x00000001 (the first permission) is represented with the value 0.
                var bit = (ulong)permission;

                // The byte index is the zero-based index into the Value property where the bit in question is.
                var byteIndex = (int)Math.Floor(bit / 8.0);

                // The bit index is the index into the byte of the value
                var bitIndex = (byte)(bit - (ulong)(8 * byteIndex));

                // Update the value in the array
                ref var currentValue = ref bytes[byteIndex];
                currentValue |= (byte)(1 << bitIndex);
            }

            this.Value = new BigInteger(bytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
        /// </summary>
        /// <param name="permissions">The permissions in the set.</param>
        public DiscordPermissionSet(params DiscordTextPermission[] permissions)
            : this(permissions.Cast<DiscordPermission>().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
        /// </summary>
        /// <param name="permissions">The permissions in the set.</param>
        public DiscordPermissionSet(params DiscordVoicePermission[] permissions)
            : this(permissions.Cast<DiscordPermission>().ToArray())
        {
        }

        /// <inheritdoc />
        public bool HasPermission(DiscordPermission permission)
        {
            // The value of the permission is defined as the zero-based bit index, that is, a Discord value of
            // 0x00000001 (the first permission) is represented with the value 0.
            var bit = (ulong)permission;

            // The byte index is the zero-based index into the Value property where the bit in question is.
            var byteIndex = (int)Math.Floor(bit / 8.0);

            // The bit index is the index into the byte of the value
            var bitIndex = (byte)(bit - (ulong)(8 * byteIndex));

            if (this.Value.GetByteCount(true) <= byteIndex)
            {
                // This would fall outside of the value; therefore, it cannot be contained in the set.
                return false;
            }

            var byteValue = this.Value.ToByteArray(true)[byteIndex];
            var isBitSet = ((byteValue >> bitIndex) & 0x00000001) > 0;

            return isBitSet;
        }

        /// <inheritdoc />
        public bool HasPermission(DiscordTextPermission permission) => HasPermission((DiscordPermission)permission);

        /// <inheritdoc />
        public bool HasPermission(DiscordVoicePermission permission) => HasPermission((DiscordPermission)permission);
    }
}
