//
//  IntegrationTypeConfigConverter.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Json;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts integration type configurations.
/// </summary>
public class IntegrationTypeConfigConverter : JsonConverter<IReadOnlyDictionary<ApplicationIntegrationType, IApplicationIntegrationTypeConfig?>>
{
    /// <inheritdoc />
    public override IReadOnlyDictionary<ApplicationIntegrationType, IApplicationIntegrationTypeConfig?> Read
    (
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new Dictionary<ApplicationIntegrationType, IApplicationIntegrationTypeConfig?>();

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

            if (!Enum.TryParse(reader.GetString(), out ApplicationIntegrationType key))
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            var value = JsonSerializer.Deserialize<IApplicationIntegrationTypeConfig>(ref reader, options);

            result.Add(key, value);
        }

        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<ApplicationIntegrationType, IApplicationIntegrationTypeConfig?> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (key, val) in value)
        {
            writer.WritePropertyName(((int)key).ToString());
            JsonSerializer.Serialize(writer, val, options);
        }

        writer.WriteEndObject();
    }
}
