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

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts instances of <see cref="IOneOf"/> to and from JSON.
    /// </summary>
    /// <typeparam name="TOneOf">The OneOf type.</typeparam>
    public class OneOfConverter<TOneOf> : JsonConverter<TOneOf>
        where TOneOf : IOneOf
    {
        /// <summary>
        /// Holds all types that are a member of the union.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyList<Type> UnionMemberTypes;

        /// <summary>
        /// Holds all member types that are builtin C# types.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyList<Type> BuiltinUnionMemberTypes;

        /// <summary>
        /// Holds all member types that are builtin numeric C# types.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyList<Type> NumericUnionMemberTypes;

        /// <summary>
        /// Holds all member types that are collection types.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyList<Type> CollectionUnionMemberTypes;

        /// <summary>
        /// Holds all other member types.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyList<Type> OtherUnionMemberTypes;

        /// <summary>
        /// Holds a mapping between the member types and the FromT methods of the union.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IReadOnlyDictionary<Type, MethodInfo> FromValueMethods;

        static OneOfConverter()
        {
            Type unionType = typeof(TOneOf);
            UnionMemberTypes = unionType.GetGenericArguments();

            BuiltinUnionMemberTypes = UnionMemberTypes.Where(t => t.IsBuiltin()).ToList();
            NumericUnionMemberTypes = UnionMemberTypes.Where(t => t.IsNumeric()).ToList();
            CollectionUnionMemberTypes = UnionMemberTypes.Where(t => t.IsCollection()).ToList();
            OtherUnionMemberTypes = UnionMemberTypes
                .Except(BuiltinUnionMemberTypes)
                .Except(CollectionUnionMemberTypes)
                .ToList();

            var fromValueMethods = new Dictionary<Type, MethodInfo>();
            for (var i = 0; i < UnionMemberTypes.Count; ++i)
            {
                var methodInfo = unionType.GetMethod($"FromT{i}");
                if (methodInfo is null)
                {
                    throw new InvalidOperationException();
                }

                fromValueMethods.Add(UnionMemberTypes[i], methodInfo);
            }

            FromValueMethods = fromValueMethods;
        }

        /// <inheritdoc />
        public override TOneOf? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TOneOf? result;

            var checkedTypes = new List<Type>();
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    if (BuiltinUnionMemberTypes.Contains(typeof(string)))
                    {
                        var method = FromValueMethods[typeof(string)];

                        var value = reader.GetString() ?? throw new InvalidOperationException();
                        return (TOneOf)method.Invoke(null, new object[] { value });
                    }

                    break;
                }
                case JsonTokenType.True:
                case JsonTokenType.False:
                {
                    if (BuiltinUnionMemberTypes.Contains(typeof(bool)))
                    {
                        var method = FromValueMethods[typeof(bool)];

                        var value = reader.GetBoolean();
                        return (TOneOf)method.Invoke(null, new object[] { value });
                    }

                    break;
                }
                case JsonTokenType.Number:
                {
                    if (TryCreateOneOf(ref reader, NumericUnionMemberTypes, options, out result))
                    {
                        return result;
                    }

                    checkedTypes.AddRange(NumericUnionMemberTypes);
                    break;
                }
                case JsonTokenType.StartObject:
                {
                    if (TryCreateOneOf(ref reader, OtherUnionMemberTypes, options, out result))
                    {
                        return result;
                    }

                    checkedTypes.AddRange(OtherUnionMemberTypes);
                    break;
                }
                case JsonTokenType.StartArray:
                {
                    if (TryCreateOneOf(ref reader, CollectionUnionMemberTypes, options, out result))
                    {
                        return result;
                    }

                    checkedTypes.AddRange(CollectionUnionMemberTypes);
                    break;
                }
            }

            // Still nothing? We'll try to bruteforce it
            var remainingTypes = UnionMemberTypes.Except(checkedTypes);
            if (TryCreateOneOf(ref reader, remainingTypes, options, out result))
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

                oneOf = (TOneOf)method.Invoke(null, new[] { value });
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TOneOf value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
        }
    }
}
