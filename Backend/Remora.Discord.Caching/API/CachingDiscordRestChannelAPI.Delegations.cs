//
//  CachingDiscordRestChannelAPI.Delegations.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

public partial class CachingDiscordRestChannelAPI
{
    /// <inheritdoc />
    public Task<Result<IChannel>> ModifyGroupDMChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<Stream> icon = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGroupDMChannelAsync(channelID, name, icon, ct);
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
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentId = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildTextChannelAsync
        (
            channelID,
            name,
            type,
            position,
            topic,
            isNsfw,
            rateLimitPerUser,
            permissionOverwrites,
            parentId,
            defaultAutoArchiveDuration,
            reason,
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IChannel>> ModifyGuildVoiceChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<int?> position = default,
        Optional<bool?> isNsfw = default,
        Optional<int?> bitrate = default,
        Optional<int?> userLimit = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentId = default,
        Optional<string?> rtcRegion = default,
        Optional<VideoQualityMode?> videoQualityMode = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildVoiceChannelAsync
        (
            channelID,
            name,
            position,
            isNsfw,
            bitrate,
            userLimit,
            permissionOverwrites,
            parentId,
            rtcRegion,
            videoQualityMode,
            reason,
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IChannel>> ModifyGuildStageChannelAsync
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
        return _actual.ModifyGuildStageChannelAsync
        (
            channelID,
            name,
            position,
            bitrate,
            permissionOverwrites,
            rtcRegion,
            reason,
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IChannel>> ModifyGuildAnnouncementChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<ChannelType> type = default,
        Optional<int?> position = default,
        Optional<string?> topic = default,
        Optional<bool?> isNsfw = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentId = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildAnnouncementChannelAsync
        (
            channelID,
            name,
            type,
            position,
            topic,
            isNsfw,
            permissionOverwrites,
            parentId,
            defaultAutoArchiveDuration,
            reason,
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IChannel>> ModifyThreadChannelAsync
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
        return _actual.ModifyThreadChannelAsync
        (
            channelID,
            name,
            isArchived,
            autoArchiveDuration,
            isLocked,
            isInvitable,
            rateLimitPerUser,
            flags,
            reason,
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> CreateReactionAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
        CancellationToken ct = default
    )
    {
        return _actual.CreateReactionAsync(channelID, messageID, emoji, ct);
    }

    /// <inheritdoc />
    public Task<Result> DeleteOwnReactionAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
        CancellationToken ct = default
    )
    {
        return _actual.DeleteOwnReactionAsync(channelID, messageID, emoji, ct);
    }

    /// <inheritdoc />
    public Task<Result> DeleteUserReactionAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
        Snowflake user,
        CancellationToken ct = default
    )
    {
        return _actual.DeleteUserReactionAsync(channelID, messageID, emoji, user, ct);
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IUser>>> GetReactionsAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return _actual.GetReactionsAsync(channelID, messageID, emoji, after, limit, ct);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAllReactionsAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        return _actual.DeleteAllReactionsAsync(channelID, messageID, ct);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAllReactionsForEmojiAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
        CancellationToken ct = default
    )
    {
        return _actual.DeleteAllReactionsForEmojiAsync(channelID, messageID, emoji, ct);
    }

    /// <inheritdoc />
    public Task<Result> EditChannelPermissionsAsync
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
        return _actual.EditChannelPermissionsAsync(channelID, overwriteID, allow, deny, type, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result<IFollowedChannel>> FollowAnnouncementChannelAsync
    (
        Snowflake channelID,
        Snowflake webhookChannelID,
        CancellationToken ct = default
    )
    {
        return _actual.FollowAnnouncementChannelAsync(channelID, webhookChannelID, ct);
    }

    /// <inheritdoc />
    public Task<Result> TriggerTypingIndicatorAsync(Snowflake channelID, CancellationToken ct = default)
    {
        return _actual.TriggerTypingIndicatorAsync(channelID, ct);
    }

    /// <inheritdoc />
    public Task<Result> PinMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.PinMessageAsync(channelID, messageID, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result> GroupDMAddRecipientAsync
    (
        Snowflake channelID,
        Snowflake userID,
        string accessToken,
        Optional<string> nickname = default,
        CancellationToken ct = default
    )
    {
        return _actual.GroupDMAddRecipientAsync(channelID, userID, accessToken, nickname, ct);
    }

    /// <inheritdoc />
    public Task<Result> GroupDMRemoveRecipientAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return _actual.GroupDMRemoveRecipientAsync(channelID, userID, ct);
    }

    /// <inheritdoc />
    public Task<Result> JoinThreadAsync(Snowflake channelID, CancellationToken ct = default)
    {
        return _actual.JoinThreadAsync(channelID, ct);
    }

    /// <inheritdoc />
    public Task<Result> AddThreadMemberAsync(Snowflake channelID, Snowflake userID, CancellationToken ct = default)
    {
        return _actual.AddThreadMemberAsync(channelID, userID, ct);
    }

    /// <inheritdoc />
    public Task<Result> LeaveThreadAsync(Snowflake channelID, CancellationToken ct = default)
    {
        return _actual.LeaveThreadAsync(channelID, ct);
    }

    /// <inheritdoc />
    public Task<Result<IChannelThreadQueryResponse>> ListPublicArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<DateTimeOffset> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return _actual.ListPublicArchivedThreadsAsync(channelID, before, limit, ct);
    }

    /// <inheritdoc />
    public Task<Result<IChannelThreadQueryResponse>> ListPrivateArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<DateTimeOffset> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return _actual.ListPrivateArchivedThreadsAsync(channelID, before, limit, ct);
    }

    /// <inheritdoc />
    public Task<Result<IChannelThreadQueryResponse>> ListJoinedPrivateArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<Snowflake> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        return _actual.ListJoinedPrivateArchivedThreadsAsync(channelID, before, limit, ct);
    }

    /// <inheritdoc/>
    public RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            // TODO: not ideal...
            throw new NotImplementedException("The decorated API type is not customizable.");
        }

        return customizable.WithCustomization(requestCustomizer);
    }

    /// <inheritdoc/>
    void IRestCustomizable.RemoveCustomization(RestRequestCustomization customization)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            return;
        }

        customizable.RemoveCustomization(customization);
    }
}
