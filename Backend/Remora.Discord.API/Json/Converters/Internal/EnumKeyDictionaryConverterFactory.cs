//
//  EnumKeyDictionaryConverterFactory.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using System.Text.Json.Serialization;
using Remora.Rest.Json;

namespace Remora.Discord.API.Json;

/// <summary>
/// Creates a JSON converter for dictionaries with enum keys.
/// </summary>
public class EnumKeyDictionaryConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeof(IReadOnlyDictionary<,>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())
                                       && typeToConvert.GetGenericArguments()[0].IsEnum;

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance
           (
               typeof(EnumKeyDictionaryConverterInner<,>).MakeGenericType
               (
                   typeToConvert.GetGenericArguments()[0],
                   typeToConvert.GetGenericArguments()[1]
               ),
               options
           );

    /// <inheritdoc />
    internal class EnumKeyDictionaryConverterInner<TEnumKey, TValue> : JsonConverter<IReadOnlyDictionary<TEnumKey, TValue>>
        where TEnumKey : struct, Enum
    {
        private readonly JsonConverter<TValue> _valueConverter;
        private readonly JsonConverter<TEnumKey> _keyConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumKeyDictionaryConverterInner{TEnumKey, TValue}"/> class.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        public EnumKeyDictionaryConverterInner(JsonSerializerOptions options)
        {
            _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
            _keyConverter = new StringEnumConverter<TEnumKey>(asInteger: true);
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<TEnumKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = new Dictionary<TEnumKey, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var keyString = reader.GetString() ?? throw new JsonException("Dictionaries cannot contain null keys.");
                var parsedKey = Enum.TryParse<TEnumKey>(keyString, out var keyValue);

                if (!parsedKey)
                {
                    throw new JsonException($"Could not parse key '{keyString}' as enum of type {typeof(TEnumKey).Name}.");
                }

                if (!reader.Read())
                {
                    throw new JsonException();
                }

                var value = _valueConverter.Read(ref reader, typeof(TValue), options);

                result.Add(keyValue, value!);
            }

            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<TEnumKey, TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var (key, val) in value)
            {
                _keyConverter.WriteAsPropertyName(writer, key, options);
                _valueConverter.Write(writer, val, options);
            }

            writer.WriteEndObject();
        }
    }
}
