//
//  CacheResult.cs
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

using System.Diagnostics.CodeAnalysis;

namespace Remora.Discord.Caching
{
    /// <summary>
    /// Result from the cache.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public readonly struct CacheResult<T> where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheResult{T}"/> struct.
        /// </summary>
        /// <param name="value">Value in the caching provider.</param>
        public CacheResult(T value)
        {
            HasValue = true;
            Value = value;
        }

        /// <summary>
        ///     Gets a value indicating whether the result has been found from the caching provider.
        /// </summary>
        [MemberNotNullWhen(returnValue: true, member: nameof(Value))]
        public bool HasValue { get; }

        /// <summary>
        ///     Gets value in the cache.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Transforms the instance into a successful cache result.
        /// </summary>
        /// <param name="value">The value to hold.</param>
        /// <returns>Instance of cache result.</returns>
        public static implicit operator CacheResult<T>(T value)
        {
            return new(value);
        }
    }
}
