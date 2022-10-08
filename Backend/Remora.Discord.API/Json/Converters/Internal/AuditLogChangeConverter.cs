//
//  AuditLogChangeConverter.cs
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
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Remora.Rest.Extensions;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts <see cref="IAuditLogChange"/> objects to and from JSON.
/// </summary>
internal class AuditLogChangeConverter : JsonConverter<IAuditLogChange>
{
    /// <inheritdoc />
    public override IAuditLogChange Read
    (
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        if (!jsonDocument.RootElement.TryGetProperty("key", out var keyProperty))
        {
            throw new JsonException();
        }

        var key = keyProperty.GetString();
        if (key is null)
        {
            throw new JsonException();
        }

        Optional<string> newValue = default;
        Optional<string> oldValue = default;

        if (jsonDocument.RootElement.TryGetProperty("old_value", out var oldValueProperty))
        {
            oldValue = oldValueProperty.GetRawText();
        }

        // ReSharper disable once InvertIf
        if (jsonDocument.RootElement.TryGetProperty("new_value", out var newValueProperty))
        {
            newValue = newValueProperty.GetRawText();
        }

        return new AuditLogChange(newValue, oldValue, key);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IAuditLogChange value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        {
            writer.Write("key", value.Key, options);

            if (value.NewValue.HasValue)
            {
                writer.WritePropertyName("new_value");
                writer.WriteRawValue(value.NewValue.Value);
            }

            if (value.OldValue.HasValue)
            {
                writer.WritePropertyName("old_value");
                writer.WriteRawValue(value.OldValue.Value);
            }
        }
        writer.WriteEndObject();
    }
}
