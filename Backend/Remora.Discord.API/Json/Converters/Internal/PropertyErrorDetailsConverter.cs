//
//  PropertyErrorDetailsConverter.cs
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
using Remora.Discord.API.Objects;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts <see cref="IRestError"/> instances to and from JSON.
/// </summary>
internal class PropertyErrorDetailsConverter : JsonConverter<IPropertyErrorDetails>
{
    /// <inheritdoc />
    public override IPropertyErrorDetails Read
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

        if (!reader.Read())
        {
            throw new JsonException();
        }

        List<IErrorDetails>? errors = null;
        Dictionary<string, IPropertyErrorDetails>? memberErrors = null;
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            if (propertyName is null)
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            if (propertyName == "_errors")
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException();
                }

                errors = JsonSerializer.Deserialize<List<IErrorDetails>>(ref reader, options);
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                continue;
            }

            // We've got an inner property error
            memberErrors ??= new Dictionary<string, IPropertyErrorDetails>();

            var propertyErrorDetails = JsonSerializer.Deserialize<IPropertyErrorDetails>(ref reader, options);
            if (propertyErrorDetails is null)
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            memberErrors.Add(propertyName, propertyErrorDetails);
        }

        return new PropertyErrorDetails(memberErrors, errors);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IPropertyErrorDetails value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        {
            if (value.MemberErrors is not null)
            {
                foreach (var (propertyName, memberError) in value.MemberErrors)
                {
                    writer.WritePropertyName(propertyName);
                    JsonSerializer.Serialize(writer, memberError, options);
                }
            }

            if (value.Errors is not null)
            {
                writer.WritePropertyName("_errors");
                JsonSerializer.Serialize(writer, value.Errors, options);
            }
        }
        writer.WriteEndObject();
    }
}
