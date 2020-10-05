//
//  JsonObjectMatcherBuilder.cs
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
    /// Builds instances of the <see cref="JsonObjectMatcher"/> class.
    /// </summary>
    public class JsonObjectMatcherBuilder
    {
        private readonly List<Func<JsonElement, bool>> _matchers
            = new List<Func<JsonElement, bool>>();

        /// <summary>
        /// Adds a requirement that a given property should exist.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="elementMatcherBuilder">The additional requirements on the property value.</param>
        /// <returns>The builder, with the requirement.</returns>
        public JsonObjectMatcherBuilder WithProperty
        (
            string name,
            Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
        )
        {
            _matchers.Add
            (
                obj =>
                {
                    if (!obj.TryGetProperty(name, out var property))
                    {
                        throw new Xunit.Sdk.ContainsException(name, obj);
                    }

                    if (elementMatcherBuilder is null)
                    {
                        return true;
                    }

                    var matcherBuilder = new JsonElementMatcherBuilder();
                    elementMatcherBuilder(matcherBuilder);

                    var valueMatches = matcherBuilder.Build().Matches(property);
                    if (!valueMatches)
                    {
                        throw new Xunit.Sdk.TrueException($"The value of \"{name}\" did not match.", valueMatches);
                    }

                    return valueMatches;
                }
            );

            return this;
        }

        /// <summary>
        /// Builds the object matcher.
        /// </summary>
        /// <returns>The built object matcher.</returns>
        public JsonObjectMatcher Build()
        {
            return new JsonObjectMatcher(_matchers);
        }
    }
}
