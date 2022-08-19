//
//  IDiscordRestGuildAPI.cs
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
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Guild API.
/// </summary>
[PublicAPI]
public interface IDiscordRestGuildAPI
{
    /// <summary>
    /// Creates a new guild. This can only by used by bots in less than 10 guilds.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="name">The name of the guild (2-100 characters).</param>
    /// <param name="icon">The icon.</param>
    /// <param name="verificationLevel">The verification level.</param>
    /// <param name="defaultMessageNotifications">The default message notification level.</param>
    /// <param name="explicitContentFilter">The explicit content filter level.</param>
    /// <param name="roles">The new guild roles.</param>
    /// <param name="channels">The new guild channels.</param>
    /// <param name="afkChannelID">The ID of the AFK channel.</param>
    /// <param name="afkTimeout">The number of seconds until AFK timeout.</param>
    /// <param name="systemChannelID">The ID of the system message channel.</param>
    /// <param name="systemChannelFlags">The channel flags to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IGuild>> CreateGuildAsync
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
    );

    /// <summary>
    /// Gets the guild with the given ID.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="withCounts">Whether member and presence counts should be included.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuild>> GetGuildAsync
    (
        Snowflake guildID,
        Optional<bool> withCounts = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a preview of the guild with the given ID.
    /// <remarks>
    /// This is only for public guilds.
    /// </remarks>
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildPreview>> GetGuildPreviewAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the guild with the given ID.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The new name of the guild.</param>
    /// <param name="verificationLevel">The new verification level of the guild.</param>
    /// <param name="defaultMessageNotifications">The new default notification level of the guild.</param>
    /// <param name="explicitContentFilter">The new explicit filter level of the guild.</param>
    /// <param name="afkChannelID">The ID of the new AFK channel.</param>
    /// <param name="afkTimeout">The new AFK timeout.</param>
    /// <param name="icon">The new icon.</param>
    /// <param name="ownerID">The ID of the new owner.</param>
    /// <param name="splash">The new splash.</param>
    /// <param name="discoverySplash">The new discovery splash.</param>
    /// <param name="banner">The new banner.</param>
    /// <param name="systemChannelID">The ID of the new channel for system messages.</param>
    /// <param name="systemChannelFlags">The new system channel flags.</param>
    /// <param name="rulesChannelID">The ID of the new channel for rules.</param>
    /// <param name="publicUpdatesChannelID">The ID of the new channel for public updates.</param>
    /// <param name="preferredLocale">The new preferred locale.</param>
    /// <param name="features">The new guild features.</param>
    /// <param name="description">The new description.</param>
    /// <param name="isPremiumProgressBarEnabled">Whether the guild has the boost progress bar enabled.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IGuild>> ModifyGuildAsync
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
    );

    /// <summary>
    /// Deletes the guild with the given ID.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildAsync(Snowflake guildID, CancellationToken ct = default);

    /// <summary>
    /// Gets the channels of the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IChannel>>> GetGuildChannelsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new channel for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the new channel.</param>
    /// <param name="type">The type of the new channel.</param>
    /// <param name="topic">The topic of the new channel.</param>
    /// <param name="bitrate">The bitrate of the new channel, if it is a voice channel.</param>
    /// <param name="userLimit">The maximum number of users in the channel, if it is a voice channel.</param>
    /// <param name="rateLimitPerUser">The number of seconds a user has to wait between messages.</param>
    /// <param name="position">The sorting position of the new channel.</param>
    /// <param name="permissionOverwrites">The permission overwrites of the new channel.</param>
    /// <param name="parentID">The ID of the parent category of the new channel.</param>
    /// <param name="isNsfw">Whether the new channel is NSFW.</param>
    /// <param name="rtcRegion">The ID of the voice region of the voice or stage channel.</param>
    /// <param name="videoQualityMode">The video quality mode of the voice channel.</param>
    /// <param name="defaultAutoArchiveDuration">The default auto archival duration for threads.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> CreateGuildChannelAsync
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
    );

    /// <summary>
    /// Modifies the positions of a set of channels in the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="positionModifications">The new positions of the modified channels.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> ModifyGuildChannelPositionsAsync
    (
        Snowflake guildID,
        IReadOnlyList
        <
            (
            Snowflake ChannelID,
            int? Position,
            bool? LockPermissions,
            Snowflake? ParentID
            )
        > positionModifications,
        CancellationToken ct = default
    );

    /// <summary>
    /// Lists the active threads in the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IGuildThreadQueryResponse>> ListActiveGuildThreadsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets member information about the given user.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildMember>> GetGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a list of guild members.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="limit">The maximum number of guild members to return (1-1000).</param>
    /// <param name="after">The highest user ID in the previously retrieved page.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IGuildMember>>> ListGuildMembersAsync
    (
        Snowflake guildID,
        Optional<int> limit = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a list of guild members whose username or nickname start with the query string.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="query">The query string.</param>
    /// <param name="limit">The maximum number of members to return (1-1000).</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IGuildMember>>> SearchGuildMembersAsync
    (
        Snowflake guildID,
        string query,
        Optional<int> limit = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Adds a user to the guild using their OAuth2 access token.
    /// </summary>
    /// <remarks>
    /// The returned value will be null if the user is already in the guild.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user to add.</param>
    /// <param name="accessToken">The OAuth2 access token.</param>
    /// <param name="nickname">The user's new nickname.</param>
    /// <param name="roles">The roles the user should have.</param>
    /// <param name="isMuted">Whether the user should be muted in voice channels.</param>
    /// <param name="isDeafened">Whether the user should be deafened in voice channels.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result that may or may not have succeeded.</returns>
    Task<Result<IGuildMember?>> AddGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        string accessToken,
        Optional<string> nickname = default,
        Optional<IReadOnlyList<Snowflake>> roles = default,
        Optional<bool> isMuted = default,
        Optional<bool> isDeafened = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies attributes of a guild member.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="nickname">The new nickname of the user.</param>
    /// <param name="roles">The new roles of the user.</param>
    /// <param name="isMuted">The new mute state of the user.</param>
    /// <param name="isDeafened">The new deaf state of the user.</param>
    /// <param name="channelID">The new voice channel of the user.</param>
    /// <param name="communicationDisabledUntil">The <see cref="DateTime"/> until the user has communication disabled.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A rest result which may or may not have succeeded.</returns>
    Task<Result> ModifyGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string?> nickname = default,
        Optional<IReadOnlyList<Snowflake>?> roles = default,
        Optional<bool?> isMuted = default,
        Optional<bool?> isDeafened = default,
        Optional<Snowflake?> channelID = default,
        Optional<DateTimeOffset?> communicationDisabledUntil = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the current member.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="nickname">The new nickname.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded, containing the updated member.</returns>
    Task<Result<IGuildMember>> ModifyCurrentMemberAsync
    (
        Snowflake guildID,
        Optional<string?> nickname = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Adds a role to a guild member.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> AddGuildMemberRoleAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Removes a role from a guild member.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> RemoveGuildMemberRoleAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Removes a member from the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> RemoveGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a list of bans.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="limit">The maximum number of bans to return (max 1000).</param>
    /// <param name="before">
    /// The ID of the ban to get bans before. This is a mutually exclusive option with <paramref name="after"/>, though
    /// Discord accepts both at the same time.
    /// </param>
    /// <param name="after">
    /// The ID of the ban to get bans after. This is a mutually exclusive option with <paramref name="before"/>, though
    /// Discord accepts both at the same time.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IBan>>> GetGuildBansAsync
    (
        Snowflake guildID,
        Optional<int> limit = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a ban object for the given user.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IBan>> GetGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Bans the given user.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="deleteMessageDays">The number of days to delete messages for (0-7). Defaults to 0.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> CreateGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<int> deleteMessageDays = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Unbans the given user.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> RemoveGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the roles in the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IRole>>> GetGuildRolesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new guild role.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the new role.</param>
    /// <param name="permissions">The permissions of the new role.</param>
    /// <param name="colour">The colour of the new role.</param>
    /// <param name="isHoisted">Whether the new role is displayed separately in the sidebar.</param>
    /// <param name="icon">The role's icon image.</param>
    /// <param name="unicodeEmoji">The role's unicode emoji icon.</param>
    /// <param name="isMentionable">Whether the new role is mentionable.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IRole>> CreateGuildRoleAsync
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
    );

    /// <summary>
    /// Modifies the positions of a set of roles in the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="modifiedPositions">The modified role positions.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IRole>>> ModifyGuildRolePositionsAsync
    (
        Snowflake guildID,
        IReadOnlyList<(Snowflake RoleID, Optional<int?> Position)> modifiedPositions,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given role.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="name">The new name of the role.</param>
    /// <param name="permissions">The new permissions of the role.</param>
    /// <param name="color">The new color of the role.</param>
    /// <param name="isHoisted">Whether the role is displayed separately in the sidebar.</param>
    /// <param name="icon">The role's icon image.</param>
    /// <param name="unicodeEmoji">The role's unicode emoji icon.</param>
    /// <param name="isMentionable">Whether the role is mentionable.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IRole>> ModifyGuildRoleAsync
    (
        Snowflake guildID,
        Snowflake roleID,
        Optional<string?> name = default,
        Optional<IDiscordPermissionSet?> permissions = default,
        Optional<Color?> color = default,
        Optional<bool?> isHoisted = default,
        Optional<Stream?> icon = default,
        Optional<string?> unicodeEmoji = default,
        Optional<bool?> isMentionable = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies a guild's multi-factor authentication level. Requires guild ownership.
    /// </summary>
    /// <param name="guildID">The ID of the guild to modify.</param>
    /// <param name="level">The new MFA level.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<MultiFactorAuthenticationLevel>> ModifyGuildMFALevelAsync
    (
        Snowflake guildID,
        MultiFactorAuthenticationLevel level,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given role.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildRoleAsync
    (
        Snowflake guildID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the number of users that would be pruned in a prune operation.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="days">The days a user needs to have been inactive for them to be pruned.</param>
    /// <param name="includeRoles">The roles that should be included in a prune operation.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IPruneCount>> GetGuildPruneCountAsync
    (
        Snowflake guildID,
        Optional<int> days = default,
        Optional<IReadOnlyList<Snowflake>> includeRoles = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Initiates a prune of the guild members.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="days">The days a user needs to have been inactive for them to be pruned.</param>
    /// <param name="computePruneCount">Whether the number of pruned users should be computed and returned.</param>
    /// <param name="includeRoles">The roles that should be included in a prune operation.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IPruneCount>> BeginGuildPruneAsync
    (
        Snowflake guildID,
        Optional<int> days = default,
        Optional<bool> computePruneCount = default,
        Optional<IReadOnlyList<Snowflake>> includeRoles = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the voice regions for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IVoiceRegion>>> GetGuildVoiceRegionsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the invites for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IInvite>>> GetGuildInvitesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the integrations for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IIntegration>>> GetGuildIntegrationsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the integration with the given ID for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="integrationID">The ID of the integration.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildIntegrationAsync
    (
        Snowflake guildID,
        Snowflake integrationID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the guild widget.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildWidgetSettings>> GetGuildWidgetSettingsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the guild widget for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="isEnabled">Whether the widget is enabled.</param>
    /// <param name="channelID">The ID of the channel invites will be generated for.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IGuildWidgetSettings>> ModifyGuildWidgetAsync
    (
        Snowflake guildID,
        Optional<bool> isEnabled = default,
        Optional<Snowflake?> channelID = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the status and invite widget for the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IGuildWidget>> GetGuildWidgetAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the vanity invite for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IPartialInvite>> GetGuildVanityUrlAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the image for the guild widget.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="style">The image style.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<Stream>> GetGuildWidgetImageAsync
    (
        Snowflake guildID,
        Optional<WidgetImageStyle> style = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the guild's welcome screen.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IWelcomeScreen>> GetGuildWelcomeScreenAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the guild's welcome screen.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="isEnabled">Whether the welcome screen is enabled.</param>
    /// <param name="welcomeChannels">The channels displayed.</param>
    /// <param name="description">The guild's description.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IWelcomeScreen>> ModifyGuildWelcomeScreenAsync
    (
        Snowflake guildID,
        Optional<bool?> isEnabled = default,
        Optional<IReadOnlyList<IWelcomeScreenChannel>?> welcomeChannels = default,
        Optional<string?> description = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the voice state of the current user.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="channelID">The ID of the voice channel the user is currently in.</param>
    /// <param name="suppress">Whether to toggle the user's suppression state.</param>
    /// <param name="requestToSpeakTimestamp">The time when the user requested to speak.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result> ModifyCurrentUserVoiceStateAsync
    (
        Snowflake guildID,
        Snowflake channelID,
        Optional<bool> suppress = default,
        Optional<DateTimeOffset?> requestToSpeakTimestamp = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the voice state of another user.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="channelID">The ID of the voice channel the user is currently in.</param>
    /// <param name="suppress">Whether to toggle the user's suppression state.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IVoiceState>> ModifyUserVoiceStateAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake channelID,
        Optional<bool> suppress = default,
        CancellationToken ct = default
    );
}
