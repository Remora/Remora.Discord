//
//  JsonArrayMatcherBuilder.cs
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
using System.Linq;
using System.Text.Json;

namespace Remora.Discord.Rest.Tests.Json
{
    /// <summary>
    /// Builds instances of the <see cref="JsonArrayMatcher"/> class.
    /// </summary>
    public class JsonArrayMatcherBuilder
    {
        private readonly List<Func<JsonElement.ArrayEnumerator, bool>> _matchers
            = new List<Func<JsonElement.ArrayEnumerator, bool>>();

        /// <summary>
        /// Adds a requirement that the array is of an exact length.
        /// </summary>
        /// <param name="countPredicate">The function of the required length.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithCount(Func<long, bool> countPredicate)
        {
            _matchers.Add(j => countPredicate(j.LongCount()));

            return this;
        }

        /// <summary>
        /// Adds a requirement that the array is of an exact length.
        /// </summary>
        /// <param name="count">The required length.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithCount(long count) => WithCount(c => c == count);

        /// <summary>
        /// Adds a requirement that the array is of an exact length.
        /// </summary>
        /// <param name="count">The required length.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithAtLeastCount(long count) => WithCount(c => c >= count);

        /// <summary>
        /// Adds a requirement that the array is of an exact length.
        /// </summary>
        /// <param name="count">The required length.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithNoMoreThanCount(long count) => WithCount(c => c <= count);

        /// <summary>
        /// Adds a requirement that any element matches the given element builder.
        /// </summary>
        /// <param name="elementMatcherBuilder">The element matcher.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithAnyElement(Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null)
        {
            var elementMatcher = new JsonElementMatcherBuilder();
            elementMatcherBuilder?.Invoke(elementMatcher);

            var matcher = elementMatcher.Build();
            _matchers.Add(j => j.Any(matcher.Matches));

            return this;
        }

        /// <summary>
        /// Adds a requirement that a single element matches the given element builder.
        /// </summary>
        /// <param name="elementMatcherBuilder">The element matcher.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithSingleElement(Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null)
        {
            var elementMatcher = new JsonElementMatcherBuilder();
            elementMatcherBuilder?.Invoke(elementMatcher);

            var matcher = elementMatcher.Build();
            _matchers.Add(j => j.Count(matcher.Matches) == 1);

            return this;
        }

        /// <summary>
        /// Adds a requirement that an element at the given index matches the given element builder.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <param name="elementMatcherBuilder">The element matcher.</param>
        /// <returns>The builder, with the added requirement.</returns>
        public JsonArrayMatcherBuilder WithElement
        (
            int index,
            Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
        )
        {
            var elementMatcher = new JsonElementMatcherBuilder();
            elementMatcherBuilder?.Invoke(elementMatcher);

            var matcher = elementMatcher.Build();
            _matchers.Add(j => j.Count() > index && matcher.Matches(j.ElementAt(index)));

            return this;
        }

        /// <summary>
        /// Builds the array matcher.
        /// </summary>
        /// <returns>The built array matcher.</returns>
        public JsonArrayMatcher Build()
        {
            return new JsonArrayMatcher(_matchers);
        }
    }
}
