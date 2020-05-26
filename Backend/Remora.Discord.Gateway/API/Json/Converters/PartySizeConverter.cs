//
//  PartySizeConverter.cs
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
using Newtonsoft.Json;
using Remora.Discord.Gateway.API.Objects;

namespace Remora.Discord.Gateway.API.Json.Converters
{
    /// <summary>
    /// Converts <see cref="PartySize"/> values to and from JSON.
    /// </summary>
    internal class PartySizeConverter : JsonConverter<PartySize>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, PartySize? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                return;
            }

            writer.WriteStartArray();
            writer.WriteValue(value.CurrentSize);
            writer.WriteValue(value.MaxSize);
            writer.WriteEndArray();
        }

        /// <inheritdoc />
        public override PartySize ReadJson
        (
            JsonReader reader,
            Type objectType,
            PartySize? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonException();
            }

            var currentSize = reader.ReadAsInt32();
            if (!currentSize.HasValue)
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            var maxSize = reader.ReadAsInt32();
            if (!maxSize.HasValue)
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new JsonException();
            }

            return new PartySize(currentSize.Value, maxSize.Value);
        }
    }
}
