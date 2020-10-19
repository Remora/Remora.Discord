//
//  TokenType.cs
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

namespace Remora.Commands.Tokenization
{
    /// <summary>
    /// Enumerates token types.
    /// </summary>
    [PublicAPI]
    public enum TokenType
    {
        /// <summary>
        /// The token is a value.
        /// </summary>
        Value,

        /// <summary>
        /// The token is the long name of an option.
        /// </summary>
        LongName,

        /// <summary>
        /// The token is the short name of an option.
        /// </summary>
        ShortName
    }
}
