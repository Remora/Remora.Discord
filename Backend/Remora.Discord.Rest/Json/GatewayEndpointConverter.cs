//
//  GatewayEndpointConverter.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Core;
using Remora.Discord.Rest.API.Objects;

namespace Remora.Discord.Rest.Json
{
    /// <summary>
    /// Converts to and from a gateway endpoint instance.
    /// </summary>
    public class GatewayEndpointConverter : JsonConverter<IGatewayEndpoint>
    {
        /// <inheritdoc />
        public override IGatewayEndpoint Read
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

            string? url = null;
            Optional<int> shardCount = default;
            Optional<ISessionStartLimit> sessionStartLimit = default;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                switch (propertyName)
                {
                    case "url":
                    {
                        url = reader.GetString();
                        break;
                    }
                    case "shards":
                    {
                        shardCount = new Optional<int>(reader.GetInt32());
                        break;
                    }
                    case "session_start_limit":
                    {
                        sessionStartLimit = new Optional<ISessionStartLimit>
                        (
                            JsonSerializer.Deserialize<SessionStartLimit>(ref reader, options)
                        );

                        break;
                    }
                }

                if (!reader.Read())
                {
                    throw new JsonException();
                }
            }

            if (url is null)
            {
                throw new JsonException();
            }

            return new GatewayEndpoint(url, shardCount, sessionStartLimit);
        }

        /// <inheritdoc />
        public override void Write
        (
            Utf8JsonWriter writer,
            IGatewayEndpoint value,
            JsonSerializerOptions options
        )
        {
            JsonSerializer.Serialize(writer, (GatewayEndpoint)value, options);
        }
    }
}
