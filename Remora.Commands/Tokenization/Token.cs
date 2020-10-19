//
//  Token.cs
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

namespace Remora.Commands.Tokenization
{
    /// <summary>
    /// Represents a single token in a sequence.
    /// </summary>
    [PublicAPI]
    public readonly ref struct Token
    {
        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets the value that the token encompasses.
        /// </summary>
        public ReadOnlySpan<char> Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> struct.
        /// </summary>
        /// <param name="type">The type of the token.</param>
        /// <param name="value">The value that the token encompasses.</param>
        public Token(TokenType type, ReadOnlySpan<char> value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}
