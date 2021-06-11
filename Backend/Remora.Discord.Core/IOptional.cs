//
//  IOptional.cs
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

namespace Remora.Discord.Core
{
    /// <summary>
    /// Defines basic functionality for an optional.
    /// </summary>
    [PublicAPI]
    public interface IOptional
    {
        /// <summary>
        /// Gets a value indicating whether the optional contains a value.
        /// </summary>
        [Obsolete("Prefer IOptional.IsSpecified")]
        bool HasValue { get; }

        /// <summary>
        /// Gets a value indicating whether the optional value is logically present.
        /// </summary>
        bool IsSpecified { get; }

        /// <summary>
        /// Gets a value indicating whether the value is <c>null</c>. If <see cref="IsSpecified"/> is false, <see cref="IsNull"/> is undefined.
        /// </summary>
        bool IsNull { get; }
    }
}
