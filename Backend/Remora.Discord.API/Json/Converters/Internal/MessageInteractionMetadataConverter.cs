//
//  MessageInteractionMetadataConverter.cs
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
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts message interaction metadata objects to and from JSON.
/// </summary>
internal class MessageInteractionMetadataConverter : JsonConverter<IMessageInteractionMetadata>
{
    /// <inheritdoc />
    public override IMessageInteractionMetadata? Read
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

        var type = (InteractionType)typeValue;
        if (!Enum.IsDefined(typeof(InteractionType), type))
        {
            // We don't know what this is
            throw new JsonException();
        }

        return type switch
        {
            InteractionType.ApplicationCommand
                => JsonSerializer.Deserialize<IApplicationCommandInteractionMetadata>(document.RootElement.GetRawText(), options),
            InteractionType.MessageComponent
                => JsonSerializer.Deserialize<IMessageComponentInteractionMetadata>(document.RootElement.GetRawText(), options),
            InteractionType.ModalSubmit
                => document.RootElement.Deserialize<IModalSubmitInteractionMetadata>(options),
            _ => throw new NotSupportedException($"Deserialization of the component type {type} is not supported")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IMessageInteractionMetadata value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case IApplicationCommandInteractionMetadata applicationCommand:
            {
                JsonSerializer.Serialize(writer, applicationCommand, options);
                break;
            }
            case IMessageComponentInteractionMetadata messageComponent:
            {
                JsonSerializer.Serialize(writer, messageComponent, options);
                break;
            }
            case IModalSubmitInteractionMetadata modalSubmit:
            {
                JsonSerializer.Serialize(writer, modalSubmit, options);
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
