//
//  AuditLogChangeConverter.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Remora.Rest.Json;

namespace Remora.Discord.API.Json;

/// <summary>
/// Converts <see cref="IAuditLogChange"/> objects to and from JSON.
/// </summary>
internal class AuditLogChangeConverter : JsonConverter<IAuditLogChange>
{
    private static readonly IReadOnlyDictionary<string, Type> KeyTypes = new Dictionary<string, Type>
    {
        { "afk_channel_id", typeof(Snowflake) },
        { "afk_timeout", typeof(int) },
        { "allow", typeof(IDiscordPermissionSet) },
        { "application_id", typeof(Snowflake) },
        { "archived", typeof(bool) },
        { "asset", typeof(string) },
        { "auto_archive_duration", typeof(AutoArchiveDuration) },
        { "available", typeof(bool) },
        { "avatar_hash", typeof(IImageHash) },
        { "banner_hash", typeof(IImageHash) },
        { "bitrate", typeof(int) },
        { "channel_id", typeof(Snowflake) },
        { "code", typeof(string) },
        { "color", typeof(Color) },
        { "deaf", typeof(bool) },
        { "default_auto_archive_duration", typeof(AutoArchiveDuration) },
        { "default_message_notifications", typeof(MessageNotificationLevel) },
        { "deny", typeof(IDiscordPermissionSet) },
        { "description", typeof(string) },
        { "discovery_splash_hash", typeof(IImageHash) },
        { "enable_emoticons", typeof(bool) },
        { "entity_type", typeof(GuildScheduledEventEntityType) },
        { "expire_behaviour", typeof(IntegrationExpireBehaviour) },
        { "expire_grace_period", typeof(TimeSpan) },
        { "explicit_content_filter", typeof(ExplicitContentFilterLevel) },
        { "format_type", typeof(StickerFormatType) },
        { "guild_id", typeof(Snowflake) },
        { "hoist", typeof(bool) },
        { "icon_hash", typeof(IImageHash) },
        { "id", typeof(Snowflake) },
        { "inviter_id", typeof(Snowflake) },
        { "location", typeof(string) },
        { "locked", typeof(bool) },
        { "max_age", typeof(TimeSpan) },
        { "max_uses", typeof(int) },
        { "mentionable", typeof(bool) },
        { "mfa_level", typeof(MultiFactorAuthenticationLevel) },
        { "mute", typeof(bool) },
        { "name", typeof(string) },
        { "nick", typeof(string) },
        { "nsfw", typeof(bool) },
        { "owner_id", typeof(Snowflake) },
        { "permission_overwrites", typeof(IReadOnlyList<IPermissionOverwrite>) },
        { "permissions", typeof(IDiscordPermissionSet) },
        { "position", typeof(int) },
        { "preferred_locale", typeof(string) },
        { "privacy_level", typeof(StagePrivacyLevel) },
        { "prune_delete_days", typeof(TimeSpan) },
        { "public_updates_channel_id", typeof(Snowflake) },
        { "rate_limit_per_user", typeof(TimeSpan) },
        { "region", typeof(string) },
        { "rules_channel_id", typeof(Snowflake) },
        { "splash_hash", typeof(IImageHash) },
        { "status", typeof(GuildScheduledEventStatus) },
        { "system_channel_id", typeof(Snowflake) },
        { "tags", typeof(string) },
        { "temporary", typeof(bool) },
        { "topic", typeof(string) },
        { "type", typeof(OneOf<int, string>) },
        { "user_limit", typeof(int) },
        { "uses", typeof(int) },
        { "vanity_url_code", typeof(string) },
        { "verification_level", typeof(VerificationLevel) },
        { "widget_channel_id", typeof(Snowflake) },
        { "widget_enabled", typeof(bool) },
        { "$add", typeof(IReadOnlyList<IPartialRole>) },
        { "$remove", typeof(IReadOnlyList<IPartialRole>) }
    };

    private static readonly IReadOnlyDictionary<string, JsonConverter> KeyConverters =
        new Dictionary<string, JsonConverter>
        {
            { "afk_timeout", new UnitTimeSpanConverter(TimeUnit.Seconds) },
            { "expire_grace_period", new UnitTimeSpanConverter(TimeUnit.Days) },
            { "max_age", new UnitTimeSpanConverter(TimeUnit.Seconds) },
            { "prune_delete_days", new UnitTimeSpanConverter(TimeUnit.Days) },
            { "rate_limit_per_user", new UnitTimeSpanConverter(TimeUnit.Days) }
        };

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

        if (!KeyTypes.TryGetValue(key, out var valueType))
        {
            throw new JsonException();
        }

        if (KeyConverters.TryGetValue(key, out var keyConverter))
        {
            options = new(options);
            options.Converters.Add(keyConverter);
        }

        Optional<object?> newValue = default;
        Optional<object?> oldValue = default;

        if (jsonDocument.RootElement.TryGetProperty("old_value", out var oldValueProperty))
        {
            var value = JsonSerializer.Deserialize(oldValueProperty.GetRawText(), valueType, options);
            oldValue = value;
        }

        // ReSharper disable once InvertIf
        if (jsonDocument.RootElement.TryGetProperty("new_value", out var newValueProperty))
        {
            var value = JsonSerializer.Deserialize(newValueProperty.GetRawText(), valueType, options);
            newValue = value;
        }

        return new AuditLogChange(newValue, oldValue, key);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IAuditLogChange value, JsonSerializerOptions options)
    {
        if (!KeyTypes.TryGetValue(value.Key, out var valueType))
        {
            throw new JsonException();
        }

        writer.WriteStartObject();
        {
            writer.WriteString("key", value.Key);

            if (value.NewValue.HasValue)
            {
                writer.WritePropertyName("new_value");
                JsonSerializer.Serialize(writer, value.NewValue.Value, valueType, options);
            }

            if (value.OldValue.HasValue)
            {
                writer.WritePropertyName("old_value");
                JsonSerializer.Serialize(writer, value.OldValue.Value, valueType, options);
            }
        }
        writer.WriteEndObject();
    }
}
