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
        /// <summary>
        /// Gets the minimum number of elements in the collection.
        /// </summary>
        public ulong? Min { get; }

        /// <summary>
        /// Gets the minimum number of elements in the collection.
        /// </summary>
        public ulong? Max { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        /// <param name="min">The minimum number of elements in the collection.</param>
        /// <param name="max">The maximum number of elements in the collection.</param>
        public RangeAttribute(ulong? min, ulong? max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}
