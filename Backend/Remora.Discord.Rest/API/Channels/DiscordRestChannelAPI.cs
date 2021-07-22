//
//  DiscordRestChannelAPI.cs
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
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using Remora.Discord.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestChannelAPI" />
    [PublicAPI]
    public class DiscordRestChannelAPI : AbstractDiscordRestAPI, IDiscordRestChannelAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestChannelAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        public DiscordRestChannelAPI(DiscordHttpClient discordHttpClient, IOptions<JsonSerializerOptions> jsonOptions)
            : base(discordHttpClient, jsonOptions)
        {
        }

        /// <inheritdoc />
        public virtual Task<Result<IChannel>> GetChannelAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IChannel>
            (
                $"channels/{channelID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IChannel>> ModifyChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<Stream> icon = default,
            Optional<ChannelType> type = default,
            Optional<int?> position = default,
            Optional<string?> topic = default,
            Optional<bool?> isNsfw = default,
            Optional<int?> rateLimitPerUser = default,
            Optional<int?> bitrate = default,
            Optional<int?> userLimit = default,
            Optional<IReadOnlyList<IPermissionOverwrite>?> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            Optional<VideoQualityMode?> videoQualityMode = default,
            Optional<bool> isArchived = default,
            Optional<TimeSpan> autoArchiveDuration = default,
            Optional<bool> isLocked = default,
            Optional<TimeSpan> defaultAutoArchiveDuration = default,
            Optional<string?> rtcRegion = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (name.HasValue && name.Value.Length is > 100 or < 1)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters.");
            }

            if (topic.HasValue && (topic.Value?.Length is > 1024 or < 0))
            {
                return new ArgumentOutOfRangeError(nameof(topic), "The topic must be between 0 and 1024 characters.");
            }

            if (userLimit.HasValue && userLimit.Value is > 99 or < 0)
            {
                return new ArgumentOutOfRangeError(nameof(userLimit), "The user limit must be between 0 and 99.");
            }

            Optional<string> base64EncodedIcon = default;
            if (icon.HasValue)
            {
                byte[] bytes;
                if (icon.Value is MemoryStream ms)
                {
                    bytes = ms.ToArray();
                }
                else
                {
                    await using var copy = new MemoryStream();
                    await icon.Value.CopyToAsync(copy, ct);

                    bytes = copy.ToArray();
                }

                base64EncodedIcon = Convert.ToBase64String(bytes);
            }

            return await this.DiscordHttpClient.PatchAsync<IChannel>
            (
                $"channels/{channelID}",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("icon", base64EncodedIcon, this.JsonOptions);
                        json.Write("type", type, this.JsonOptions);
                        json.Write("position", position, this.JsonOptions);
                        json.Write("topic", topic, this.JsonOptions);
                        json.Write("nsfw", isNsfw, this.JsonOptions);
                        json.Write("rate_limit_per_user", rateLimitPerUser, this.JsonOptions);
                        json.Write("bitrate", bitrate, this.JsonOptions);
                        json.Write("user_limit", userLimit, this.JsonOptions);
                        json.Write("permission_overwrites", permissionOverwrites, this.JsonOptions);
                        json.Write("parent_id", parentId, this.JsonOptions);
                        json.Write("video_quality_mode", videoQualityMode, this.JsonOptions);
                        json.Write("archived", isArchived, this.JsonOptions);

                        if (autoArchiveDuration.HasValue)
                        {
                            json.WriteNumber("auto_archive_duration", autoArchiveDuration.Value.TotalMinutes);
                        }

                        json.Write("locked", isLocked, this.JsonOptions);

                        if (defaultAutoArchiveDuration.HasValue)
                        {
                            json.WriteNumber
                            (
                                "default_auto_archive_duration",
                                defaultAutoArchiveDuration.Value.TotalMinutes
                            );
                        }

                        json.Write("rtc_region", rtcRegion, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<Result<IChannel>> ModifyGroupDMChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<Stream> icon = default,
            CancellationToken ct = default
        )
        {
            return ModifyChannelAsync(channelID, name, icon, ct: ct);
        }

        /// <inheritdoc />
        public Task<Result<IChannel>> ModifyGuildTextChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<ChannelType> type = default,
            Optional<int?> position = default,
            Optional<string?> topic = default,
            Optional<bool?> isNsfw = default,
            Optional<int?> rateLimitPerUser = default,
            Optional<IReadOnlyList<IPermissionOverwrite>?> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            Optional<TimeSpan> defaultAutoArchiveDuration = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return ModifyChannelAsync
            (
                channelID,
                name,
                type: type,
                position: position,
                topic: topic,
                isNsfw: isNsfw,
                rateLimitPerUser: rateLimitPerUser,
                permissionOverwrites: permissionOverwrites,
                parentId: parentId,
                defaultAutoArchiveDuration: defaultAutoArchiveDuration,
                reason: reason,
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<Result<IChannel>> ModifyGuildVoiceChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<int?> position = default,
            Optional<int?> bitrate = default,
            Optional<int?> userLimit = default,
            Optional<IReadOnlyList<IPermissionOverwrite>?> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            Optional<string?> rtcRegion = default,
            Optional<VideoQualityMode?> videoQualityMode = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return ModifyChannelAsync
            (
                channelID,
                name,
                position: position,
                bitrate: bitrate,
                userLimit: userLimit,
                permissionOverwrites: permissionOverwrites,
                parentId: parentId,
                rtcRegion: rtcRegion,
                videoQualityMode: videoQualityMode,
                reason: reason,
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<Result<IChannel>> ModifyGuildNewsChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<ChannelType> type = default,
            Optional<int?> position = default,
            Optional<string?> topic = default,
            Optional<bool?> isNsfw = default,
            Optional<IReadOnlyList<IPermissionOverwrite>?> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return ModifyChannelAsync
            (
                channelID,
                name,
                type: type,
                position: position,
                topic: topic,
                isNsfw: isNsfw,
                permissionOverwrites: permissionOverwrites,
                parentId: parentId,
                reason: reason,
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<Result<IChannel>> ModifyGuildStoreChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<int?> position = default,
            Optional<bool?> isNsfw = default,
            Optional<IReadOnlyList<IPermissionOverwrite>?> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return ModifyChannelAsync
            (
                channelID,
                name,
                position: position,
                isNsfw: isNsfw,
                permissionOverwrites: permissionOverwrites,
                parentId: parentId,
                reason: reason,
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<Result<IChannel>> ModifyThreadChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<bool> isArchived = default,
            Optional<TimeSpan> autoArchiveDuration = default,
            Optional<bool> isLocked = default,
            Optional<int?> rateLimitPerUser = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return ModifyChannelAsync
            (
                channelID,
                name,
                isArchived: isArchived,
                autoArchiveDuration: autoArchiveDuration,
                isLocked: isLocked,
                rateLimitPerUser: rateLimitPerUser,
                reason: reason,
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteChannelAsync
        (
            Snowflake channelID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
        (
            Snowflake channelID,
            Optional<Snowflake> around = default,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            var hasAny = around.HasValue || before.HasValue || after.HasValue;
            var hasStrictlyOne = around.HasValue ^ before.HasValue ^ after.HasValue;
            if (hasAny && !hasStrictlyOne)
            {
                return new NotSupportedError
                (
                    $"{nameof(around)}, {nameof(before)}, and {nameof(after)} are mutually exclusive."
                );
            }

            if (limit.HasValue && limit.Value is > 100 or < 1)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(limit),
                    "The message limit must be between 1 and 100."
                );
            }

            return await this.DiscordHttpClient.GetAsync<IReadOnlyList<IMessage>>
            (
                $"channels/{channelID}/messages",
                b =>
                {
                    if (around.HasValue)
                    {
                        b.AddQueryParameter("around", around.Value.ToString());
                    }

                    if (before.HasValue)
                    {
                        b.AddQueryParameter("before", before.Value.ToString());
                    }

                    if (after.HasValue)
                    {
                        b.AddQueryParameter("after", after.Value.ToString());
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IMessage>> GetChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IMessage>
            (
                $"channels/{channelID}/messages/{messageID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IMessage>> CreateMessageAsync
        (
            Snowflake channelID,
            Optional<string> content = default,
            Optional<string> nonce = default,
            Optional<bool> isTTS = default,
            Optional<FileData> file = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IAllowedMentions> allowedMentions = default,
            Optional<IMessageReference> messageReference = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            Optional<IReadOnlyList<Snowflake>> stickerIds = default,
            CancellationToken ct = default
        )
        {
            if (nonce.HasValue && nonce.Value.Length > 25)
            {
                return new ArgumentOutOfRangeError(nameof(nonce), "The nonce length must be less than 25 characters.");
            }

            if (!content.HasValue && !file.HasValue && !embeds.HasValue && !stickerIds.HasValue)
            {
                return new InvalidOperationError
                (
                    $"At least one of {nameof(content)}, {nameof(file)}, {nameof(embeds)}, or {nameof(stickerIds)} " +
                    "is required."
                );
            }

            return await this.DiscordHttpClient.PostAsync<IMessage>
            (
                $"channels/{channelID}/messages",
                b =>
                {
                    if (file.HasValue)
                    {
                        b.AddContent(new StreamContent(file.Value.Content), "file", file.Value.Name);
                    }

                    b.WithJson
                    (
                        json =>
                        {
                            json.Write("content", content, this.JsonOptions);
                            json.Write("nonce", nonce, this.JsonOptions);
                            json.Write("tts", isTTS, this.JsonOptions);
                            json.Write("embeds", embeds, this.JsonOptions);
                            json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                            json.Write("message_reference", messageReference, this.JsonOptions);
                            json.Write("components", components, this.JsonOptions);
                            json.Write("sticker_ids", stickerIds, this.JsonOptions);
                        }
                    );
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> CreateReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PutAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/@me",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteOwnReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/@me",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteUserReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Snowflake user,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/{user}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IReadOnlyList<IUser>>> GetReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            if (limit.HasValue && limit.Value is > 100 or < 1)
            {
                return new ArgumentOutOfRangeError(nameof(limit), "The limit must be between 1 and 100.");
            }

            return await this.DiscordHttpClient.GetAsync<IReadOnlyList<IUser>>
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}",
                b =>
                {
                    if (after.HasValue)
                    {
                        b.AddQueryParameter("after", after.Value.ToString());
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteAllReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteAllReactionsForEmojiAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IMessage>> EditMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<MessageFlags?> flags = default,
            Optional<IAllowedMentions?> allowedMentions = default,
            Optional<IReadOnlyList<IAttachment>> attachments = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync<IMessage>
            (
                $"channels/{channelID}/messages/{messageID}",
                b => b.WithJson
                (
                    json =>
                    {
                        json.Write("content", content, this.JsonOptions);
                        json.Write("embeds", embeds, this.JsonOptions);
                        json.Write("flags", flags, this.JsonOptions);
                        json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                        json.Write("attachments", attachments, this.JsonOptions);
                        json.Write("components", components, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result> BulkDeleteMessagesAsync
        (
            Snowflake channelID,
            IReadOnlyList<Snowflake> messageIDs,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (messageIDs.Count is > 100 or < 2)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(messageIDs),
                    "The number of messages to delete must be between 2 and 100."
                );
            }

            return await this.DiscordHttpClient.PostAsync
            (
                $"channels/{channelID}/messages/bulk-delete",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WritePropertyName("messages");
                        JsonSerializer.Serialize(json, messageIDs, this.JsonOptions);
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> EditChannelPermissionsAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            Optional<IDiscordPermissionSet> allow = default,
            Optional<IDiscordPermissionSet> deny = default,
            Optional<PermissionOverwriteType> type = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PutAsync
            (
                $"channels/{channelID}/permissions/{overwriteID}",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("allow", allow, this.JsonOptions);
                        json.Write("deny", deny, this.JsonOptions);
                        json.Write("type", type, this.JsonOptions);
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IInvite>>
            (
                $"channels/{channelID}/invites",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IInvite>> CreateChannelInviteAsync
        (
            Snowflake channelID,
            Optional<TimeSpan> maxAge = default,
            Optional<int> maxUses = default,
            Optional<bool> isTemporary = default,
            Optional<bool> isUnique = default,
            Optional<InviteTarget> targetType = default,
            Optional<Snowflake> targetUserID = default,
            Optional<Snowflake> targetApplicationID = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (maxAge.HasValue && maxAge.Value.TotalSeconds is < 0 or > 604800)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(maxAge),
                    "The maximum age must be between 0 and 604800 seconds."
                );
            }

            if (maxUses.HasValue && maxUses.Value is < 0 or > 100)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(maxUses),
                    "The maximum number of uses must be between 0 and 100."
                );
            }

            return await this.DiscordHttpClient.PostAsync<IInvite>
            (
                $"channels/{channelID}/invites",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        if (maxAge.HasValue)
                        {
                            json.WriteNumber("max_age", (ulong)maxAge.Value.TotalSeconds);
                        }

                        json.Write("max_uses", maxUses, this.JsonOptions);
                        json.Write("temporary", isTemporary, this.JsonOptions);
                        json.Write("unique", isUnique, this.JsonOptions);
                        json.Write("target_type", targetType, this.JsonOptions);
                        json.Write("target_user_id", targetUserID, this.JsonOptions);
                        json.Write("target_application_id", targetApplicationID, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteChannelPermissionAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/permissions/{overwriteID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IFollowedChannel>> FollowNewsChannelAsync
        (
            Snowflake channelID,
            Snowflake webhookChannelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PostAsync<IFollowedChannel>
            (
                $"channels/{channelID}/followers",
                b => b.WithJson
                (
                    p =>
                    {
                        p.WriteString("webhook_channel_id", webhookChannelID.ToString());
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> TriggerTypingIndicatorAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PostAsync
            (
                $"channels/{channelID}/typing",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IMessage>>
            (
                $"channels/{channelID}/pins",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> PinMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PutAsync
            (
                $"channels/{channelID}/pins/{messageID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> UnpinMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/pins/{messageID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> GroupDMAddRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            string accessToken,
            Optional<string> nickname = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PutAsync
            (
                $"channels/{channelID}/recipients/{userID}",
                b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("access_token", accessToken);
                        json.Write("nick", nickname, this.JsonOptions);
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> GroupDMRemoveRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/recipients/{userID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IChannel>> StartThreadWithMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string name,
            TimeSpan autoArchiveDuration,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (name.Length is < 1 or > 100)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters");
            }

            return await this.DiscordHttpClient.PostAsync<IChannel>
            (
                $"channels/{channelID}/messages/{messageID}/threads",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.WriteNumber("auto_archive_duration", autoArchiveDuration.TotalMinutes);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IChannel>> StartThreadWithoutMessageAsync
        (
            Snowflake channelID,
            string name,
            TimeSpan autoArchiveDuration,
            Optional<ChannelType> type,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (name.Length is < 1 or > 100)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters");
            }

            return await this.DiscordHttpClient.PostAsync<IChannel>
            (
                $"channels/{channelID}/threads",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.WriteNumber("auto_archive_duration", autoArchiveDuration.TotalMinutes);
                        json.Write("type", type, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> JoinThreadAsync(Snowflake channelID, CancellationToken ct = default)
        {
            return this.DiscordHttpClient.PutAsync($"channels/{channelID}/thread-members/@me", ct: ct);
        }

        /// <inheritdoc />
        public virtual Task<Result> AddThreadMemberAsync
        (
            Snowflake channelID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PutAsync
            (
                $"channels/{channelID}/thread-members/{userID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> LeaveThreadAsync(Snowflake channelID, CancellationToken ct = default)
        {
            return this.DiscordHttpClient.DeleteAsync($"channels/{channelID}/thread-members/@me", ct: ct);
        }

        /// <inheritdoc />
        public virtual Task<Result> RemoveThreadMemberAsync
        (
            Snowflake channelID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/thread-members/{userID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IThreadMember>>> ListThreadMembersAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IThreadMember>>
            (
                $"channels/{channelID}/thread-members",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IThreadQueryResponse>> ListActiveThreadsAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IThreadQueryResponse>
            (
                $"channels/{channelID}/threads/active",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IThreadQueryResponse>> ListPublicArchivedThreadsAsync
        (
            Snowflake channelID,
            Optional<DateTimeOffset> before = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IThreadQueryResponse>
            (
                $"channels/{channelID}/threads/archived/public",
                b =>
                {
                    if (before.HasValue)
                    {
                        var offset = before.Value.Offset;
                        var value = before.Value.ToString
                        (
                            $"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'+'{offset.Hours:D2}':'{offset.Minutes:D2}"
                        );

                        b.AddQueryParameter("before", value);
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IThreadQueryResponse>> ListPrivateArchivedThreadsAsync
        (
            Snowflake channelID,
            Optional<DateTimeOffset> before = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IThreadQueryResponse>
            (
                $"channels/{channelID}/threads/archived/private",
                b =>
                {
                    if (before.HasValue)
                    {
                        var offset = before.Value.Offset;
                        var value = before.Value.ToString
                        (
                            $"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'+'{offset.Hours:D2}':'{offset.Minutes:D2}"
                        );

                        b.AddQueryParameter("before", value);
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IThreadQueryResponse>> ListJoinedPrivateArchivedThreadsAsync
        (
            Snowflake channelID,
            Optional<DateTimeOffset> before = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IThreadQueryResponse>
            (
                $"channels/{channelID}/users/@me/threads/archived/private",
                b =>
                {
                    if (before.HasValue)
                    {
                        var offset = before.Value.Offset;
                        var value = before.Value.ToString
                        (
                            $"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'+'{offset.Hours:D2}':'{offset.Minutes:D2}"
                        );

                        b.AddQueryParameter("before", value);
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct: ct
            );
        }
    }
}
