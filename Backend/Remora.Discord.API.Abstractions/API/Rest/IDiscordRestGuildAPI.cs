//
//  IDiscordRestGuildAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Rest
{
    /// <summary>
    /// Represents the Discord Guild API.
    /// </summary>
    [PublicAPI]
    public interface IDiscordRestGuildAPI
    {
        /// <summary>
        /// Creates a new guild. This can only by used by bots in less than 10 guilds.
        /// </summary>
        /// <param name="name">The name of the guild (2-100 characters).</param>
        /// <param name="region">The voice region ID.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="verificationLevel">The verification level.</param>
        /// <param name="defaultMessageNotifications">The default message notification level.</param>
        /// <param name="explicitContentFilter">The explicit content filter level.</param>
        /// <param name="roles">The new guild roles.</param>
        /// <param name="channels">The new guild channels.</param>
        /// <param name="afkChannelID">The ID of the AFK channel.</param>
        /// <param name="afkTimeout">The number of seconds until AFK timeout.</param>
        /// <param name="systemChannelID">The ID of the system message channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IGuild>> CreateGuildAsync
        (
            string name,
            Optional<string> region = default,
            Optional<Stream> icon = default,
            Optional<VerificationLevel> verificationLevel = default,
            Optional<MessageNotificationLevel> defaultMessageNotifications = default,
            Optional<ExplicitContentFilterLevel> explicitContentFilter = default,
            Optional<IReadOnlyList<IRole>> roles = default,
            Optional<IReadOnlyList<IPartialChannel>> channels = default,
            Optional<Snowflake> afkChannelID = default,
            Optional<TimeSpan> afkTimeout = default,
            Optional<Snowflake> systemChannelID = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the guild with the given ID.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="withCounts">Whether member and presence counts should be included.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IGuild>> GetGuildAsync
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
        Task<IRetrieveRestEntityResult<IGuildPreview>> GetGuildPreviewAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the guild with the given ID.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="name">The new name of the guild.</param>
        /// <param name="region">The new voice region of the guild.</param>
        /// <param name="verificationLevel">The new verification level of the guild.</param>
        /// <param name="defaultMessageNotifications">The new default notification level of the guild.</param>
        /// <param name="explicitContentFilter">The new explicit filter level of the guild.</param>
        /// <param name="afkChannelID">The ID of the new AFK channel.</param>
        /// <param name="afkTimeout">The new AFK timeout.</param>
        /// <param name="icon">The new icon.</param>
        /// <param name="ownerID">The ID of the new owner.</param>
        /// <param name="splash">The new splash.</param>
        /// <param name="banner">The new banner.</param>
        /// <param name="systemChannelID">The ID of the new channel for system messages.</param>
        /// <param name="rulesChannelID">The ID of the new channel for rules.</param>
        /// <param name="publicUpdatesChannelID">The ID of the new channel for public updates.</param>
        /// <param name="preferredLocale">The new preferred locale.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IGuild>> ModifyGuildAsync
        (
            Snowflake guildID,
            Optional<string> name = default,
            Optional<string?> region = default,
            Optional<VerificationLevel?> verificationLevel = default,
            Optional<MessageNotificationLevel?> defaultMessageNotifications = default,
            Optional<ExplicitContentFilterLevel?> explicitContentFilter = default,
            Optional<Snowflake?> afkChannelID = default,
            Optional<TimeSpan> afkTimeout = default,
            Optional<Stream?> icon = default,
            Optional<Snowflake> ownerID = default,
            Optional<Stream?> splash = default,
            Optional<Stream?> banner = default,
            Optional<Snowflake?> systemChannelID = default,
            Optional<Snowflake?> rulesChannelID = default,
            Optional<Snowflake?> publicUpdatesChannelID = default,
            Optional<string?> preferredLocale = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes the guild with the given ID.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteGuildAsync(Snowflake guildID, CancellationToken ct = default);

        /// <summary>
        /// Gets the channels of the given guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IChannel>>> GetGuildChannelsAsync
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IChannel>> CreateGuildChannelAsync
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
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the positions of a set of channels in the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="positionModifications">The new positions of the modified channels.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> ModifyGuildChannelPositionsAsync
        (
            Snowflake guildID,
            IReadOnlyList<(Snowflake ChannelID, int? Position)> positionModifications,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets member information about the given user.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IGuildMember>> GetGuildMemberAsync
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
        Task<IRetrieveRestEntityResult<IReadOnlyList<IGuildMember>>> ListGuildMembersAsync
        (
            Snowflake guildID,
            Optional<int> limit = default,
            Optional<Snowflake> after = default,
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
        Task<ICreateRestEntityResult<IGuildMember?>> AddGuildMemberAsync
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A rest result which may or may not have succeeded.</returns>
        Task<IRestResult> ModifyGuildMemberAsync
        (
            Snowflake guildID,
            Snowflake userID,
            Optional<string?> nickname = default,
            Optional<IReadOnlyList<Snowflake>?> roles = default,
            Optional<bool?> isMuted = default,
            Optional<bool?> isDeafened = default,
            Optional<Snowflake?> channelID = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the nickname of the current user.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="nickname">The new nickname.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<string>> ModifyCurrentUserNickAsync
        (
            Snowflake guildID,
            Optional<string?> nickname = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Adds a role to a guild member.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="roleID">The ID of the role.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> AddGuildMemberRoleAsync
        (
            Snowflake guildID,
            Snowflake userID,
            Snowflake roleID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Removes a role from a guild member.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="roleID">The ID of the role.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> RemoveGuildMemberRoleAsync
        (
            Snowflake guildID,
            Snowflake userID,
            Snowflake roleID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Removes a member from the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> RemoveGuildMemberAsync
        (
            Snowflake guildID,
            Snowflake userID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets a list of bans.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IBan>>> GetGuildBansAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets a ban object for the given user.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IBan>> GetGuildBanAsync
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
        /// <param name="deleteMessageDays">The number of days to delete messages for (0-7).</param>
        /// <param name="reason">The reason for the ban.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> CreateGuildBanAsync
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> RemoveGuildBanAsync
        (
            Snowflake guildID,
            Snowflake userID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the roles in the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IRole>>> GetGuildRolesAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Creates a new guild role.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="name">The name of the new role.</param>
        /// <param name="permissions">The permissions of the new role.</param>
        /// <param name="colour">The colour of the new role.</param>
        /// <param name="isHoisted">Whether the new role is displayed separately in the sidebar.</param>
        /// <param name="isMentionable">Whether the new role is mentionable.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IRole>> CreateGuildRoleAsync
        (
            Snowflake guildID,
            Optional<string> name = default,
            Optional<IDiscordPermissionSet> permissions = default,
            Optional<Color> colour = default,
            Optional<bool> isHoisted = default,
            Optional<bool> isMentionable = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the positions of a set of roles in the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="modifiedPositions">The modified role positions.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IReadOnlyList<IRole>>> ModifyGuildRolePositionsAsync
        (
            Snowflake guildID,
            IReadOnlyList<(Snowflake RoleID, Optional<int?> Position)> modifiedPositions,
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the given role.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="roleID">The ID of the role.</param>
        /// <param name="name">The new name of the role.</param>
        /// <param name="permissions">The new permissions of the role.</param>
        /// <param name="color">The new color of the role.</param>
        /// <param name="isHoisted">Whether the role is displayed separately in the sidebar.</param>
        /// <param name="isMentionable">Whether the role is mentionable.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IRole>> ModifyGuildRoleAsync
        (
            Snowflake guildID,
            Snowflake roleID,
            Optional<string?> name = default,
            Optional<IDiscordPermissionSet?> permissions = default,
            Optional<Color?> color = default,
            Optional<bool?> isHoisted = default,
            Optional<bool?> isMentionable = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes the given role.
        /// </summary>
        /// <param name="guildId">The ID of the guild.</param>
        /// <param name="roleID">The ID of the role.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteGuildRoleAsync
        (
            Snowflake guildId,
            Snowflake roleID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the number of users that would br pruned in a prune operation.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="days">The days a user needs to have been inactive for them to be pruned.</param>
        /// <param name="includeRoles">The roles that should be included in a prune operation.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IPruneCount>> GetGuildPruneCountAsync
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IPruneCount>> BeginGuildPruneAsync
        (
            Snowflake guildID,
            Optional<int> days = default,
            Optional<bool> computePruneCount = default,
            Optional<IReadOnlyList<Snowflake>> includeRoles = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the voice regions for the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IVoiceRegion>>> GetGuildVoiceRegionsAsync
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
        Task<IRetrieveRestEntityResult<IReadOnlyList<IInvite>>> GetGuildInvitesAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the integrations for the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="includeApplications">Whether bot and OAuth2 webhook integrations should be included.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IIntegration>>> GetGuildIntegrationsAsync
        (
            Snowflake guildID,
            Optional<bool> includeApplications = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Attaches an integration object from the current user to the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="integrationType">The type of integration.</param>
        /// <param name="integrationID">The ID of the integration.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> CreateGuildIntegrationAsync
        (
            Snowflake guildID,
            string integrationType,
            Snowflake integrationID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the behaviour and settings of an integration object for the guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="integrationID">The ID of the integration.</param>
        /// <param name="expireBehaviour">The behaviour when an integration subscription lapses.</param>
        /// <param name="expireGracePeriod">
        /// The number of days where the integration will ignore lapsed subscriptions.
        /// </param>
        /// <param name="enableEmoticons">Whether emoticons should be synced for this integration.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> ModifyGuildIntegrationAsync
        (
            Snowflake guildID,
            Snowflake integrationID,
            Optional<IntegrationExpireBehaviour?> expireBehaviour = default,
            Optional<int?> expireGracePeriod = default,
            Optional<bool?> enableEmoticons = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Detaches the given integration object.
        /// </summary>
        /// <remarks>
        ///  Deletes any associated webhooks, and kicks the associated bot (if there is one).
        /// </remarks>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="integrationID">The ID of the integration.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteGuildIntegrationAsync
        (
            Snowflake guildID,
            Snowflake integrationID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Synchronizes the given integration object.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="integrationID">The ID of the integration.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IRestResult> SyncGuildIntegrationAsync
        (
            Snowflake guildID,
            Snowflake integrationID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the guild widget.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IGuildWidget>> GetGuildWidgetSettingsAsync
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IGuildWidget>> ModifyGuildWidgetAsync
        (
            Snowflake guildID,
            Optional<bool> isEnabled = default,
            Optional<Snowflake?> channelID = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the vanity invite for the given guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IPartialInvite>> GetGuildVanityUrlAsync
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
        Task<IRetrieveRestEntityResult<Stream>> GetGuildWidgetImageAsync
        (
            Snowflake guildID,
            Optional<WidgetImageStyle> style = default,
            CancellationToken ct = default
        );
    }
}
