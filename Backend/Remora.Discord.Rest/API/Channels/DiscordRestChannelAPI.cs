//
//  DiscordRestChannelAPI.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Utility;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestChannelAPI" />
[PublicAPI]
public class DiscordRestChannelAPI : AbstractDiscordRestAPI, IDiscordRestChannelAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestChannelAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestChannelAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> GetChannelAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IChannel>
        (
            $"channels/{channelID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
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
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<VideoQualityMode?> videoQualityMode = default,
        Optional<bool> isArchived = default,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<bool> isLocked = default,
        Optional<bool> isInvitable = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string?> rtcRegion = default,
        Optional<ChannelFlags> flags = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        if (name.HasValue && name.Value.Length is > 100 or < 1)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters.");
        }

        if (topic.HasValue && topic.Value?.Length is > 1024 or < 0)
        {
            return new ArgumentOutOfRangeError(nameof(topic), "The topic must be between 0 and 1024 characters.");
        }

        if (userLimit.HasValue && userLimit.Value is > 99 or < 0)
        {
            return new ArgumentOutOfRangeError(nameof(userLimit), "The user limit must be between 0 and 99.");
        }

        var packImage = await ImagePacker.PackImageAsync(icon!, ct);
        if (!packImage.IsSuccess)
        {
            return Result<IChannel>.FromError(packImage);
        }

        Optional<string> base64EncodedIcon = packImage.Entity!;

        return await this.RestHttpClient.PatchAsync<IChannel>
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
                        json.Write("parent_id", parentID, this.JsonOptions);
                        json.Write("video_quality_mode", videoQualityMode, this.JsonOptions);
                        json.Write("archived", isArchived, this.JsonOptions);
                        json.Write("auto_archive_duration", autoArchiveDuration);
                        json.Write("locked", isLocked, this.JsonOptions);
                        json.Write("invitable", isInvitable, this.JsonOptions);
                        json.Write("default_auto_archive_duration", defaultAutoArchiveDuration);
                        json.Write("rtc_region", rtcRegion, this.JsonOptions);
                        json.Write("flags", flags, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> ModifyGroupDMChannelAsync
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
    public virtual Task<Result<IChannel>> ModifyGuildTextChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<ChannelType> type = default,
        Optional<int?> position = default,
        Optional<string?> topic = default,
        Optional<bool?> isNsfw = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
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
            parentID: parentID,
            defaultAutoArchiveDuration: defaultAutoArchiveDuration,
            reason: reason,
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> ModifyGuildVoiceChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<int?> position = default,
        Optional<bool?> isNsfw = default,
        Optional<int?> bitrate = default,
        Optional<int?> userLimit = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
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
            isNsfw: isNsfw,
            bitrate: bitrate,
            userLimit: userLimit,
            permissionOverwrites: permissionOverwrites,
            parentID: parentID,
            rtcRegion: rtcRegion,
            videoQualityMode: videoQualityMode,
            reason: reason,
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> ModifyGuildStageChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<int?> position = default,
        Optional<int?> bitrate = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<string?> rtcRegion = default,
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
            permissionOverwrites: permissionOverwrites,
            rtcRegion: rtcRegion,
            reason: reason,
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> ModifyGuildAnnouncementChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<ChannelType> type = default,
        Optional<int?> position = default,
        Optional<string?> topic = default,
        Optional<bool?> isNsfw = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
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
            parentID: parentID,
            defaultAutoArchiveDuration: defaultAutoArchiveDuration,
            reason: reason,
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> ModifyThreadChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<bool> isArchived = default,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<bool> isLocked = default,
        Optional<bool> isInvitable = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<ChannelFlags> flags = default,
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
            isInvitable: isInvitable,
            rateLimitPerUser: rateLimitPerUser,
            flags: flags,
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
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
        var hasStrictlyOne = (around.HasValue ^ before.HasValue ^ after.HasValue) && !(around.HasValue && before.HasValue && after.HasValue);
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

        return await this.RestHttpClient.GetAsync<IReadOnlyList<IMessage>>
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

                b.WithRateLimitContext(this.RateLimitCache);
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
        return this.RestHttpClient.GetAsync<IMessage>
        (
            $"channels/{channelID}/messages/{messageID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
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
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        Optional<IAllowedMentions> allowedMentions = default,
        Optional<IMessageReference> messageReference = default,
        Optional<IReadOnlyList<IMessageComponent>> components = default,
        Optional<IReadOnlyList<Snowflake>> stickerIDs = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        Optional<MessageFlags> flags = default,
        CancellationToken ct = default
    )
    {
        if (nonce.HasValue && nonce.Value.Length > 25)
        {
            return new ArgumentOutOfRangeError(nameof(nonce), "The nonce length must be less than 25 characters.");
        }

        if (!content.HasValue && !attachments.HasValue && !embeds.HasValue && !stickerIDs.HasValue)
        {
            return new InvalidOperationError
            (
                $"At least one of {nameof(content)}, {nameof(attachments)}, {nameof(embeds)}, or " +
                $"{nameof(stickerIDs)} is required."
            );
        }

        return await this.RestHttpClient.PostAsync<IMessage>
        (
            $"channels/{channelID}/messages",
            b =>
            {
                Optional<IReadOnlyList<IPartialAttachment>> attachmentList = default;
                if (attachments.HasValue)
                {
                    // build attachment list
                    attachmentList = attachments.Value.Select
                    (
                        (f, i) => f.Match
                        (
                            data => new PartialAttachment(DiscordSnowflake.New((ulong)i), data.Name, data.Description),
                            attachment => attachment
                        )
                    ).ToList();

                    for (var i = 0; i < attachments.Value.Count; i++)
                    {
                        if (!attachments.Value[i].IsT0)
                        {
                            continue;
                        }

                        var (name, stream, _) = attachments.Value[i].AsT0;
                        var contentName = $"files[{i}]";

                        b.AddContent(new StreamContent(stream), contentName, name);
                    }
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
                            json.Write("sticker_ids", stickerIDs, this.JsonOptions);
                            json.Write("attachments", attachmentList, this.JsonOptions);
                            json.Write("flags", flags, this.JsonOptions);
                        }
                    )
                    .WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IMessage>> CrosspostMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        return await this.RestHttpClient.PostAsync<IMessage>
        (
            $"channels/{channelID}/messages/{messageID}/crosspost",
            b => b.WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.PutAsync
        (
            $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/{user}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
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

        return await this.RestHttpClient.GetAsync<IReadOnlyList<IUser>>
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

                b.WithRateLimitContext(this.RateLimitCache);
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/messages/{messageID}/reactions",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IMessage>> EditMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<MessageFlags?> flags = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    )
    {
        if (!content.HasValue && !attachments.HasValue && !embeds.HasValue)
        {
            return new InvalidOperationError
            (
                $"At least one of {nameof(content)}, {nameof(attachments)}, or {nameof(embeds)} is required."
            );
        }

        return await this.RestHttpClient.PatchAsync<IMessage>
        (
            $"channels/{channelID}/messages/{messageID}",
            b =>
            {
                Optional<IReadOnlyList<IPartialAttachment>?> attachmentList = default;
                if (attachments.HasValue && attachments.Value is null)
                {
                    attachmentList = null;
                }
                else if (attachments.HasValue && attachments.Value is not null)
                {
                    // build attachment list
                    attachmentList = attachments.Value.Select
                    (
                        (f, i) => f.Match
                        (
                            data => new PartialAttachment(DiscordSnowflake.New((ulong)i), data.Name, data.Description),
                            attachment => attachment
                        )
                    ).ToList();

                    for (var i = 0; i < attachments.Value.Count; i++)
                    {
                        if (!attachments.Value[i].IsT0)
                        {
                            continue;
                        }

                        var (name, stream, _) = attachments.Value[i].AsT0;
                        var contentName = $"files[{i}]";

                        b.AddContent(new StreamContent(stream), contentName, name);
                    }
                }

                b.WithJson
                    (
                        json =>
                        {
                            json.Write("content", content, this.JsonOptions);
                            json.Write("embeds", embeds, this.JsonOptions);
                            json.Write("flags", flags, this.JsonOptions);
                            json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                            json.Write("components", components, this.JsonOptions);
                            json.Write("attachments", attachmentList, this.JsonOptions);
                        }
                    )
                    .WithRateLimitContext(this.RateLimitCache);
            },
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/messages/{messageID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
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

        return await this.RestHttpClient.PostAsync
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
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> EditChannelPermissionsAsync
    (
        Snowflake channelID,
        Snowflake overwriteID,
        Optional<IDiscordPermissionSet?> allow = default,
        Optional<IDiscordPermissionSet?> deny = default,
        Optional<PermissionOverwriteType> type = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync
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
                )
                .WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.GetAsync<IReadOnlyList<IInvite>>
        (
            $"channels/{channelID}/invites",
            b => b.WithRateLimitContext(this.RateLimitCache),
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

        return await this.RestHttpClient.PostAsync<IInvite>
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
                )
                .WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/permissions/{overwriteID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IFollowedChannel>> FollowAnnouncementChannelAsync
    (
        Snowflake channelID,
        Snowflake webhookChannelID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IFollowedChannel>
        (
            $"channels/{channelID}/followers",
            b => b.WithJson
                (
                    p =>
                    {
                        p.WriteString("webhook_channel_id", webhookChannelID.ToString());
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.PostAsync
        (
            $"channels/{channelID}/typing",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IMessage>>
        (
            $"channels/{channelID}/pins",
            b => b.WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.PutAsync
        (
            $"channels/{channelID}/pins/{messageID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/pins/{messageID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.PutAsync
        (
            $"channels/{channelID}/recipients/{userID}",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("access_token", accessToken);
                        json.Write("nick", nickname, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/recipients/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IChannel>> StartThreadWithMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string name,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        if (name.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters");
        }

        return await this.RestHttpClient.PostAsync<IChannel>
        (
            $"channels/{channelID}/messages/{messageID}/threads",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.Write("auto_archive_duration", autoArchiveDuration, this.JsonOptions);
                        json.Write("rate_limit_per_user", rateLimitPerUser, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IChannel>> StartThreadWithoutMessageAsync
    (
        Snowflake channelID,
        string name,
        ChannelType type,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<bool> isInvitable = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        if (name.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters");
        }

        return await this.RestHttpClient.PostAsync<IChannel>
        (
            $"channels/{channelID}/threads",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.Write("type", type, this.JsonOptions);
                        json.Write("auto_archive_duration", autoArchiveDuration, this.JsonOptions);
                        json.Write("invitable", isInvitable, this.JsonOptions);
                        json.Write("rate_limit_per_user", rateLimitPerUser, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> JoinThreadAsync(Snowflake channelID, CancellationToken ct = default)
    {
        return this.RestHttpClient.PutAsync
        (
            $"channels/{channelID}/thread-members/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> AddThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync
        (
            $"channels/{channelID}/thread-members/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> LeaveThreadAsync(Snowflake channelID, CancellationToken ct = default)
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/thread-members/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> RemoveThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"channels/{channelID}/thread-members/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IThreadMember>> GetThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IThreadMember>
        (
            $"channels/{channelID}/thread-members/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
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
        return this.RestHttpClient.GetAsync<IReadOnlyList<IThreadMember>>
        (
            $"channels/{channelID}/thread-members",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannelThreadQueryResponse>> ListPublicArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<DateTimeOffset> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IChannelThreadQueryResponse>
        (
            $"channels/{channelID}/threads/archived/public",
            b =>
            {
                if (before.HasValue)
                {
                    b.AddQueryParameter("before", before.Value.ToISO8601String());
                }

                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannelThreadQueryResponse>> ListPrivateArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<DateTimeOffset> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IChannelThreadQueryResponse>
        (
            $"channels/{channelID}/threads/archived/private",
            b =>
            {
                if (before.HasValue)
                {
                    b.AddQueryParameter("before", before.Value.ToISO8601String());
                }

                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannelThreadQueryResponse>> ListJoinedPrivateArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<Snowflake> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IChannelThreadQueryResponse>
        (
            $"channels/{channelID}/users/@me/threads/archived/private",
            b =>
            {
                if (before.HasValue)
                {
                    b.AddQueryParameter("before", before.Value.ToString());
                }

                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }
}
