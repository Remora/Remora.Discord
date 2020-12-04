//
//  IUpdateable.cs
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

namespace Remora.Discord.Generators.Support
{
    /// <summary>
    /// Represents the public API of an updateable record.
    /// </summary>
    /// <typeparam name="TRecord">The record type.</typeparam>
    public interface IUpdateable<TRecord> where TRecord : class, IUpdateable<TRecord>
    {
        /// <summary>
        /// Updates the record with the values of the provided record, respecting optional values.
        /// </summary>
        /// <param name="other">The record to update from.</param>
        /// <returns>An updated copy.</returns>
        TRecord Update(TRecord other);
    }
}
