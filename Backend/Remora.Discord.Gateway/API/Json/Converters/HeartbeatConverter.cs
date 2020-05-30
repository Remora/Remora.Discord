//
//  HeartbeatConverter.cs
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
    /// <inheritdoc />
    public class HeartbeatConverter : JsonConverter<Heartbeat>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Heartbeat? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.LastSequenceNumber);
        }

        /// <inheritdoc />
        public override Heartbeat ReadJson
        (
            JsonReader reader,
            Type objectType,
            Heartbeat? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.TokenType != JsonToken.Integer)
            {
                throw new JsonException();
            }

            return new Heartbeat((long?)reader.Value);
        }
    }
}
