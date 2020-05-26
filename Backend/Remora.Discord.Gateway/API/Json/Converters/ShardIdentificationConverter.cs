//
//  ShardIdentificationConverter.cs
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
using Remora.Discord.Gateway.API.Commands;

namespace Remora.Discord.Gateway.API.Json.Converters
{
    /// <summary>
    /// Converts <see cref="ShardIdentification"/> values to and from JSON.
    /// </summary>
    public class ShardIdentificationConverter : JsonConverter<ShardIdentification>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, ShardIdentification value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.ShardID);
            writer.WriteValue(value.ShardCount);
            writer.WriteEndArray();
        }

        /// <inheritdoc />
        public override ShardIdentification ReadJson
        (
            JsonReader reader,
            Type objectType,
            ShardIdentification existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonException();
            }

            var shardID = reader.ReadAsInt32();
            if (!shardID.HasValue)
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            var shardCount = reader.ReadAsInt32();
            if (!shardCount.HasValue)
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

            return new ShardIdentification(shardID.Value, shardCount.Value);
        }
    }
}
