//
//  ISO8601DateTimeOffsetConverter.cs
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
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts instances of the <see cref="DateTime"/> struct to and from an ISO8601 representation in JSON.
    /// </summary>
    public class ISO8601DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    if (!DateTimeOffset.TryParse(reader.GetString(), out var time))
                    {
                        throw new JsonException();
                    }

                    return time;
                }
                default:
                {
                    throw new JsonException();
                }
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            var offset = value.Offset;
            writer.WriteStringValue(value.ToString($"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'+'{offset.Hours:D2}':'{offset.Minutes:D2}"));
        }
    }
}
