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
        bool HasValue { get; }

        /// <summary>
        /// Determines whether the option has a defined value; that is, whether it both has a value and that value is
        /// non-null.
        /// </summary>
        /// <returns>true if the optional has a value and that value is non-null; otherwise, false.</returns>
        bool IsDefined();
    }
}
