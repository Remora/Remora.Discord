//
//  UnixDateTimeOffsetConverter.cs
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

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to and from unix time.
    /// </summary>
    public class UnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                {
                    var milliseconds = reader.GetDouble();
                    return DateTimeOffset.UnixEpoch + TimeSpan.FromMilliseconds(milliseconds);
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
            var milliseconds = (value.ToUniversalTime() - DateTimeOffset.UnixEpoch).TotalMilliseconds;
            writer.WriteNumberValue(milliseconds);
        }
    }
}
