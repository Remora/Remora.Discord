//
//  DiscriminatorConverter.cs
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

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts a user discriminator to or from JSON.
/// </summary>
internal class DiscriminatorConverter : JsonConverter<ushort>
{
    /// <inheritdoc />
    public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String)
        {
            throw new JsonException();
        }

        if (!ushort.TryParse(reader.GetString(), out var result))
        {
            throw new JsonException();
        }

        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ushort value, JsonSerializerOptions options)
    {
        if (value is 0)
        {
            // zeroes should not be padded
            writer.WriteStringValue($"{value}");
            return;
        }

        writer.WriteStringValue($"{value:D4}");
    }
}
