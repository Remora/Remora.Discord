//
//  CachingDiscordRestGuildAPI.cs
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
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API
{
    /// <summary>
    /// Implements a caching version of the channel API.
    /// </summary>
    [PublicAPI]
    public class CachingDiscordRestGuildAPI : DiscordRestGuildAPI
    {
        private readonly CacheService _cacheService;

        /// <inheritdoc cref="DiscordRestGuildAPI" />
        public CachingDiscordRestGuildAPI
        (
            IRestHttpClient restHttpClient,
            JsonSerializerOptions jsonOptions,
            CacheService cacheService
        )
            : base(restHttpClient, jsonOptions)
        {
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuild>> CreateGuildAsync
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
            var createResult = await base.CreateGuildAsync
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
            _cacheService.Cache(key, guild);

            return createResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuild>> GetGuildAsync
        (
            Snowflake guildID,
            Optional<bool> withCounts = default,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildCacheKey(guildID);
            if (_cacheService.TryGetValue<IGuild>(key, out var cachedInstance))
            {
                return Result<IGuild>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildAsync(guildID, withCounts, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var guild = getResult.Entity;
            _cacheService.Cache(key, guild);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuildPreview>> GetGuildPreviewAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildPreviewCacheKey(guildID);
            if (_cacheService.TryGetValue<IGuildPreview>(key, out var cachedInstance))
            {
                return Result<IGuildPreview>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildPreviewAsync(guildID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var guildPreview = getResult.Entity;
            _cacheService.Cache(key, guildPreview);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuild>> ModifyGuildAsync
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
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var modifyResult = await base.ModifyGuildAsync
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
                reason,
                ct
            );

            if (!modifyResult.IsSuccess)
            {
                return modifyResult;
            }

            var guild = modifyResult.Entity;
            var key = KeyHelpers.CreateGuildCacheKey(guild.ID);
            _cacheService.Cache(key, guild);

            return modifyResult;
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteGuildAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.DeleteGuildAsync(guildID, ct);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateGuildCacheKey(guildID);
            _cacheService.Evict(key);

            return deleteResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IChannel>>> GetGuildChannelsAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildChannelsCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IChannel>>(key, out var cachedInstance))
            {
                return Result<IReadOnlyList<IChannel>>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildChannelsAsync(guildID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var channels = getResult.Entity;
            _cacheService.Cache(key, channels);

            foreach (var channel in channels)
            {
                var channelKey = KeyHelpers.CreateChannelCacheKey(channel.ID);
                _cacheService.Cache(channelKey, channel);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IChannel>> CreateGuildChannelAsync
        (
            Snowflake guildID,
            string name,
            Optional<ChannelType> type = default,
            Optional<string> topic = default,
            Optional<int> bitrate = default,
            Optional<int> userLimit = default,
            Optional<int> rateLimitPerUser = default,
            Optional<int> position = default,
            Optional<IReadOnlyList<IPermissionOverwrite>> permissionOverwrites = default,
            Optional<Snowflake> parentID = default,
            Optional<bool> isNsfw = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var createResult = await base.CreateGuildChannelAsync
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
                reason,
                ct
            );

            if (!createResult.IsSuccess)
            {
                return createResult;
            }

            var guild = createResult.Entity;
            var key = KeyHelpers.CreateGuildCacheKey(guild.ID);
            _cacheService.Cache(key, guild);

            return createResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuildMember>> GetGuildMemberAsync
        (
            Snowflake guildID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildMemberKey(guildID, userID);
            if (_cacheService.TryGetValue<IGuildMember>(key, out var cachedInstance))
            {
                return Result<IGuildMember>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildMemberAsync(guildID, userID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var guildMember = getResult.Entity;
            _cacheService.Cache(key, guildMember);

            if (!guildMember.User.IsDefined(out var user))
            {
                return getResult;
            }

            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
            _cacheService.Cache(userKey, user);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IGuildMember>>> ListGuildMembersAsync
        (
            Snowflake guildID,
            Optional<int> limit = default,
            Optional<Snowflake> after = default,
            CancellationToken ct = default
        )
        {
            var collectionKey = KeyHelpers.CreateGuildMembersKey(guildID, limit, after);
            if (_cacheService.TryGetValue<IReadOnlyList<IGuildMember>>(collectionKey, out var cachedInstance))
            {
                return Result<IReadOnlyList<IGuildMember>>.FromSuccess(cachedInstance);
            }

            var getResult = await base.ListGuildMembersAsync(guildID, limit, after, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var members = getResult.Entity;
            _cacheService.Cache(collectionKey, members);

            foreach (var member in members)
            {
                if (!member.User.IsDefined(out var user))
                {
                    continue;
                }

                var key = KeyHelpers.CreateGuildMemberKey(guildID, user.ID);
                _cacheService.Cache(key, member);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuildMember?>> AddGuildMemberAsync
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
            var getResult = await base.AddGuildMemberAsync
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
            _cacheService.Cache(key, member);
            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuildMember>> ModifyCurrentMemberAsync
        (
            Snowflake guildID,
            Optional<string?> nickname = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var getResult = await base.ModifyCurrentMemberAsync(guildID, nickname, reason, ct);
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
            _cacheService.Cache(key, guildMember);

            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
            _cacheService.Cache(userKey, user);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result> RemoveGuildMemberAsync
        (
            Snowflake guildID,
            Snowflake userID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var removeMember = await base.RemoveGuildMemberAsync(guildID, userID, reason, ct);
            if (!removeMember.IsSuccess)
            {
                return removeMember;
            }

            var memberKey = KeyHelpers.CreateGuildMemberKey(guildID, userID);
            _cacheService.Evict(memberKey);

            return removeMember;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IBan>>> GetGuildBansAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var collectionKey = KeyHelpers.CreateGuildBansCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IBan>>(collectionKey, out var cachedInstance))
            {
                return Result<IReadOnlyList<IBan>>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildBansAsync(guildID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var bans = getResult.Entity;
            _cacheService.Cache(collectionKey, bans);

            foreach (var ban in bans)
            {
                var key = KeyHelpers.CreateGuildBanCacheKey(guildID, ban.User.ID);
                _cacheService.Cache(key, ban);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IBan>> GetGuildBanAsync
        (
            Snowflake guildID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildBanCacheKey(guildID, userID);
            if (_cacheService.TryGetValue<IBan>(key, out var cachedInstance))
            {
                return Result<IBan>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildBanAsync(guildID, userID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var ban = getResult.Entity;
            _cacheService.Cache(key, ban);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result> RemoveGuildBanAsync
        (
            Snowflake guildID,
            Snowflake userID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.RemoveGuildBanAsync(guildID, userID, reason, ct);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateGuildBanCacheKey(guildID, userID);
            _cacheService.Evict(key);

            return deleteResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IRole>>> GetGuildRolesAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var collectionKey = KeyHelpers.CreateGuildRolesCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IRole>>(collectionKey, out var cachedInstance))
            {
                return Result<IReadOnlyList<IRole>>.FromSuccess(cachedInstance);
            }

            var getRoles = await base.GetGuildRolesAsync(guildID, ct);
            if (!getRoles.IsSuccess)
            {
                return getRoles;
            }

            var roles = getRoles.Entity;
            _cacheService.Cache(collectionKey, roles);

            foreach (var role in roles)
            {
                var key = KeyHelpers.CreateGuildRoleCacheKey(guildID, role.ID);
                _cacheService.Cache(key, role);
            }

            return getRoles;
        }

        /// <inheritdoc />
        public override async Task<Result<IRole>> CreateGuildRoleAsync
        (
            Snowflake guildID,
            Optional<string> name = default,
            Optional<IDiscordPermissionSet> permissions = default,
            Optional<Color> colour = default,
            Optional<bool> isHoisted = default,
            Optional<Stream> icon = default,
            Optional<string> unicodeEmoji = default,
            Optional<bool> isMentionable = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var createResult = await base.CreateGuildRoleAsync
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
            _cacheService.Cache(key, role);

            return createResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IRole>>> ModifyGuildRolePositionsAsync
        (
            Snowflake guildID,
            IReadOnlyList<(Snowflake RoleID, Optional<int?> Position)> modifiedPositions,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var modifyResult = await base.ModifyGuildRolePositionsAsync(guildID, modifiedPositions, reason, ct);

            if (!modifyResult.IsSuccess)
            {
                return modifyResult;
            }

            var roles = modifyResult.Entity;
            var collectionKey = KeyHelpers.CreateGuildRolesCacheKey(guildID);
            _cacheService.Cache(collectionKey, roles);

            foreach (var role in roles)
            {
                var key = KeyHelpers.CreateGuildRoleCacheKey(guildID, role.ID);
                _cacheService.Cache(key, role);
            }

            return modifyResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IRole>> ModifyGuildRoleAsync
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
            var modifyResult = await base.ModifyGuildRoleAsync
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
            _cacheService.Cache(key, role);

            return modifyResult;
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteGuildRoleAsync
        (
            Snowflake guildId,
            Snowflake roleID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.DeleteGuildRoleAsync(guildId, roleID, reason, ct);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateGuildRoleCacheKey(guildId, roleID);
            _cacheService.Evict(key);

            return deleteResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IVoiceRegion>>> GetGuildVoiceRegionsAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var collectionKey = KeyHelpers.CreateGuildVoiceRegionsCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IVoiceRegion>>(collectionKey, out var cachedInstance))
            {
                return Result<IReadOnlyList<IVoiceRegion>>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildVoiceRegionsAsync(guildID, ct);

            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var voiceRegions = getResult.Entity;
            _cacheService.Cache(collectionKey, voiceRegions);

            foreach (var voiceRegion in voiceRegions)
            {
                var key = KeyHelpers.CreateGuildVoiceRegionCacheKey(guildID, voiceRegion.ID);
                _cacheService.Cache(key, voiceRegion);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IInvite>>> GetGuildInvitesAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var collectionKey = KeyHelpers.CreateGuildInvitesCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IInvite>>(collectionKey, out var cachedInstance))
            {
                return Result<IReadOnlyList<IInvite>>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildInvitesAsync(guildID, ct);

            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var invites = getResult.Entity;
            _cacheService.Cache(collectionKey, invites);

            foreach (var invite in invites)
            {
                var key = KeyHelpers.CreateInviteCacheKey(invite.Code);
                _cacheService.Cache(key, invite);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IIntegration>>> GetGuildIntegrationsAsync
        (
            Snowflake guildID,
            Optional<bool> includeApplications = default,
            CancellationToken ct = default
        )
        {
            var collectionKey = KeyHelpers.CreateGuildIntegrationsCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IIntegration>>(collectionKey, out var cachedInstance))
            {
                return Result<IReadOnlyList<IIntegration>>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildIntegrationsAsync(guildID, includeApplications, ct);

            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var integrations = getResult.Entity;
            _cacheService.Cache(collectionKey, integrations);

            foreach (var integration in integrations)
            {
                var key = KeyHelpers.CreateGuildIntegrationCacheKey(guildID, integration.ID);
                _cacheService.Cache(key, integration);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuildWidget>> GetGuildWidgetSettingsAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildWidgetSettingsCacheKey(guildID);
            if (_cacheService.TryGetValue<IGuildWidget>(key, out var cachedInstance))
            {
                return Result<IGuildWidget>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildWidgetSettingsAsync(guildID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var widget = getResult.Entity;
            _cacheService.Cache(key, widget);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuildWidget>> ModifyGuildWidgetAsync
        (
            Snowflake guildID,
            Optional<bool> isEnabled = default,
            Optional<Snowflake?> channelID = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            var modifyResult = await base.ModifyGuildWidgetAsync(guildID, isEnabled, channelID, reason, ct);
            if (!modifyResult.IsSuccess)
            {
                return modifyResult;
            }

            var key = KeyHelpers.CreateGuildWidgetSettingsCacheKey(guildID);
            var widget = modifyResult.Entity;
            _cacheService.Cache(key, widget);

            return modifyResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IWelcomeScreen>> GetGuildWelcomeScreenAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildWelcomeScreenCacheKey(guildID);
            if (_cacheService.TryGetValue<IWelcomeScreen>(key, out var cachedInstance))
            {
                return Result<IWelcomeScreen>.FromSuccess(cachedInstance);
            }

            var getResult = await base.GetGuildWelcomeScreenAsync(guildID, ct);
            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var welcomeScreen = getResult.Entity;
            _cacheService.Cache(key, welcomeScreen);

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IGuildMember>>> SearchGuildMembersAsync
        (
            Snowflake guildID,
            string query,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            var result = await base.SearchGuildMembersAsync(guildID, query, limit, ct);
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
                _cacheService.Cache(key, guildMember);
            }

            return result;
        }
    }
}
