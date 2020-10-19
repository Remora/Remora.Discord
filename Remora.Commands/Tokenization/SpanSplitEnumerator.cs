//
//  SpanSplitEnumerator.cs
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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remora.Commands.Tokenization
{
    /// <summary>
    /// Enumerates a <see cref="ReadOnlySpan{T}"/> containing <see cref="char"/>s, splitting it into parts based on
    /// a delimiter.
    /// </summary>
    /// <remarks>
    /// This type serves a slightly more specific purpose than the ostensibly equivalent
    /// <see cref="string.Split(char, StringSplitOptions)"/> method, in that it mimics a command-line string splitting
    /// function by default - that is, it respects quotations and discards empty results.
    /// </remarks>
    [PublicAPI]
    public ref struct SpanSplitEnumerator
    {
        private static readonly IReadOnlyList<(string Start, string End)> Quotations = new List<(string Start, string End)>
        {
            ("\"", "\""),
            ("'", "'")
        };

        private readonly ReadOnlySpan<char> _delimiter;
        private readonly bool _ignoreEmpty;

        private ReadOnlySpan<char> _value;
        private ReadOnlySpan<char> _current;

        /// <summary>
        /// Gets the current value of the enumerator.
        /// </summary>
        public readonly ReadOnlySpan<char> Current => _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanSplitEnumerator"/> struct.
        /// </summary>
        /// <param name="value">The value to split.</param>
        /// <param name="delimiter">The delimiter to split on.</param>
        /// <param name="ignoreEmpty">Whether empty results should be discarded.</param>
        public SpanSplitEnumerator(ReadOnlySpan<char> value, ReadOnlySpan<char> delimiter, bool ignoreEmpty = true)
        {
            _value = value;
            _delimiter = delimiter;
            _ignoreEmpty = ignoreEmpty;

            _current = default;
        }

        /// <summary>
        /// Gets the enumerator for this type.
        /// </summary>
        /// <returns>The instance itself.</returns>
        public SpanSplitEnumerator GetEnumerator() => this;

        /// <summary>
        /// Attempts to advance the enumerator.
        /// </summary>
        /// <returns>true if the enumerator advanced and a new value is available; otherwise, false.</returns>
        public bool MoveNext()
        {
            while (true)
            {
                var span = _value;

                if (span.Length == 0)
                {
                    // No more content
                    return false;
                }

                var index = span.IndexOf(_delimiter);
                if (index == -1)
                {
                    _value = ReadOnlySpan<char>.Empty;
                    _current = span;

                    // Everything that remains is one value
                    return true;
                }

                var segment = span.Slice(0, index);

                var continuationIndex = Math.Clamp(index + _delimiter.Length, 0, span.Length);
                var remainder = span.Slice(continuationIndex);

                // Check if we're trying to read a quoted value
                var foundStartQuote = false;
                var foundEndQuote = false;
                foreach (var (start, end) in Quotations)
                {
                    if (!segment.Contains(start, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    foundStartQuote = true;

                    var closingIndex = remainder.IndexOf(end);
                    if (closingIndex < 0)
                    {
                        continue;
                    }

                    segment = span.Slice(0, continuationIndex + closingIndex + 1);
                    remainder = span.Slice(continuationIndex + closingIndex + 1);

                    foundEndQuote = true;
                    break;
                }

                if (foundStartQuote && !foundEndQuote)
                {
                    // Read to end if we can't find a matching quote
                    _current = _value;
                    _value = ReadOnlySpan<char>.Empty;

                    return true;
                }

                _current = segment;
                _value = remainder;

                if (this.Current.Length == 0 && _ignoreEmpty)
                {
                    continue;
                }

                return true;
            }
        }
    }
}
