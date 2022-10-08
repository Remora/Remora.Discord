//
//  IPAddressConverter.cs
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
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts an <see cref="IPAddress"/> to or from JSON.
/// </summary>
[PublicAPI]
public class IPAddressConverter : JsonConverter<IPAddress>
{
    /// <inheritdoc />
    public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String)
        {
            throw new JsonException("Cannot convert IP address: expected a string token");
        }

        var address = reader.GetString();
        if (address is null)
        {
            throw new JsonException("Cannot convert IP address: token was null");
        }

        if (IPAddress.TryParse(address, out var parsedAddress))
        {
            return parsedAddress;
        }

        throw new JsonException("Cannot convert IP address");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
