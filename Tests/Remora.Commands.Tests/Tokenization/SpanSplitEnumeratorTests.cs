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

using System.Collections.Generic;
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
        /// <param name="expectedElements">The expected number of elements.</param>
        [Theory]
        [InlineData("", new string[] { })]
        [InlineData("a", new[] { "a" })]
        [InlineData("a b", new[] { "a", "b" })]
        [InlineData("a b c", new[] { "a", "b", "c" })]
        [InlineData("abc def", new[] { "abc", "def" })]
        [InlineData("\"abc\"", new[] { "\"abc\"" })]
        [InlineData("\"abc def\"", new[] { "\"abc def\"" })]
        [InlineData("\"abc\" \"def\"", new[] { "\"abc\"", "\"def\"" })]
        [InlineData("\"abc\"    \"def\"", new[] { "\"abc\"", "\"def\"" })]
        [InlineData("\"abc\"\"def\"", new[] { "\"abc\"", "\"def\"" })]
        [InlineData("aa \"abc\"\"def\"", new[] { "aa", "\"abc\"", "\"def\"" })]
        [InlineData("\"abc\" \"def\" ghi jkl", new[] { "\"abc\"", "\"def\"", "ghi", "jkl" })]
        [InlineData("\"abc\"\"def\" ghi jkl", new[] { "\"abc\"", "\"def\"", "ghi", "jkl" })]
        [InlineData("\"abc\"\"def\"ghi jkl", new[] { "\"abc\"", "\"def\"", "ghi", "jkl" })]
        [InlineData(" ", new string[] { })]
        [InlineData("  ", new string[] { })]
        [InlineData(" a ", new[] { "a" })]
        [InlineData(" a b", new[] { "a", "b" })]
        [InlineData(" a b ", new[] { "a", "b" })]
        [InlineData(" a b     ", new[] { "a", "b" })]
        [InlineData("      a b     ", new[] { "a", "b" })]
        [InlineData("test -b=aa --aaaagh 10 \"booga wooga\"", new[] { "test", "-b=aa", "--aaaagh", "10", "\"booga wooga\"" })]
        [InlineData("--b=\"booga wooga\"", new[] { "--b=\"booga wooga\"" })]
        [InlineData("\"aaaasdasd done oops i forgot my end quote", new[] { "\"aaaasdasd done oops i forgot my end quote" })]
        [InlineData("a \"b c", new[] { "a", "\"b c" })]
        [InlineData("\"a", new[] { "\"a" })]
        public void SplitsStringIntoCorrectElements(string value, IEnumerable<string> expectedElements)
        {
            var actualElements = new List<string>();
            foreach (var segment in new SpanSplitEnumerator(value, " "))
            {
                actualElements.Add(segment.ToString());
            }

            Assert.Equal(expectedElements, actualElements);
        }
    }
}
