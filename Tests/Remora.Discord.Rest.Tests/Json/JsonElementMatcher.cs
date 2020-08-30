//
//  JsonElementMatcher.cs
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
    /// Matches against Json elements.
    /// </summary>
    public class JsonElementMatcher
    {
        private readonly IReadOnlyList<Func<JsonElement, bool>> _matchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonElementMatcher"/> class.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        public JsonElementMatcher(IReadOnlyList<Func<JsonElement, bool>> matchers)
        {
            _matchers = matchers;
        }

        /// <summary>
        /// Determines whether the matcher fully matches the given element.
        /// </summary>
        /// <param name="elementEnumerator">The element enumerator.</param>
        /// <returns>Whether the matcher matches the element.</returns>
        public bool Matches(JsonElement elementEnumerator)
        {
            return _matchers.All(m => m(elementEnumerator));
        }
    }
}
