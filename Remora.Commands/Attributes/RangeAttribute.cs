//
//  RangeAttribute.cs
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
using System.Collections.Generic;

namespace Remora.Commands.Attributes
{
    /// <summary>
    /// Marks a parameter as having a restricted range of item counts. This attribute only has an effect on
    /// collection-like parameters (such as <see cref="IEnumerable{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RangeAttribute : Attribute
    {
        private ulong? _min;
        private ulong? _max;

        /// <summary>
        /// Sets the minimum number of elements in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the getter is used.</exception>
#pragma warning disable SA1623
        public ulong Min
        {
            [Obsolete("Do not use.", true)] get => throw new InvalidOperationException();
            set => _min = value;
        }

        /// <summary>
        /// Sets the minimum number of elements in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the getter is used.</exception>
        public ulong Max
        {
            [Obsolete("Do not use.", true)] get => throw new InvalidOperationException();
            set => _max = value;
        }
#pragma warning restore

        /// <summary>
        /// Gets the minimum number of elements in the collection.
        /// </summary>
        /// <returns>The minimum number.</returns>
        public ulong? GetMin() => _min;

        /// <summary>
        /// Gets the maximum number of elements in the collection.
        /// </summary>
        /// <returns>The maximum number of elements.</returns>
        public ulong? GetMax() => _max;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        public RangeAttribute()
        {
        }
    }
}
