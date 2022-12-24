//
//  PartialMessageComponentConverter.cs
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
/// Converts message components to and from JSON.
/// </summary>
internal class PartialMessageComponentConverter : JsonConverter<IPartialMessageComponent>
{
    /// <inheritdoc />
    public override IPartialMessageComponent? Read
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

        var type = (ComponentType)typeValue;
        if (!Enum.IsDefined(typeof(ComponentType), type))
        {
            // We don't know what this is
            throw new JsonException();
        }

        return type switch
        {
            ComponentType.ActionRow
                => JsonSerializer.Deserialize<IPartialActionRowComponent>(document.RootElement.GetRawText(), options),
            ComponentType.Button
                => JsonSerializer.Deserialize<IPartialButtonComponent>(document.RootElement.GetRawText(), options),
            ComponentType.StringSelect
                => document.RootElement.Deserialize<IPartialStringSelectComponent>(options),
            ComponentType.TextInput
                => JsonSerializer.Deserialize<IPartialTextInputComponent>(document.RootElement.GetRawText(), options),
            ComponentType.UserSelect
                => document.RootElement.Deserialize<IPartialUserSelectComponent>(options),
            ComponentType.RoleSelect
                => document.RootElement.Deserialize<IPartialRoleSelectComponent>(options),
            ComponentType.MentionableSelect
                => document.RootElement.Deserialize<IPartialMentionableSelectComponent>(options),
            ComponentType.ChannelSelect
                => document.RootElement.Deserialize<IPartialChannelSelectComponent>(options),
            _ => throw new NotSupportedException($"Deserialization of the component type {type} is not supported")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IPartialMessageComponent value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case IPartialActionRowComponent actionRow:
            {
                JsonSerializer.Serialize(writer, actionRow, options);
                break;
            }
            case IPartialButtonComponent button:
            {
                JsonSerializer.Serialize(writer, button, options);
                break;
            }
            case IPartialStringSelectComponent stringSelect:
            {
                JsonSerializer.Serialize(writer, stringSelect, options);
                break;
            }
            case IPartialTextInputComponent textInput:
            {
                JsonSerializer.Serialize(writer, textInput, options);
                break;
            }
            case IPartialUserSelectComponent userSelect:
            {
                JsonSerializer.Serialize(writer, userSelect, options);
                break;
            }
            case IPartialRoleSelectComponent roleSelect:
            {
                JsonSerializer.Serialize(writer, roleSelect, options);
                break;
            }
            case IPartialMentionableSelectComponent mentionableSelect:
            {
                JsonSerializer.Serialize(writer, mentionableSelect, options);
                break;
            }
            case IPartialChannelSelectComponent channelSelect:
            {
                JsonSerializer.Serialize(writer, channelSelect, options);
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
