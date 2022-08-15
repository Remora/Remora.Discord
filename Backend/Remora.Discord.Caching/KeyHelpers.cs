//
//  KeyHelpers.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Caching;

/// <summary>
/// Contains methods that create cache keys for various identified entities.
/// </summary>
[PublicAPI]
public static class KeyHelpers
{
    /// <summary>
    /// Creates a cache key for an <see cref="IPermissionOverwrite"/> instance.
    /// </summary>
    /// <param name="channelID">The ID of the channel the overwrite is for.</param>
    /// <param name="overwriteID">The ID of the overwrite.</param>
    /// <returns>The cache key.</returns>
    public static string CreateChannelPermissionCacheKey(in Snowflake channelID, in Snowflake overwriteID)
    {
        return $"{CreateChannelCacheKey(channelID)}:Overwrite:{overwriteID}";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IInvite"/> instance.
    /// </summary>
    /// <param name="code">The invite code.</param>
    /// <returns>The cache key.</returns>
    public static string CreateInviteCacheKey(string code)
    {
        return $"Invite:{code}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IInvite"/> instances from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildInvitesCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Invites";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IChannel"/> instance.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <returns>The cache key.</returns>
    public static string CreateChannelCacheKey(in Snowflake channelID)
    {
        return $"Channel:{channelID}";
    }

    /// <summary>
    /// Creates a cache key for a list of pinned <see cref="IMessage"/> instances.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <returns>The cache key.</returns>
    public static string CreatePinnedMessagesCacheKey(in Snowflake channelID)
    {
        return $"{CreateChannelCacheKey(channelID)}:Messages:Pinned";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="channelID">The ID of the channel the message is in.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <returns>The cache key.</returns>
    public static string CreateMessageCacheKey(in Snowflake channelID, in Snowflake messageID)
    {
        return $"{CreateChannelCacheKey(channelID)}:Message:{messageID}";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IEmoji"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild the emoji is in.</param>
    /// <param name="emojiID">The ID of the emoji.</param>
    /// <returns>The cache key.</returns>
    public static string CreateEmojiCacheKey(in Snowflake guildID, in Snowflake emojiID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Emoji:{emojiID}";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuild"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildCacheKey(in Snowflake guildID)
    {
        return $"Guild:{guildID}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IChannel"/> instances from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildChannelsCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Channels";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuildPreview"/> instance.
    /// </summary>
    /// <param name="guildPreviewID">The ID of the guild preview.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildPreviewCacheKey(in Snowflake guildPreviewID)
    {
        return $"GuildPreview:{guildPreviewID}";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuildMember"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild the member is in.</param>
    /// <param name="userID">The ID of the member.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildMemberKey(in Snowflake guildID, in Snowflake userID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Member:{userID}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IGuildMember"/> instances from a guild, constrained by
    /// the given input parameters. The parameters are used as components for the cache key.
    /// </summary>
    /// <param name="guildID">The ID of the guild the members are in.</param>
    /// <param name="limit">The limit parameter.</param>
    /// <param name="after">The after parameter.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildMembersKey
    (
        in Snowflake guildID,
        in Optional<int> limit,
        in Optional<Snowflake> after
    )
    {
        var limitKey = limit.HasValue ? $":Limit:{limit.Value}" : string.Empty;
        var afterKey = after.HasValue ? $":After:{after.Value}" : string.Empty;

        return $"{CreateGuildCacheKey(guildID)}:Members{limitKey}{afterKey}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IBan"/> instances from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildBansCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Bans";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IBan"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild the ban is in.</param>
    /// <param name="userID">The ID of the banned user.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildBanCacheKey(in Snowflake guildID, in Snowflake userID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Ban:{userID}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IRole"/> instances from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildRolesCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Roles";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IRole"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild the role is in.</param>
    /// <param name="roleID">The ID of the role.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildRoleCacheKey(in Snowflake guildID, in Snowflake roleID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Role:{roleID}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IVoiceRegion"/> instances from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildVoiceRegionsCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:VoiceRegions";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IVoiceRegion"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guildID the voice region is for.</param>
    /// <param name="voiceRegionID">The voice region ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildVoiceRegionCacheKey(in Snowflake guildID, string voiceRegionID)
    {
        return $"{CreateGuildCacheKey(guildID)}:VoiceRegion:{voiceRegionID}";
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IIntegration"/> instances from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildIntegrationsCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Integrations";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IIntegration"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild the integration is in.</param>
    /// <param name="integrationID">The ID of the integration.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildIntegrationCacheKey(in Snowflake guildID, in Snowflake integrationID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Integration:{integrationID}";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuildWidgetSettings"/> instance.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildWidgetSettingsCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:WidgetSettings";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IUser"/> instance.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>The cache key.</returns>
    public static string CreateUserCacheKey(in Snowflake userID)
    {
        return $"User:{userID}";
    }

    /// <summary>
    /// Creates a cache key for the current <see cref="IUser"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public static string CreateCurrentUserCacheKey()
    {
        return "User:@me";
    }

    /// <summary>
    /// Creates a cache key for the <see cref="IConnection"/> objects of the current <see cref="IUser"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public static string CreateCurrentUserConnectionsCacheKey()
    {
        return $"{CreateCurrentUserCacheKey()}:Connections";
    }

    /// <summary>
    /// Creates a cache key for the <see cref="IChannel"/> DM objects of the current <see cref="IUser"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public static string CreateCurrentUserDMsCacheKey()
    {
        return $"{CreateCurrentUserCacheKey()}:Channels";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IConnection"/> instance.
    /// </summary>
    /// <param name="connectionID">The ID of the connection.</param>
    /// <returns>The cache key.</returns>
    public static string CreateConnectionCacheKey(string connectionID)
    {
        return $"Connection:{connectionID}";
    }

    /// <summary>
    /// Creates a cache key for the current <see cref="IApplication"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public static string CreateCurrentApplicationCacheKey()
    {
        return "Application:@me";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="ITemplate"/> instance.
    /// </summary>
    /// <param name="templateCode">The template code.</param>
    /// <returns>The cache key.</returns>
    public static string CreateTemplateCacheKey(string templateCode)
    {
        return $"Template:{templateCode}";
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="ITemplate"/> instances belonging to a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild..</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildTemplatesCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Templates";
    }

    /// <summary>
    /// Creates a cache key for a set of available <see cref="IVoiceRegion"/> instances.
    /// </summary>
    /// <returns>The cache key.</returns>
    public static string CreateVoiceRegionsCacheKey()
    {
        return "VoiceRegions";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IVoiceRegion"/> instance.
    /// </summary>
    /// <param name="voiceRegionID">The voice region ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateVoiceRegionCacheKey(string voiceRegionID)
    {
        return $"VoiceRegion:{voiceRegionID}";
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IWebhook"/> instance.
    /// </summary>
    /// <param name="webhookID">The webhook ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateWebhookCacheKey(in Snowflake webhookID)
    {
        return $"Webhook:{webhookID}";
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="IWebhook"/> instances belonging to a channel.
    /// </summary>
    /// <param name="channelID">The channel ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateChannelWebhooksCacheKey(in Snowflake channelID)
    {
        return $"{CreateChannelCacheKey(channelID)}:Webhooks";
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="IWebhook"/> instances belonging to a guild.
    /// </summary>
    /// <param name="guildID">The guild ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildWebhooksCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Webhooks";
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IPresence"/> instance.
    /// </summary>
    /// <param name="guildID">The guild ID.</param>
    /// <param name="userID">The user ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreatePresenceCacheKey(in Snowflake guildID, in Snowflake userID)
    {
        return $"{CreateGuildMemberKey(guildID, userID)}:Presence";
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IWelcomeScreen"/> instance.
    /// </summary>
    /// <param name="guildID">The guild ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildWelcomeScreenCacheKey(in Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:WelcomeScreen";
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IThreadMember"/> instance.
    /// </summary>
    /// <param name="threadID">The ID of the thread.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>The cache key.</returns>
    public static string CreateThreadMemberCacheKey(in Snowflake threadID, in Snowflake userID)
    {
        return $"Thread:{threadID}:Member:{userID}";
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IAuthorizationInformation"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public static string CreateCurrentAuthorizationInformationCacheKey()
    {
        return "CurrentAuthorizationInformation";
    }

    /// <summary>
    /// Creates a cache key for an original interaction <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="token">The interaction token.</param>
    /// <returns>The cache key.</returns>
    public static string CreateOriginalInteractionMessageCacheKey(string token)
    {
        return $"Interaction:{token}:Original";
    }

    /// <summary>
    /// Creates a cache key for an interaction followup <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="token">The interaction token.</param>
    /// <param name="messageID">The message ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateFollowupMessageCacheKey(string token, in Snowflake messageID)
    {
        return CreateWebhookMessageCacheKey(token, messageID);
    }

    /// <summary>
    /// Creates a cache key for a webhook <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="token">The webhook token.</param>
    /// <param name="messageID">The message ID.</param>
    /// <returns>The cache key.</returns>
    public static string CreateWebhookMessageCacheKey(string token, in Snowflake messageID)
    {
        return $"Webhook:{token}:Message:{messageID}";
    }

    /// <summary>
    /// Creates a cache key for a set of channel-scoped <see cref="IInvite"/> instances.
    /// </summary>
    /// <param name="channelID">The ID of the queried channel.</param>
    /// <returns>The cache key.</returns>
    public static string CreateChannelInvitesCacheKey(in Snowflake channelID)
    {
        return $"{CreateChannelCacheKey(channelID)}:Invites";
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="IThreadMember"/> instances.
    /// </summary>
    /// <param name="channelID">The ID of the queried channel.</param>
    /// <returns>The cache key.</returns>
    public static string CreateThreadMembersCacheKey(Snowflake channelID)
    {
        return $"Thread:{channelID}:Members";
    }

    /// <summary>
    /// Creates a cache key for a set of guild-scoped <see cref="IEmoji"/> instances.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public static string CreateGuildEmojisCacheKey(Snowflake guildID)
    {
        return $"{CreateGuildCacheKey(guildID)}:Emojis";
    }

    /// <summary>
    /// Creates a cache key for an evicted entity, identified by the given key.
    /// </summary>
    /// <param name="key">The original key.</param>
    /// <returns>The eviction key.</returns>
    public static string CreateEvictionCacheKey(string key)
    {
        return $"Evicted:{key}";
    }
}
