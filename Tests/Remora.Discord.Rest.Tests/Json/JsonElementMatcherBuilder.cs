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
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;

namespace Remora.Discord.Rest.Tests.Json
{
    /// <summary>
    /// Builds instances of the <see cref="JsonElementMatcher"/> class.
    /// </summary>
    [PublicAPI]
    public class JsonElementMatcherBuilder
    {
        private readonly List<Func<JsonElement, bool>> _matchers = new();

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
        /// Adds a requirement that the element should be a null value.
        /// </summary>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder IsNull()
        {
            _matchers.Add(j => j.ValueKind == JsonValueKind.Null);
            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a boolean.
        /// </summary>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder IsBoolean()
        {
            _matchers.Add
            (
                j => j.ValueKind is JsonValueKind.True or JsonValueKind.False
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a number.
        /// </summary>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder IsNumber()
        {
            _matchers.Add
            (
                j => j.ValueKind == JsonValueKind.Number
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a boolean.
        /// </summary>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder IsString()
        {
            _matchers.Add
            (
                j => j.ValueKind == JsonValueKind.String
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be an 8-bit integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(sbyte value)
        {
            _matchers.Add
            (
                j => j.TryGetSByte(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 16-bit integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(short value)
        {
            _matchers.Add
            (
                j => j.TryGetInt16(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 32-bit integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(int value)
        {
            _matchers.Add
            (
                j => j.TryGetInt32(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 64-bit integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(long value)
        {
            _matchers.Add
            (
                j => j.TryGetInt64(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be an 8-bit unsigned integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(byte value)
        {
            _matchers.Add
            (
                j => j.TryGetByte(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 16-bit unsigned integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(ushort value)
        {
            _matchers.Add
            (
                j => j.TryGetUInt16(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 32-bit unsigned integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(uint value)
        {
            _matchers.Add
            (
                j => j.TryGetUInt32(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 64-bit unsigned integer with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(ulong value)
        {
            _matchers.Add
            (
                j => j.TryGetUInt64(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a string with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(string value)
        {
            _matchers.Add
            (
                j => j.ValueKind == JsonValueKind.String && j.GetString() == value
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a boolean with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(bool value)
        {
            _matchers.Add(j =>
            {
                if (value)
                {
                    return j.ValueKind == JsonValueKind.True;
                }

                return j.ValueKind == JsonValueKind.False;
            });

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a decimal with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(decimal value)
        {
            _matchers.Add
            (
                j => j.TryGetDecimal(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 32-bit floating point value with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(float value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            _matchers.Add
            (
                j => j.TryGetSingle(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a 64-bit floating point value with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(double value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            _matchers.Add
            (
                j => j.TryGetDouble(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a <see cref="Guid"/>-representable string with the given
        /// value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(Guid value)
        {
            _matchers.Add
            (
                j => j.TryGetGuid(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a <see cref="DateTime"/> with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(DateTime value)
        {
            _matchers.Add
            (
                j => j.TryGetDateTime(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a <see cref="DateTimeOffset"/> with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(DateTimeOffset value)
        {
            _matchers.Add
            (
                j => j.TryGetDateTimeOffset(out var actualValue) && value == actualValue
            );

            return this;
        }

        /// <summary>
        /// Adds a requirement that the element should be a base64-encoded byte array with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The builder, with the requirement added.</returns>
        public JsonElementMatcherBuilder Is(IEnumerable<byte> value)
        {
            _matchers.Add
            (
                j => j.TryGetBytesFromBase64(out var actualValue) && value.SequenceEqual(actualValue)
            );

            return this;
        }

        /// <summary>
        /// Builds the element matcher.
        /// </summary>
        /// <returns>The built element matcher.</returns>
        public JsonElementMatcher Build()
        {
            return new(_matchers);
        }
    }
}
