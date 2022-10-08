//
//  PartySizeConverter.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts <see cref="PartySize"/> values to and from JSON.
/// </summary>
internal class PartySizeConverter : JsonConverter<IPartySize?>
{
    /// <inheritdoc/>
    public override IPartySize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        if (!reader.Read())
        {
            throw new JsonException();
        }

        var currentSize = reader.GetInt32();
        if (!reader.Read())
        {
            throw new JsonException();
        }

        var maxSize = reader.GetInt32();
        if (!reader.Read())
        {
            throw new JsonException();
        }

        if (reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException();
        }

        return new PartySize(currentSize, maxSize);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IPartySize? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        writer.WriteNumberValue(value.CurrentSize);
        writer.WriteNumberValue(value.MaxSize);
        writer.WriteEndArray();
    }
}
