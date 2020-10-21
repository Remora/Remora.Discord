//
//  Quotations.cs
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

using System.Collections.Generic;

namespace Remora.Commands.Tokenization
{
    /// <summary>
    /// Holds a set of valid start and end quotation pairs.
    /// </summary>
    public static class Quotations
    {
        /// <summary>
        /// Gets the quotation pairs.
        /// </summary>
        public static IReadOnlyList<(string Start, string End)> Pairs { get; } = new List<(string Start, string End)>
        {
            ("\"", "\""),
            ("'", "'")
        };
    }
}
