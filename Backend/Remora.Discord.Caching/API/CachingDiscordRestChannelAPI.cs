//
//  CachingDiscordRestChannelAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered channel API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestChannelAPI : IDiscordRestChannelAPI, IRestCustomizable
{
    private readonly IDiscordRestChannelAPI _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestChannelAPI"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestChannelAPI
    (
        IDiscordRestChannelAPI actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> GetChannelAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateChannelCacheKey(channelID);

        var cacheResult = await _cacheService.TryGetValueAsync<IChannel>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getChannel = await _actual.GetChannelAsync(channelID, ct);
        if (!getChannel.IsSuccess)
        {
            return getChannel;
        }

        var channel = getChannel.Entity;
        await _cacheService.CacheAsync(key, channel, ct);

        return getChannel;
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> ModifyChannelAsync
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
        var modificationResult = await _actual.ModifyChannelAsync
        (
            channelID,
            name,
            icon,
            type,
            position,
            topic,
            isNsfw,
            rateLimitPerUser,
            bitrate,
            userLimit,
            permissionOverwrites,
            parentID,
            videoQualityMode,
            isArchived,
            autoArchiveDuration,
            isLocked,
            isInvitable,
            defaultAutoArchiveDuration,
            rtcRegion,
            flags,
            reason,
            ct
        );

        if (!modificationResult.IsSuccess)
        {
            return modificationResult;
        }

        var channel = modificationResult.Entity;
        var key = KeyHelpers.CreateChannelCacheKey(channelID);
        await _cacheService.CacheAsync(key, channel, ct);

        return modificationResult;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteChannelAsync
    (
        Snowflake channelID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.DeleteChannelAsync(channelID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateChannelCacheKey(channelID);
        await _cacheService.EvictAsync<IChannel>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> GetChannelMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);

        var cacheResult = await _cacheService.TryGetValueAsync<IMessage>(key, ct);
        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getMessage = await _actual.GetChannelMessageAsync(channelID, messageID, ct);
        if (!getMessage.IsSuccess)
        {
            return getMessage;
        }

        var message = getMessage.Entity;
        await _cacheService.CacheAsync(key, message, ct);

        return getMessage;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> CreateMessageAsync
    (
        Snowflake channelID,
        Optional<string> content = default,
        Optional<string> nonce = default,
        Optional<bool> isTTS = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        Optional<IAllowedMentions> allowedMentions = default,
        Optional<IMessageReference> messageReference = default,
        Optional<IReadOnlyList<IMessageComponent>> components = default,
        Optional<IReadOnlyList<Snowflake>> stickerIds = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        Optional<MessageFlags> flags = default,
        CancellationToken ct = default
    )
    {
        var createResult = await _actual.CreateMessageAsync
        (
            channelID,
            content,
            nonce,
            isTTS,
            embeds,
            allowedMentions,
            messageReference,
            components,
            stickerIds,
            attachments,
            flags,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var message = createResult.Entity;
        var key = KeyHelpers.CreateMessageCacheKey(channelID, message.ID);
        await _cacheService.CacheAsync(key, message, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> EditMessageAsync
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
        var editResult = await _actual.EditMessageAsync
        (
            channelID,
            messageID,
            content,
            embeds,
            flags,
            allowedMentions,
            components,
            attachments,
            ct
        );

        if (!editResult.IsSuccess)
        {
            return editResult;
        }

        var message = editResult.Entity;
        var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
        await _cacheService.CacheAsync(key, message, ct);

        return editResult;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.DeleteMessageAsync(channelID, messageID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
        await _cacheService.EvictAsync<IMessage>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result> BulkDeleteMessagesAsync
    (
        Snowflake channelID,
        IReadOnlyList<Snowflake> messageIDs,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.BulkDeleteMessagesAsync(channelID, messageIDs, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        foreach (var messageID in messageIDs)
        {
            var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
            await _cacheService.EvictAsync<IMessage>(key, ct);
        }

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IInvite>> CreateChannelInviteAsync
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
        var createResult = await _actual.CreateChannelInviteAsync
        (
            channelID,
            maxAge,
            maxUses,
            isTemporary,
            isUnique,
            targetType,
            targetUserID,
            targetApplicationID,
            reason,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var invite = createResult.Entity;
        var key = KeyHelpers.CreateInviteCacheKey(invite.Code);
        await _cacheService.CacheAsync(key, invite, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteChannelPermissionAsync
    (
        Snowflake channelID,
        Snowflake overwriteID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.DeleteChannelPermissionAsync(channelID, overwriteID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateChannelPermissionCacheKey(channelID, overwriteID);
        await _cacheService.EvictAsync<IPermissionOverwrite>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreatePinnedMessagesCacheKey(channelID);

        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IMessage>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetPinnedMessagesAsync(channelID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var messages = getResult.Entity;
        await _cacheService.CacheAsync(key, messages, ct);

        foreach (var message in messages)
        {
            var messageKey = KeyHelpers.CreateMessageCacheKey(channelID, message.ID);
            await _cacheService.CacheAsync(messageKey, message, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result> UnpinMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.UnpinMessageAsync(channelID, messageID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
        await _cacheService.EvictAsync<IMessage>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> StartThreadWithMessageAsync
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
        var createResult = await _actual.StartThreadWithMessageAsync
        (
            channelID,
            messageID,
            name,
            autoArchiveDuration,
            rateLimitPerUser,
            reason,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var key = KeyHelpers.CreateChannelCacheKey(channelID);
        await _cacheService.CacheAsync(key, createResult.Entity, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> StartThreadWithoutMessageAsync
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
        var createResult = await _actual.StartThreadWithoutMessageAsync
        (
            channelID,
            name,
            type,
            autoArchiveDuration,
            isInvitable,
            rateLimitPerUser,
            reason,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var key = KeyHelpers.CreateChannelCacheKey(channelID);
        await _cacheService.CacheAsync(key, createResult.Entity, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
    (
        Snowflake channelID,
        Optional<Snowflake> around = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        var getResult = await _actual.GetChannelMessagesAsync(channelID, around, before, after, limit, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        foreach (var message in getResult.Entity)
        {
            var key = KeyHelpers.CreateMessageCacheKey(channelID, message.ID);
            await _cacheService.CacheAsync(key, message, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> CrosspostMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        var result = await _actual.CrosspostMessageAsync(channelID, messageID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        var message = result.Entity;
        var key = KeyHelpers.CreateMessageCacheKey(message.ChannelID, message.ID);
        await _cacheService.CacheAsync(key, message, ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateChannelInvitesCacheKey(channelID);

        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IInvite>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var result = await _actual.GetChannelInvitesAsync(channelID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _cacheService.CacheAsync(key, result.Entity, ct);

        foreach (var invite in result.Entity)
        {
            var inviteKey = KeyHelpers.CreateInviteCacheKey(invite.Code);
            await _cacheService.CacheAsync(inviteKey, invite, ct);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<Result<IThreadMember>> GetThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateThreadMemberCacheKey(channelID, userID);

        var cacheResult = await _cacheService.TryGetValueAsync<IThreadMember>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var result = await _actual.GetThreadMemberAsync(channelID, userID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        var threadMember = result.Entity;
        await _cacheService.CacheAsync(key, threadMember, ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IThreadMember>>> ListThreadMembersAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateThreadMembersCacheKey(channelID);

        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IThreadMember>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var result = await _actual.ListThreadMembersAsync(channelID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        foreach (var threadMember in result.Entity)
        {
            if (!threadMember.UserID.IsDefined(out var userID))
            {
                continue;
            }

            var memberKey = KeyHelpers.CreateThreadMemberCacheKey(channelID, userID);
            await _cacheService.CacheAsync(memberKey, threadMember, ct);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<Result> RemoveThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        var result = await _actual.RemoveThreadMemberAsync(channelID, userID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        var key = KeyHelpers.CreateThreadMemberCacheKey(channelID, userID);
        await _cacheService.EvictAsync<IThreadMember>(key, ct);

        return result;
    }
}
