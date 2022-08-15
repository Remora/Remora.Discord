//
//  CachingDiscordRestGuildAPI.cs
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
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered guild API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestGuildAPI : IDiscordRestGuildAPI, IRestCustomizable
{
    private readonly IDiscordRestGuildAPI _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestGuildAPI"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestGuildAPI
    (
        IDiscordRestGuildAPI actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IGuild>> CreateGuildAsync
    (
        string name,
        Optional<Stream> icon = default,
        Optional<VerificationLevel> verificationLevel = default,
        Optional<MessageNotificationLevel> defaultMessageNotifications = default,
        Optional<ExplicitContentFilterLevel> explicitContentFilter = default,
        Optional<IReadOnlyList<IRole>> roles = default,
        Optional<IReadOnlyList<IPartialChannel>> channels = default,
        Optional<Snowflake> afkChannelID = default,
        Optional<TimeSpan> afkTimeout = default,
        Optional<Snowflake> systemChannelID = default,
        Optional<SystemChannelFlags> systemChannelFlags = default,
        CancellationToken ct = default
    )
    {
        var createResult = await _actual.CreateGuildAsync
        (
            name,
            icon,
            verificationLevel,
            defaultMessageNotifications,
            explicitContentFilter,
            roles,
            channels,
            afkChannelID,
            afkTimeout,
            systemChannelID,
            systemChannelFlags,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var guild = createResult.Entity;
        var key = KeyHelpers.CreateGuildCacheKey(guild.ID);
        await _cacheService.CacheAsync(key, guild, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuild>> GetGuildAsync
    (
        Snowflake guildID,
        Optional<bool> withCounts = default,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IGuild>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildAsync(guildID, withCounts, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var guild = getResult.Entity;
        await _cacheService.CacheAsync(key, guild, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildPreview>> GetGuildPreviewAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildPreviewCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IGuildPreview>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildPreviewAsync(guildID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var guildPreview = getResult.Entity;
        await _cacheService.CacheAsync(key, guildPreview, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuild>> ModifyGuildAsync
    (
        Snowflake guildID,
        Optional<string> name = default,
        Optional<VerificationLevel?> verificationLevel = default,
        Optional<MessageNotificationLevel?> defaultMessageNotifications = default,
        Optional<ExplicitContentFilterLevel?> explicitContentFilter = default,
        Optional<Snowflake?> afkChannelID = default,
        Optional<TimeSpan> afkTimeout = default,
        Optional<Stream?> icon = default,
        Optional<Snowflake> ownerID = default,
        Optional<Stream?> splash = default,
        Optional<Stream?> discoverySplash = default,
        Optional<Stream?> banner = default,
        Optional<Snowflake?> systemChannelID = default,
        Optional<SystemChannelFlags> systemChannelFlags = default,
        Optional<Snowflake?> rulesChannelID = default,
        Optional<Snowflake?> publicUpdatesChannelID = default,
        Optional<string?> preferredLocale = default,
        Optional<IReadOnlyList<GuildFeature>> features = default,
        Optional<string?> description = default,
        Optional<bool> isPremiumProgressBarEnabled = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyResult = await _actual.ModifyGuildAsync
        (
            guildID,
            name,
            verificationLevel,
            defaultMessageNotifications,
            explicitContentFilter,
            afkChannelID,
            afkTimeout,
            icon,
            ownerID,
            splash,
            discoverySplash,
            banner,
            systemChannelID,
            systemChannelFlags,
            rulesChannelID,
            publicUpdatesChannelID,
            preferredLocale,
            features,
            description,
            isPremiumProgressBarEnabled,
            reason,
            ct
        );

        if (!modifyResult.IsSuccess)
        {
            return modifyResult;
        }

        var guild = modifyResult.Entity;
        var key = KeyHelpers.CreateGuildCacheKey(guild.ID);
        await _cacheService.CacheAsync(key, guild, ct);

        return modifyResult;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteGuildAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.DeleteGuildAsync(guildID, ct);

        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateGuildCacheKey(guildID);
        await _cacheService.EvictAsync<IGuild>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IChannel>>> GetGuildChannelsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildChannelsCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IChannel>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildChannelsAsync(guildID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var channels = getResult.Entity;
        await _cacheService.CacheAsync(key, channels, ct);

        foreach (var channel in channels)
        {
            var channelKey = KeyHelpers.CreateChannelCacheKey(channel.ID);
            await _cacheService.CacheAsync(channelKey, channel, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> CreateGuildChannelAsync
    (
        Snowflake guildID,
        string name,
        Optional<ChannelType?> type = default,
        Optional<string?> topic = default,
        Optional<int?> bitrate = default,
        Optional<int?> userLimit = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<int?> position = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<bool?> isNsfw = default,
        Optional<string?> rtcRegion = default,
        Optional<VideoQualityMode?> videoQualityMode = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var createResult = await _actual.CreateGuildChannelAsync
        (
            guildID,
            name,
            type,
            topic,
            bitrate,
            userLimit,
            rateLimitPerUser,
            position,
            permissionOverwrites,
            parentID,
            isNsfw,
            rtcRegion,
            videoQualityMode,
            defaultAutoArchiveDuration,
            reason,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var guild = createResult.Entity;
        var key = KeyHelpers.CreateGuildCacheKey(guild.ID);
        await _cacheService.CacheAsync(key, guild, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildMember>> GetGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildMemberKey(guildID, userID);
        var cacheResult = await _cacheService.TryGetValueAsync<IGuildMember>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildMemberAsync(guildID, userID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var guildMember = getResult.Entity;
        await _cacheService.CacheAsync(key, guildMember, ct);

        if (!guildMember.User.IsDefined(out var user))
        {
            return getResult;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await _cacheService.CacheAsync(userKey, user, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IGuildMember>>> ListGuildMembersAsync
    (
        Snowflake guildID,
        Optional<int> limit = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    )
    {
        var collectionKey = KeyHelpers.CreateGuildMembersKey(guildID, limit, after);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IGuildMember>>(collectionKey, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.ListGuildMembersAsync(guildID, limit, after, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var members = getResult.Entity;
        await _cacheService.CacheAsync(collectionKey, members, ct);

        foreach (var member in members)
        {
            if (!member.User.IsDefined(out var user))
            {
                continue;
            }

            var key = KeyHelpers.CreateGuildMemberKey(guildID, user.ID);
            await _cacheService.CacheAsync(key, member, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildMember?>> AddGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        string accessToken,
        Optional<string> nickname = default,
        Optional<IReadOnlyList<Snowflake>> roles = default,
        Optional<bool> isMuted = default,
        Optional<bool> isDeafened = default,
        CancellationToken ct = default
    )
    {
        var getResult = await _actual.AddGuildMemberAsync
        (
            guildID,
            userID,
            accessToken,
            nickname,
            roles,
            isMuted,
            isDeafened,
            ct
        );

        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var member = getResult.Entity;
        if (member is null)
        {
            return getResult;
        }

        var key = KeyHelpers.CreateGuildMemberKey(guildID, userID);
        await _cacheService.CacheAsync(key, member, ct);
        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildMember>> ModifyCurrentMemberAsync
    (
        Snowflake guildID,
        Optional<string?> nickname = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var getResult = await _actual.ModifyCurrentMemberAsync(guildID, nickname, reason, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var guildMember = getResult.Entity;

        if (!guildMember.User.IsDefined(out var user))
        {
            return getResult;
        }

        var key = KeyHelpers.CreateGuildMemberKey(guildID, user.ID);
        await _cacheService.CacheAsync(key, guildMember, ct);

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await _cacheService.CacheAsync(userKey, user, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result> RemoveGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var removeMember = await _actual.RemoveGuildMemberAsync(guildID, userID, reason, ct);
        if (!removeMember.IsSuccess)
        {
            return removeMember;
        }

        var memberKey = KeyHelpers.CreateGuildMemberKey(guildID, userID);
        await _cacheService.EvictAsync<IGuildMember>(memberKey, ct);

        return removeMember;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IBan>>> GetGuildBansAsync
    (
        Snowflake guildID,
        Optional<int> limit = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    )
    {
        var collectionKey = KeyHelpers.CreateGuildBansCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IBan>>(collectionKey, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildBansAsync(guildID, limit, before, after, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var bans = getResult.Entity;
        await _cacheService.CacheAsync(collectionKey, bans, ct);

        foreach (var ban in bans)
        {
            var key = KeyHelpers.CreateGuildBanCacheKey(guildID, ban.User.ID);
            await _cacheService.CacheAsync(key, ban, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IBan>> GetGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildBanCacheKey(guildID, userID);
        var cacheResult = await _cacheService.TryGetValueAsync<IBan>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildBanAsync(guildID, userID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var ban = getResult.Entity;
        await _cacheService.CacheAsync(key, ban, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result> RemoveGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.RemoveGuildBanAsync(guildID, userID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateGuildBanCacheKey(guildID, userID);
        await _cacheService.EvictAsync<IBan>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IRole>>> GetGuildRolesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var collectionKey = KeyHelpers.CreateGuildRolesCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IRole>>(collectionKey, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getRoles = await _actual.GetGuildRolesAsync(guildID, ct);
        if (!getRoles.IsSuccess)
        {
            return getRoles;
        }

        var roles = getRoles.Entity;
        await _cacheService.CacheAsync(collectionKey, roles, ct);

        foreach (var role in roles)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(guildID, role.ID);
            await _cacheService.CacheAsync(key, role, ct);
        }

        return getRoles;
    }

    /// <inheritdoc />
    public async Task<Result<IRole>> CreateGuildRoleAsync
    (
        Snowflake guildID,
        Optional<string> name = default,
        Optional<IDiscordPermissionSet> permissions = default,
        Optional<Color> colour = default,
        Optional<bool> isHoisted = default,
        Optional<Stream?> icon = default,
        Optional<string?> unicodeEmoji = default,
        Optional<bool> isMentionable = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var createResult = await _actual.CreateGuildRoleAsync
        (
            guildID,
            name,
            permissions,
            colour,
            isHoisted,
            icon,
            unicodeEmoji,
            isMentionable,
            reason,
            ct
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var role = createResult.Entity;
        var key = KeyHelpers.CreateGuildRoleCacheKey(guildID, role.ID);
        await _cacheService.CacheAsync(key, role, ct);

        return createResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IRole>>> ModifyGuildRolePositionsAsync
    (
        Snowflake guildID,
        IReadOnlyList<(Snowflake RoleID, Optional<int?> Position)> modifiedPositions,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyResult = await _actual.ModifyGuildRolePositionsAsync(guildID, modifiedPositions, reason, ct);

        if (!modifyResult.IsSuccess)
        {
            return modifyResult;
        }

        var roles = modifyResult.Entity;
        var collectionKey = KeyHelpers.CreateGuildRolesCacheKey(guildID);
        await _cacheService.CacheAsync(collectionKey, roles, ct);

        foreach (var role in roles)
        {
            var key = KeyHelpers.CreateGuildRoleCacheKey(guildID, role.ID);
            await _cacheService.CacheAsync(key, role, ct);
        }

        return modifyResult;
    }

    /// <inheritdoc />
    public async Task<Result<IRole>> ModifyGuildRoleAsync
    (
        Snowflake guildID,
        Snowflake roleID,
        Optional<string?> name = default,
        Optional<IDiscordPermissionSet?> permissions = default,
        Optional<Color?> colour = default,
        Optional<bool?> isHoisted = default,
        Optional<Stream?> icon = default,
        Optional<string?> unicodeEmoji = default,
        Optional<bool?> isMentionable = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyResult = await _actual.ModifyGuildRoleAsync
        (
            guildID,
            roleID,
            name,
            permissions,
            colour,
            isHoisted,
            icon,
            unicodeEmoji,
            isMentionable,
            reason,
            ct
        );

        if (!modifyResult.IsSuccess)
        {
            return modifyResult;
        }

        var role = modifyResult.Entity;
        var key = KeyHelpers.CreateGuildRoleCacheKey(guildID, roleID);
        await _cacheService.CacheAsync(key, role, ct);

        return modifyResult;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteGuildRoleAsync
    (
        Snowflake guildId,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.DeleteGuildRoleAsync(guildId, roleID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateGuildRoleCacheKey(guildId, roleID);
        await _cacheService.EvictAsync<IRole>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IVoiceRegion>>> GetGuildVoiceRegionsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var collectionKey = KeyHelpers.CreateGuildVoiceRegionsCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IVoiceRegion>>(collectionKey, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildVoiceRegionsAsync(guildID, ct);

        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var voiceRegions = getResult.Entity;
        await _cacheService.CacheAsync(collectionKey, voiceRegions, ct);

        foreach (var voiceRegion in voiceRegions)
        {
            var key = KeyHelpers.CreateGuildVoiceRegionCacheKey(guildID, voiceRegion.ID);
            await _cacheService.CacheAsync(key, voiceRegion, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IInvite>>> GetGuildInvitesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var collectionKey = KeyHelpers.CreateGuildInvitesCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IInvite>>(collectionKey, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildInvitesAsync(guildID, ct);

        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var invites = getResult.Entity;
        await _cacheService.CacheAsync(collectionKey, invites, ct);

        foreach (var invite in invites)
        {
            var key = KeyHelpers.CreateInviteCacheKey(invite.Code);
            await _cacheService.CacheAsync(key, invite, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IIntegration>>> GetGuildIntegrationsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var collectionKey = KeyHelpers.CreateGuildIntegrationsCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IIntegration>>(collectionKey, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildIntegrationsAsync(guildID, ct);

        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var integrations = getResult.Entity;
        await _cacheService.CacheAsync(collectionKey, integrations, ct);

        foreach (var integration in integrations)
        {
            var key = KeyHelpers.CreateGuildIntegrationCacheKey(guildID, integration.ID);
            await _cacheService.CacheAsync(key, integration, ct);
        }

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildWidgetSettings>> GetGuildWidgetSettingsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildWidgetSettingsCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IGuildWidgetSettings>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildWidgetSettingsAsync(guildID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var widget = getResult.Entity;
        await _cacheService.CacheAsync(key, widget, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IGuildWidgetSettings>> ModifyGuildWidgetAsync
    (
        Snowflake guildID,
        Optional<bool> isEnabled = default,
        Optional<Snowflake?> channelID = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyResult = await _actual.ModifyGuildWidgetAsync(guildID, isEnabled, channelID, reason, ct);
        if (!modifyResult.IsSuccess)
        {
            return modifyResult;
        }

        var key = KeyHelpers.CreateGuildWidgetSettingsCacheKey(guildID);
        var widget = modifyResult.Entity;
        await _cacheService.CacheAsync(key, widget, ct);

        return modifyResult;
    }

    /// <inheritdoc />
    public async Task<Result<IWelcomeScreen>> GetGuildWelcomeScreenAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildWelcomeScreenCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IWelcomeScreen>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getResult = await _actual.GetGuildWelcomeScreenAsync(guildID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var welcomeScreen = getResult.Entity;
        await _cacheService.CacheAsync(key, welcomeScreen, ct);

        return getResult;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IGuildMember>>> SearchGuildMembersAsync
    (
        Snowflake guildID,
        string query,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        var result = await _actual.SearchGuildMembersAsync(guildID, query, limit, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        foreach (var guildMember in result.Entity)
        {
            if (!guildMember.User.IsDefined(out var user))
            {
                continue;
            }

            var key = KeyHelpers.CreateGuildMemberKey(guildID, user.ID);
            await _cacheService.CacheAsync(key, guildMember, ct);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteGuildIntegrationAsync
    (
        Snowflake guildID,
        Snowflake integrationID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await _actual.DeleteGuildIntegrationAsync(guildID, integrationID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateGuildIntegrationCacheKey(guildID, integrationID);
        await _cacheService.EvictAsync<IIntegration>(key, ct);

        var collectionKey = KeyHelpers.CreateGuildIntegrationsCacheKey(guildID);
        await _cacheService.EvictAsync<IReadOnlyList<IIntegration>>(collectionKey, ct);

        return deleteResult;
    }
}
