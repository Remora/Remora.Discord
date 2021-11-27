//
//  EncryptionModeConverter.cs
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
using Remora.Discord.API.Abstractions.API.VoiceGateway;

namespace Remora.Discord.API.Json.Converters.Internal
{
    /// <inheritdoc />
    public class EncryptionModeConverter : JsonConverter<EncryptionMode>
    {
        /// <inheritdoc />
        public override EncryptionMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected the token type of an {nameof(EncryptionMode)} to be 'string'.");
            }

            var mode = reader.GetString();
            if (mode is null)
            {
                throw new JsonException($"A null {nameof(EncryptionMode)} is invalid.");
            }

            return mode switch
            {
                "xsalsa20_poly1305" => EncryptionMode.XSalsa20_Poly1305,
                "xsalsa20_poly1305_lite" => EncryptionMode.XSalsa20_Poly1305_Lite,
                "xsalsa20_poly1305_suffix" => EncryptionMode.XSalsa20_Poly1305_Suffix,
                _ => throw new JsonException("Unrecognised encryption mode")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, EncryptionMode value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case EncryptionMode.XSalsa20_Poly1305:
                    writer.WriteStringValue("xsalsa20_poly1305");
                    break;
                case EncryptionMode.XSalsa20_Poly1305_Lite:
                    writer.WriteStringValue("xsalsa20_poly1305_lite");
                    break;
                case EncryptionMode.XSalsa20_Poly1305_Suffix:
                    writer.WriteStringValue("xsalsa20_poly1305_suffix");
                    break;
                default:
                    throw new InvalidOperationException("Unrecognised encryption mode.");
            }
        }
    }
}
