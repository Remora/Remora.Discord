//
//  StringEnumListConverter.cs
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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts string array values into corresponding enum lists.
/// </summary>
/// <typeparam name="TEnum">The type of enum values to be converted.</typeparam>
[PublicAPI]
public class StringEnumListConverter<TEnum> : JsonConverter<IReadOnlyList<TEnum>>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumListConverter{TEnum}"/> class.
    /// </summary>
    /// <param name="namingPolicy">The naming policy to be used to translate JSON values into enum values, and vice-versa.</param>
    public StringEnumListConverter(JsonNamingPolicy namingPolicy)
    {
        _enumValuesByJsonValue = new Dictionary<string, TEnum>();
        _jsonValuesByEnumValue = new Dictionary<TEnum, string>();

        foreach (var enumValue in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
        {
            var jsonValue = namingPolicy.ConvertName(enumValue.ToString());

            _enumValuesByJsonValue.Add(jsonValue, enumValue);
            _jsonValuesByEnumValue.Add(enumValue, jsonValue);
        }
    }

    /// <inheritdoc />
    public override IReadOnlyList<TEnum> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var enumValues = new List<TEnum>();

        if (reader.TokenType is not JsonTokenType.StartArray)
        {
            throw new JsonException($"Unexpected token {reader.TokenType}: Expected {nameof(JsonTokenType.StartArray)}");
        }

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    if (_enumValuesByJsonValue.TryGetValue(reader.GetString()!, out var enumValue))
                    {
                        enumValues.Add(enumValue);
                    }
                    break;
                }
                case JsonTokenType.EndArray:
                {
                    return enumValues;
                }
                default:
                {
                    throw new JsonException($"Unexpected token {reader.TokenType}: Expected {nameof(JsonTokenType.String)} or {nameof(JsonTokenType.EndArray)}");
                }
            }
        }

        throw new JsonException("Unexpected end of document");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IReadOnlyList<TEnum> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var enumValue in value)
        {
            if (!_jsonValuesByEnumValue.TryGetValue(enumValue, out var jsonValue))
            {
                throw new ArgumentException($"{enumValue} is not a valid {typeof(TEnum).Name} value");
            }
            writer.WriteStringValue(jsonValue);
        }
        writer.WriteEndArray();
    }

    private readonly Dictionary<string, TEnum> _enumValuesByJsonValue;
    private readonly Dictionary<TEnum, string> _jsonValuesByEnumValue;
}
