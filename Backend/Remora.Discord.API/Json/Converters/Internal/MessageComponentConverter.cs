//
//  MessageComponentConverter.cs
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
internal class MessageComponentConverter : JsonConverter<IMessageComponent>
{
    /// <inheritdoc />
    public override IMessageComponent? Read
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
                => JsonSerializer.Deserialize<IActionRowComponent>(document.RootElement.GetRawText(), options),
            ComponentType.Button
                => JsonSerializer.Deserialize<IButtonComponent>(document.RootElement.GetRawText(), options),
            ComponentType.StringSelect
                => document.RootElement.Deserialize<IStringSelectComponent>(options),
            ComponentType.TextInput
                => JsonSerializer.Deserialize<ITextInputComponent>(document.RootElement.GetRawText(), options),
            ComponentType.UserSelect
                => document.RootElement.Deserialize<IUserSelectComponent>(options),
            ComponentType.RoleSelect
                => document.RootElement.Deserialize<IRoleSelectComponent>(options),
            ComponentType.MentionableSelect
                => document.RootElement.Deserialize<IMentionableSelectComponent>(options),
            ComponentType.ChannelSelect
                => document.RootElement.Deserialize<IChannelSelectComponent>(options),
            ComponentType.Section
                => document.RootElement.Deserialize<ISectionComponent>(options),
            ComponentType.TextDisplay
                => document.RootElement.Deserialize<ITextDisplayComponent>(options),
            ComponentType.Thumbnail
                => document.RootElement.Deserialize<IThumbnailComponent>(options),
            ComponentType.MediaGallery
                => document.RootElement.Deserialize<IMediaGalleryComponent>(options),
            ComponentType.File
                => document.RootElement.Deserialize<IFileDisplayServerComponent>(options),
            ComponentType.Separator
                => document.RootElement.Deserialize<ISeparatorComponent>(options),
            _ => throw new NotSupportedException($"Deserialization of the component type {type} is not supported")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IMessageComponent value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case IActionRowComponent actionRow:
            {
                JsonSerializer.Serialize(writer, actionRow, options);
                break;
            }
            case IButtonComponent button:
            {
                JsonSerializer.Serialize(writer, button, options);
                break;
            }
            case IStringSelectComponent stringSelect:
            {
                JsonSerializer.Serialize(writer, stringSelect, options);
                break;
            }
            case ITextInputComponent textInput:
            {
                JsonSerializer.Serialize(writer, textInput, options);
                break;
            }
            case IUserSelectComponent userSelect:
            {
                JsonSerializer.Serialize(writer, userSelect, options);
                break;
            }
            case IRoleSelectComponent roleSelect:
            {
                JsonSerializer.Serialize(writer, roleSelect, options);
                break;
            }
            case IChannelSelectComponent channelSelect:
            {
                JsonSerializer.Serialize(writer, channelSelect, options);
                break;
            }
            case IMentionableSelectComponent mentionableSelect:
            {
                JsonSerializer.Serialize(writer, mentionableSelect, options);
                break;
            }
            case ISectionComponent section:
            {
                JsonSerializer.Serialize(writer, section, options);
                break;
            }
            case ITextDisplayComponent textDisplay:
            {
                JsonSerializer.Serialize(writer, textDisplay, options);
                break;
            }
            case IThumbnailComponent thumbnail:
            {
                JsonSerializer.Serialize(writer, thumbnail, options);
                break;
            }
            case IMediaGalleryComponent mediaGallery:
            {
                JsonSerializer.Serialize(writer, mediaGallery, options);
                break;
            }
            case IFileDisplayServerComponent file:
            {
                JsonSerializer.Serialize(writer, file, options);
                break;
            }
            case ISeparatorComponent separator:
            {
                JsonSerializer.Serialize(writer, separator, options);
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
