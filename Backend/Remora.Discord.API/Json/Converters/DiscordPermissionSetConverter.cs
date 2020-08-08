//
//  DiscordPermissionSetConverter.cs
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
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts to and from the JSON representation of a <see cref="IDiscordPermissionSet"/>.
    /// </summary>
    public class DiscordPermissionSetConverter : JsonConverter<IDiscordPermissionSet>
    {
        /// <inheritdoc />
        public override IDiscordPermissionSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                {
                    return new DiscordPermissionSet(new BigInteger(reader.GetUInt64()));
                }
                case JsonTokenType.String:
                {
                    if (!BigInteger.TryParse(reader.GetString(), out var value))
                    {
                        throw new JsonException();
                    }

                    return new DiscordPermissionSet(value);
                }
                default:
                {
                    throw new JsonException();
                }
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IDiscordPermissionSet value, JsonSerializerOptions options)
        {
            if (value.Value <= ulong.MaxValue)
            {
                writer.WriteNumberValue((ulong)value.Value);
                return;
            }

            writer.WriteStringValue(value.Value.ToString());
        }
    }
}
