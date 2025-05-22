//
//  HexCodeColourConverter.cs
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
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts instances of the <see cref="Color"/> struct to and from JSON.
/// </summary>
[PublicAPI]
public class HexCodeColourConverter : JsonConverter<Color>
{
    /// <inheritdoc />
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var text = reader.GetString();
        if (text is null)
        {
            throw new JsonException();
        }

        if (!uint.TryParse(text[1..], NumberStyles.HexNumber, null, out var value))
        {
            throw new JsonException();
        }

        var clrValue = value | 0xFF000000;

        return Color.FromArgb((int)clrValue);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        var val = value.ToArgb() & 0x00FFFFFF;
        writer.WriteStringValue($"#{val:x6}");
    }
}
