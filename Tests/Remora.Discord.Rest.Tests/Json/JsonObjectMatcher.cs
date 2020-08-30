//
//  JsonObjectMatcher.cs
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
    /// Matches against Json objects.
    /// </summary>
    public class JsonObjectMatcher
    {
        private readonly IReadOnlyList<Func<JsonElement, bool>> _matchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonObjectMatcher"/> class.
        /// </summary>
        /// <param name="matchers">The matchers.</param>
        public JsonObjectMatcher(IReadOnlyList<Func<JsonElement, bool>> matchers)
        {
            _matchers = matchers;
        }

        /// <summary>
        /// Determines whether the matcher fully matches the given object.
        /// </summary>
        /// <param name="jsonObject">The json object.</param>
        /// <returns>Whether the matcher matches the object.</returns>
        public bool Matches(JsonElement jsonObject)
        {
            return _matchers.All(m => m(jsonObject));
        }
    }
}
