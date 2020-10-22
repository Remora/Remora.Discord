//
//  ICollectionParameterShape.cs
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

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents the shape of a collection of parameters.
    /// </summary>
    public interface ICollectionParameterShape
    {
        /// <summary>
        /// Gets the minimum number of elements in the collection. If no minimum is set, the collection is allowed to
        /// be empty.
        /// </summary>
        ulong? Min { get; }

        /// <summary>
        /// Gets the maximum number of elements in the collection. If no maximum is set, the collection has no upper
        /// bound on its size.
        /// </summary>
        ulong? Max { get; }
    }
}
