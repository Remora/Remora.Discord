//
//  OneOfConverter.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;
using Remora.Discord.API.Extensions;
using Remora.Discord.Core;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts instances of <see cref="IOneOf"/> to and from JSON.
    /// </summary>
    /// <typeparam name="TOneOf">The OneOf type.</typeparam>
    internal class OneOfConverter<TOneOf> : JsonConverter<TOneOf>
        where TOneOf : IOneOf
    {
        /// <summary>
        /// Holds the member types, sorted in the order they should be attempted to be deserialized.
        /// </summary>
        /// <remarks>
        /// The order is constructed to produce correct results in as many cases as possible, giving leeway to the fact
        /// that multiple type parsers can take primitive elements from JSON as an input. A typical example of this is
        /// integers vs <see cref="Snowflake"/>.
        ///
        /// The order is as follows:
        ///   * Numeric C# types
        ///   * Collection types
        ///   * Complex types (classes, records, etc)
        ///   * Builtin C# types (string, etc)
        ///
        /// Hopefully, this works for most cases.
        /// </remarks>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyList<Type> OrderedUnionMemberTypes;

        /// <summary>
        /// Holds a mapping between the member types and the FromT methods of the union.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyDictionary<Type, MethodInfo> FromValueMethods;

        static OneOfConverter()
        {
            Type unionType = typeof(TOneOf);
            var unionMemberTypes = unionType.GetGenericArguments();

            OrderedUnionMemberTypes = unionMemberTypes
                .OrderByDescending(t => t.IsNumeric())
                .ThenByDescending(t => t.IsCollection())
                .ThenBy(t => t.IsBuiltin())
                .ToList();

            var fromValueMethods = new Dictionary<Type, MethodInfo>();
            for (var i = 0; i < unionMemberTypes.Length; ++i)
            {
                var methodInfo = unionType.GetMethod($"FromT{i}");
                if (methodInfo is null)
                {
                    throw new InvalidOperationException();
                }

                fromValueMethods.Add(unionMemberTypes[i], methodInfo);
            }

            FromValueMethods = fromValueMethods;
        }

        /// <inheritdoc />
        public override TOneOf Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (TryCreateOneOf(ref reader, OrderedUnionMemberTypes, options, out var result))
            {
                return result;
            }

            throw new JsonException("Could not parse value as a member of the union.");
        }

        private static bool TryCreateOneOf
        (
            ref Utf8JsonReader reader,
            IEnumerable<Type> types,
            JsonSerializerOptions options,
            [NotNullWhen(true)] out TOneOf? oneOf
        )
        {
            oneOf = default;

            // This method is used to promote the most likely type to successfully deserialize to the start of the
            // considered types. The original most-likely order is generally preserved, but this increases our chances
            // significantly.
            static bool CouldMatchJson(Type type, JsonTokenType tokenType)
            {
                return tokenType switch
                {
                    JsonTokenType.StartArray => type.IsCollection(),
                    JsonTokenType.String => type == typeof(string),
                    JsonTokenType.Number => type.IsNumeric(),
                    JsonTokenType.True => type == typeof(bool),
                    JsonTokenType.False => type == typeof(bool),
                    JsonTokenType.Null => type.IsNullable(),
                    JsonTokenType.StartObject => !type.IsPrimitive,
                    _ => false
                };
            }

            var currentTokenType = reader.TokenType;
            types = types.OrderByDescending(t => CouldMatchJson(t, currentTokenType));

            // Try deserializing from one of the "other" types
            foreach (var type in types)
            {
                object? value;
                try
                {
                    value = JsonSerializer.Deserialize(ref reader, type, options);
                }
                catch (JsonException)
                {
                    // Pass, we'll try the next one
                    continue;
                }

                // It worked!
                var method = FromValueMethods[type];

                oneOf = (TOneOf)method.Invoke(null, new[] { value })!;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TOneOf value, JsonSerializerOptions options)
        {
            var declaredType = typeof(TOneOf).GetGenericArguments()[value.Index];
            JsonSerializer.Serialize(writer, value.Value, declaredType, options);
        }
    }
}
