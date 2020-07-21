//
//  JsonAssert.cs
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
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Remora.Discord.Tests
{
    /// <summary>
    /// Defines various additional assertions for xUnit.
    /// </summary>
    public static class JsonAssert
    {
        /// <summary>
        /// Asserts that the given <see cref="JsonDocument"/> values are equivalent, that is, ordering of properties
        /// may differ, but the logical contents must be the same.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        public static void Equivalent(JsonDocument expected, JsonDocument actual)
            => Equivalent(expected.RootElement, actual.RootElement);

        /// <summary>
        /// Asserts that the given <see cref="JsonDocument"/> values are equivalent, that is, ordering of properties
        /// in an object may differ, but the logical contents must be the same. Arrays must equal in both count,
        /// elements, and order.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        public static void Equivalent(JsonElement expected, JsonElement actual)
        {
            Assert.Equal(expected.ValueKind, actual.ValueKind);

            switch (expected.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    var actualElements = actual.EnumerateObject().ToList();
                    var expectedElements = expected.EnumerateObject().ToList();

                    Assert.Equal(expectedElements.Count, actualElements.Count);

                    foreach (var expectedElement in expectedElements)
                    {
                        var matchingElement = actualElements.First(ae => ae.NameEquals(expectedElement.Name));
                        Equivalent(expectedElement.Value, matchingElement.Value);
                    }

                    break;
                }
                case JsonValueKind.Array:
                {
                    var actualElements = expected.EnumerateArray().ToList();
                    var expectedElements = actual.EnumerateArray().ToList();

                    Assert.Equal(expectedElements.Count, actualElements.Count);

                    for (var i = 0; i < expectedElements.Count; ++i)
                    {
                        Equivalent(expectedElements[i], actualElements[i]);
                    }

                    break;
                }
                case JsonValueKind.String:
                {
                    Assert.Equal(expected.GetString(), actual.GetString());
                    break;
                }
                case JsonValueKind.Number:
                {
                    Assert.Equal(expected.GetDouble(), actual.GetDouble());
                    break;
                }
                case JsonValueKind.True:
                case JsonValueKind.False:
                {
                    Assert.Equal(expected.GetBoolean(), actual.GetBoolean());
                    break;
                }
                case JsonValueKind.Null:
                {
                    // Equal by definition
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
