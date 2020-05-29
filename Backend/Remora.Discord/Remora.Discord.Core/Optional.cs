//
//  Optional.cs
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
    /// Represents an optional value. This is mainly used for JSON de/serializalization where a value can be either
    /// present, null, or completely missing.
    ///
    /// While a <see cref="Nullable{T}"/> allows for a value to be logically present but contain a null value,
    /// <see cref="Optional{TValue}"/> allows for a value to be logically missing. For example, an optional without a
    /// value would never be serialized, but a nullable with a null value would (albeit as "null").
    /// </summary>
    /// <typeparam name="TValue">The inner type.</typeparam>
    public readonly struct Optional<TValue>
    {
        [MaybeNull, AllowNull]
        private readonly TValue _value;

        /// <summary>
        /// Gets the value contained in the optional.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the optional does not contain a value.</exception>
        [MaybeNull]
        [PublicAPI]
        public TValue Value
        {
            get
            {
                if (this.HasValue)
                {
                    // We want to explicitly allow returning null here. ! is technically a lie, but it works.
                    return _value!;
                }

                throw new InvalidOperationException("The optional did not contain a valid value.");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the optional contains a value.
        /// </summary>
        [PublicAPI]
        public bool HasValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Optional{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The contained value.</param>
        [PublicAPI]
        public Optional([AllowNull] TValue value)
        {
            _value = value;
            this.HasValue = true;
        }
    }
}
