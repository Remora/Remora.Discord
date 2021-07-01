//
//  MessageComponentConverter.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts message components to and from JSON.
    /// </summary>
    internal class MessageComponentConverter : JsonConverter<IMessageComponent>
    {
        /// <inheritdoc />
        public override IMessageComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!JsonDocument.TryParseValue(ref reader, out var document))
            {
                throw new JsonException();
            }

            if (!document.RootElement.TryGetProperty("type", out var typeElement))
            {
                throw new JsonException();
            }

            if (!typeElement.TryGetInt32(out var typeValue))
            {
                throw new JsonException();
            }

            var type = (ComponentType)typeValue;
            if (!Enum.IsDefined(typeof(ComponentType), type))
            {
                // We don't know what this is
                return JsonSerializer.Deserialize<Component>(document.RootElement.GetRawText(), options);
            }

            return type switch
            {
                ComponentType.ActionRow
                    => JsonSerializer.Deserialize<ActionRowComponent>(document.RootElement.GetRawText(), options),
                ComponentType.Button
                    => JsonSerializer.Deserialize<ButtonComponent>(document.RootElement.GetRawText(), options),
                ComponentType.SelectMenu
                    => JsonSerializer.Deserialize<SelectMenuComponent>(document.RootElement.GetRawText(), options),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IMessageComponent value, JsonSerializerOptions options)
        {
            if (value is not IComponent component)
            {
                throw new ArgumentException
                (
                    $"This implementation requires that the concrete type implements the general-purpose {nameof(IComponent)} interface.",
                    nameof(value)
                );
            }

            JsonSerializer.Serialize(writer, component, options);
        }
    }
}
