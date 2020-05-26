//
//  Snowflake.cs
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Remora.Discord.Core
{
    /// <summary>
    /// Represents a Twitter snowflake, used as a unique ID.
    /// </summary>
    public readonly struct Snowflake : IEquatable<Snowflake>, IComparable<Snowflake>
    {
        /// <summary>
        /// Gets the Discord epoch, used for timestamp offsetting.
        /// </summary>
        private const ulong DiscordEpoch = 1420070400000;

        /// <summary>
        /// Gets the internal value representation of the snowflake.
        /// </summary>
        [PublicAPI]
        public ulong Value { get; }

        /// <summary>
        /// Gets the timestamp embedded in the snowflake.
        /// </summary>
        [PublicAPI]
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the internal worker ID used by Discord.
        /// </summary>
        [PublicAPI]
        public byte InternalWorkerID { get; }

        /// <summary>
        /// Gets the internal process ID used by Discord.
        /// </summary>
        [PublicAPI]
        public byte InternalProcessID { get; }

        /// <summary>
        /// Gets a per-process increment. This number is incremented every time a new ID is generated on the process
        /// referred to by <see cref="InternalProcessID"/>.
        /// </summary>
        [PublicAPI]
        public ushort Increment { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake"/> struct.
        /// </summary>
        /// <param name="value">The binary representation of the snowflake.</param>
        [PublicAPI]
        public Snowflake(ulong value)
        {
            this.Value = value;
            this.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)((value >> 22) + DiscordEpoch)).UtcDateTime;
            this.InternalWorkerID = (byte)((value & 0x3E0000) >> 17);
            this.InternalProcessID = (byte)((value & 0x1F0000) >> 12);
            this.Increment = (ushort)(value & 0xFFF);
        }

        /// <summary>
        /// Creates a new snowflake from a timestamp. This is generally used for pagination in the Discord API.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The snowflake.</returns>
        [PublicAPI]
        public static Snowflake CreateTimestampSnowflake(DateTimeOffset? timestamp = null)
        {
            timestamp ??= DateTimeOffset.UtcNow;

            var value = (ulong)((timestamp.Value.ToUnixTimeMilliseconds() - (long)DiscordEpoch) << 22);
            return new Snowflake(value);
        }

        /// <summary>
        /// Attempts to parse a snowflake value from the given string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if a snowflake was successfully parsed; otherwise, false.</returns>
        [PublicAPI]
        public static bool TryParse(string value, [NotNullWhen(true)] out Snowflake? result)
        {
            result = null;

            if (!ulong.TryParse(value, out var binary))
            {
                return false;
            }

            var snowflake = new Snowflake(binary);
            if (snowflake.Timestamp < DateTimeOffset.FromUnixTimeMilliseconds((long)DiscordEpoch))
            {
                // Bad ID, discord didn't exist before then
                return false;
            }

            result = snowflake;

            return true;
        }

        /// <inheritdoc/>
        [PublicAPI]
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <inheritdoc/>
        [PublicAPI]
        public override bool Equals(object? obj)
        {
            return obj is Snowflake other && Equals(other);
        }

        /// <inheritdoc/>
        [PublicAPI]
        public bool Equals(Snowflake other)
        {
            return this.Value == other.Value;
        }

        /// <inheritdoc/>
        [PublicAPI]
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <inheritdoc/>
        [PublicAPI]
        public int CompareTo(Snowflake other)
        {
            return this.Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares two snowflakes, determining whether the left operand is considered less than the right operand.
        /// This is generally based on time. An earlier snowflake will compare as less than another, and a snowflake
        /// with a higher increment will compare as more than another (provided they are from the same worker and same
        /// time).
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>true if the relationship holds; otherwise, false.</returns>
        [PublicAPI]
        public static bool operator <(Snowflake left, Snowflake right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compares two snowflakes, determining whether the left operand is considered greater than the right operand.
        /// This is generally based on time. An earlier snowflake will compare as less than another, and a snowflake
        /// with a higher increment will compare as more than another (provided they are from the same worker and same
        /// time).
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>true if the relationship holds; otherwise, false.</returns>
        [PublicAPI]
        public static bool operator >(Snowflake left, Snowflake right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Compares two snowflakes, determining whether the left operand is considered less than or equal to the right
        /// operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
        /// snowflake with a higher increment will compare as more than another (provided they are from the same worker
        /// and same time).
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>true if the relationship holds; otherwise, false.</returns>
        [PublicAPI]
        public static bool operator <=(Snowflake left, Snowflake right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Compares two snowflakes, determining whether the left operand is considered greater than or equal to the
        /// right operand. This is generally based on time. An earlier snowflake will compare as less than another, and
        /// a snowflake with a higher increment will compare as more than another (provided they are from the same
        /// worker and same time).
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>true if the relationship holds; otherwise, false.</returns>
        [PublicAPI]
        public static bool operator >=(Snowflake left, Snowflake right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
