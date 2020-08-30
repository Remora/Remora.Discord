//
//  JsonElementMatcherBuilder.cs
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
using System.Text.Json;

namespace Remora.Discord.Rest.Tests.Json
{
    /// <summary>
    /// Builds instances of the <see cref="JsonElementMatcher"/> class.
    /// </summary>
    public class JsonElementMatcherBuilder
    {
        private readonly List<Func<JsonElement, bool>> _matchers
            = new List<Func<JsonElement, bool>>();

        /// <summary>
        /// Adds a requirement that the element should be an object, with optional additional requirements.
        /// </summary>
        /// <param name="objectMatcherBuilder">The optional requirement builder.</param>
        /// <returns>The request matcher builder, with the requirements added.</returns>
        public JsonElementMatcherBuilder IsObject(Action<JsonObjectMatcherBuilder>? objectMatcherBuilder = null)
        {
            _matchers.Add
            (
                j =>
                {
                    var isObject = j.ValueKind == JsonValueKind.Object;
                    if (objectMatcherBuilder is null)
                    {
                        return isObject;
                    }

                    var objectMatcher = new JsonObjectMatcherBuilder();
                    objectMatcherBuilder(objectMatcher);

                    return isObject && objectMatcher.Build().Matches(j);
                }
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be an array, with optional additional requirements.
        /// </summary>
        /// <param name="arrayMatcherBuilder">The optional requirement builder.</param>
        /// <returns>The request matcher builder, with the requirements added.</returns>
        public JsonElementMatcherBuilder IsArray(Action<JsonArrayMatcherBuilder>? arrayMatcherBuilder = null)
        {
            _matchers.Add
            (
                j =>
                {
                    var isArray = j.ValueKind == JsonValueKind.Array;
                    if (arrayMatcherBuilder is null)
                    {
                        return isArray;
                    }

                    var arrayMatcher = new JsonArrayMatcherBuilder();
                    arrayMatcherBuilder(arrayMatcher);

                    return isArray && arrayMatcher.Build().Matches(j.EnumerateArray());
                }
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a value, with optional additional requirements.
        /// </summary>
        /// <param name="valueKind">The type of value the element should be.</param>
        /// <param name="valueMatcher">The optional requirement.</param>
        /// <returns>The request matcher builder, with the requirements added.</returns>
        public JsonElementMatcherBuilder IsValue(JsonValueKind valueKind, Func<JsonElement, bool>? valueMatcher = null)
        {
            _matchers.Add
            (
                j =>
                {
                    var isCorrectKind = j.ValueKind == valueKind;
                    if (valueMatcher is null)
                    {
                        return isCorrectKind;
                    }

                    return isCorrectKind && valueMatcher(j);
                }
            );

            return this;
        }

        /// <summary>
        /// Builds the element matcher.
        /// </summary>
        /// <returns>The built element matcher.</returns>
        public JsonElementMatcher Build()
        {
            return new JsonElementMatcher(_matchers);
        }
    }
}
