//
//  TokenizingEnumeratorTests.cs
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
using Remora.Commands.Tokenization;
using Xunit;
using static Remora.Commands.Tokenization.TokenType;

namespace Remora.Commands.Tests.Tokenization
{
    /// <summary>
    /// Tests the <see cref="TokenizingEnumeratorTests"/> class.
    /// </summary>
    public class TokenizingEnumeratorTests
    {
        /// <summary>
        /// Tests whether the enumerator tokenizes values into the correct token sequences.
        /// </summary>
        /// <param name="value">The value to tokenize.</param>
        /// <param name="expectedTokenTypes">The type sequence to expect.</param>
        [Theory]
        [InlineData("", new TokenType[] { })]
        [InlineData("a", new[] { Value })]
        [InlineData("a b", new[] { Value, Value })]
        [InlineData("-a b", new[] { ShortName, Value })]
        [InlineData("-a=b", new[] { ShortName, Value })]
        [InlineData("--a b", new[] { LongName, Value })]
        [InlineData("--a=b", new[] { LongName, Value })]
        [InlineData("-a -b", new[] { ShortName, ShortName })]
        [InlineData("--a --b", new[] { LongName, LongName })]
        [InlineData("--a --b c", new[] { LongName, LongName, Value })]
        [InlineData("--b \"booga wooga\"", new[] { LongName, Value })]
        [InlineData("--b=\"booga wooga\"", new[] { LongName, Value })]
        [InlineData("aa bb --b=\"booga wooga\"", new[] { Value, Value, LongName, Value })]
        public void TokenizesStringCorrectly(string value, IEnumerable<TokenType> expectedTokenTypes)
        {
            var actualTokenTypes = new List<TokenType>();
            foreach (var token in new TokenizingEnumerator(value))
            {
                actualTokenTypes.Add(token.Type);
            }

            Assert.Equal(expectedTokenTypes, actualTokenTypes);
        }
    }
}
