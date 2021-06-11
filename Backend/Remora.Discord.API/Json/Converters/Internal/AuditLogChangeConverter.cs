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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Extensions;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts <see cref="IAuditLogChange"/> objects to and from JSON.
    /// </summary>
    internal class AuditLogChangeConverter : JsonConverter<IAuditLogChange>
    {
        private static readonly IReadOnlyDictionary<string, Type> KeyTypes = new Dictionary<string, Type>
        {
            { "name", typeof(string) },
            { "description", typeof(string) },
            { "icon_hash", typeof(IImageHash) },
            { "splash_hash", typeof(IImageHash) },
            { "discovery_splash_hash", typeof(IImageHash) },
            { "banner_hash", typeof(IImageHash) },
            { "owner_id", typeof(Snowflake) },
            { "region", typeof(string) },
            { "preferred_locale", typeof(string) },
            { "afk_channel_id", typeof(Snowflake) },
            { "afk_timeout", typeof(int) },
            { "rules_channel_id", typeof(Snowflake) },
            { "public_updates_channel_id", typeof(Snowflake) },
            { "mfa_level", typeof(MultiFactorAuthenticationLevel) },
            { "verification_level", typeof(VerificationLevel) },
            { "explicit_content_filter", typeof(ExplicitContentFilterLevel) },
            { "default_message_notifications", typeof(MessageNotificationLevel) },
            { "vanity_url_code", typeof(string) },
            { "$add", typeof(IReadOnlyList<IPartialRole>) },
            { "$remove", typeof(IReadOnlyList<IPartialRole>) },
            { "prune_delete_days", typeof(TimeSpan) },
            { "widget_enabled", typeof(bool) },
            { "widget_channel_id", typeof(Snowflake) },
            { "system_channel_id", typeof(Snowflake) },
            { "position", typeof(int) },
            { "topic", typeof(string) },
            { "bitrate", typeof(int) },
            { "permission_overwrites", typeof(IReadOnlyList<IPermissionOverwrite>) },
            { "nsfw", typeof(bool) },
            { "application_id", typeof(Snowflake) },
            { "rate_limit_per_user", typeof(TimeSpan) },
            { "permissions", typeof(IDiscordPermissionSet) },
            { "color", typeof(Color) },
            { "hoist", typeof(bool) },
            { "mentionable", typeof(bool) },
            { "allow", typeof(IDiscordPermissionSet) },
            { "deny", typeof(IDiscordPermissionSet) },
            { "code", typeof(string) },
            { "channel_id", typeof(Snowflake) },
            { "inviter_id", typeof(Snowflake) },
            { "max_uses", typeof(int) },
            { "uses", typeof(int) },
            { "max_age", typeof(TimeSpan) },
            { "temporary", typeof(bool) },
            { "deaf", typeof(bool) },
            { "mute", typeof(bool) },
            { "nick", typeof(string) },
            { "avatar_hash", typeof(IImageHash) },
            { "id", typeof(Snowflake) },
            { "type", typeof(string) },
            { "enable_emoticons", typeof(bool) },
            { "expire_behaviour", typeof(IntegrationExpireBehaviour) },
            { "expire_grace_period", typeof(TimeSpan) },
            { "user_limit", typeof(int) },
            { "privacy_level", typeof(StagePrivacyLevel) }
        };

        private static readonly IReadOnlyDictionary<string, JsonConverter> KeyConverters =
        new Dictionary<string, JsonConverter>
        {
            { "afk_timeout", new UnitTimeSpanConverter(TimeUnit.Seconds) },
            { "prune_delete_days", new UnitTimeSpanConverter(TimeUnit.Days) },
            { "rate_limit_per_user", new UnitTimeSpanConverter(TimeUnit.Days) },
            { "max_age", new UnitTimeSpanConverter(TimeUnit.Seconds) },
            { "expire_grace_period", new UnitTimeSpanConverter(TimeUnit.Days) }
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
                options = options.Clone();
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
}
