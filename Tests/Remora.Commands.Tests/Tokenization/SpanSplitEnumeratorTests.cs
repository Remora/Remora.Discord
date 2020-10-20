//
//  SpanSplitEnumeratorTests.cs
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

using Remora.Commands.Tokenization;
using Xunit;

namespace Remora.Commands.Tests.Tokenization
{
    /// <summary>
    /// Tests the <see cref="SpanSplitEnumerator"/> class.
    /// </summary>
    public class SpanSplitEnumeratorTests
    {
        /// <summary>
        /// Tests whether the enumerator splits an empty string into no constituent parts.
        /// </summary>
        [Fact]
        public void SplitsEmptyStringsIntoNothing()
        {
            var value = string.Empty;
            var enumerator = new SpanSplitEnumerator(value, " ");

            Assert.False(enumerator.MoveNext());
        }

        /// <summary>
        /// Tests whether the enumerator splits a single word into one part.
        /// </summary>
        [Fact]
        public void SplitsSingleWordIntoSingleSegment()
        {
            var value = "bunt";
            var enumerator = new SpanSplitEnumerator(value, " ");

            Assert.True(enumerator.MoveNext());
            Assert.Equal(value, enumerator.Current.ToString());
            Assert.False(enumerator.MoveNext());
        }

        /// <summary>
        /// Tests whether the enumerator splits a single word into one part.
        /// </summary>
        [Fact]
        public void SplitsMultipleWordsIntoMultipleSegments()
        {
            var value = "a b c d e f g";
            var enumerator = new SpanSplitEnumerator(value, " ");

            for (var i = 0; i < 7; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(value[i * 2].ToString(), enumerator.Current.ToString());
            }

            Assert.False(enumerator.MoveNext());
        }

        /// <summary>
        /// Tests whether the enumerator splits a single term into one part.
        /// </summary>
        [Fact]
        public void SplitsSingleQuotedTermIntoSingleSegment()
        {
            var value = "\"one term\"";
            var enumerator = new SpanSplitEnumerator(value, " ");

            Assert.True(enumerator.MoveNext());
            Assert.Equal(value, enumerator.Current.ToString());
            Assert.False(enumerator.MoveNext());
        }

        /// <summary>
        /// Tests whether the enumerator splits values into the correct number of elements.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="expectedCount">The expected number of elements.</param>
        [Theory]
        [InlineData("", 0)]
        [InlineData("a", 1)]
        [InlineData("a b", 2)]
        [InlineData("a b c", 3)]
        [InlineData("abc def", 2)]
        [InlineData("\"abc\"", 1)]
        [InlineData("\"abc def\"", 1)]
        [InlineData("\"abc\" \"def\"", 2)]
        [InlineData("\"abc\"    \"def\"", 2)]
        [InlineData("\"abc\"\"def\"", 2)]
        [InlineData("aa \"abc\"\"def\"", 3)]
        [InlineData("\"abc\" \"def\" ghi jkl", 4)]
        [InlineData("\"abc\"\"def\" ghi jkl", 4)]
        [InlineData("\"abc\"\"def\"ghi jkl", 4)]
        [InlineData(" ", 0)]
        [InlineData("  ", 0)]
        [InlineData(" a ", 1)]
        [InlineData(" a b", 2)]
        [InlineData(" a b ", 2)]
        [InlineData(" a b     ", 2)]
        [InlineData("      a b     ", 2)]
        [InlineData("test -b=aa --aaaagh 10 \"booga wooga\"", 5)]
        public void SplitsStringIntoCorrectNumberOfElements(string value, int expectedCount)
        {
            var enumerator = new SpanSplitEnumerator(value, " ");

            var actualCount = 0;
            while (enumerator.MoveNext())
            {
                ++actualCount;
            }

            Assert.Equal(expectedCount, actualCount);
        }
    }
}
