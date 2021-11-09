//
//  StringEnumConverter.cs
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
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Discord.API.Extensions;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts enum values to or from JSON.
/// </summary>
/// <typeparam name="TEnum">The enum to read.</typeparam>
[PublicAPI]
public class StringEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, string> _enumsToNames;
    private readonly Dictionary<string, TEnum> _namesToEnums;

    private readonly bool _asInteger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumConverter{TEnum}"/> class.
    /// </summary>
    /// <param name="namingPolicy">The naming policy to use.</param>
    /// <param name="asInteger">Whether to convert the value as a string-serialized integer.</param>
    public StringEnumConverter
    (
        JsonNamingPolicy? namingPolicy = null,
        bool asInteger = false
    )
    {
        _enumsToNames = new Dictionary<TEnum, string>();
        _namesToEnums = new Dictionary<string, TEnum>();

        _asInteger = asInteger;

        foreach (var value in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
        {
            var name = namingPolicy?.ConvertName(value.ToString()) ?? value.ToString();

            _enumsToNames.Add(value, name);
            _namesToEnums.Add(name, value);
        }
    }

    /// <inheritdoc />
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        TEnum result;

        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                var value = reader.GetString();
                if (value is null)
                {
                    throw new JsonException();
                }

                if (Enum.TryParse(value, out result))
                {
                    break;
                }

                if (!_namesToEnums.TryGetValue(value, out result))
                {
                    var caseInsensitiveKey = _namesToEnums.Keys.FirstOrDefault
                    (
                        s => s.Equals(value, StringComparison.OrdinalIgnoreCase)
                    );

                    if (caseInsensitiveKey is null)
                    {
                        throw new JsonException("Failed to deserialize an enumeration value.");
                    }

                    result = _namesToEnums[caseInsensitiveKey];
                }

                break;
            }
            default:
            {
                throw new JsonException("Invalid type for enum deserialization.");
            }
        }

        if (!reader.IsFinalBlock && !reader.Read())
        {
            throw new JsonException();
        }

        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (_asInteger)
        {
            writer.WriteStringValue(Enum.GetUnderlyingType(typeof(TEnum)).IsUnsigned()
                ? Convert.ToUInt64(value).ToString()
                : Convert.ToInt64(value).ToString());

            return;
        }

        writer.WriteStringValue(_enumsToNames[value]);
    }
}
