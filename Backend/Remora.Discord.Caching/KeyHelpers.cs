//
//  KeyHelpers.cs
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

using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.Caching
{
    /// <summary>
    /// Contains methods that create cache keys for various identified entities.
    /// </summary>
    public static class KeyHelpers
    {
        /// <summary>
        /// Creates a cache key for an <see cref="IPermissionOverwrite"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel the overwrite is for.</param>
        /// <param name="overwriteID">The ID of the overwrite.</param>
        /// <returns>The cache key.</returns>
        public static object CreateChannelPermissionCacheKey(in Snowflake channelID, in Snowflake overwriteID)
        {
            return (typeof(IPermissionOverwrite), overwriteID, CreateChannelCacheKey(channelID));
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IInvite"/> instance.
        /// </summary>
        /// <param name="code">The invite code.</param>
        /// <returns>The cache key.</returns>
        public static object CreateInviteCacheKey(string code)
        {
            return (typeof(IInvite), code);
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IInvite"/> instances from a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildInvitesCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IInvite>), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IChannel"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <returns>The cache key.</returns>
        public static object CreateChannelCacheKey(in Snowflake channelID)
        {
            return (typeof(IChannel), channelID);
        }

        /// <summary>
        /// Creates a cache key for a list of pinned <see cref="IMessage"/> instances.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <returns>The cache key.</returns>
        public static object CreatePinnedMessagesCacheKey(in Snowflake channelID)
        {
            return ("Pinned", typeof(IReadOnlyList<IMessage>), CreateChannelCacheKey(channelID));
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IMessage"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <returns>The cache key.</returns>
        public static object CreateMessageCacheKey(in Snowflake channelID, in Snowflake messageID)
        {
            var key = (typeof(IMessage), messageID, CreateChannelCacheKey(channelID));
            return key;
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IEmoji"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild the emoji is in.</param>
        /// <param name="emojiID">The ID of the emoji.</param>
        /// <returns>The cache key.</returns>
        public static object CreateEmojiCacheKey(in Snowflake guildID, in Snowflake emojiID)
        {
            return (typeof(IEmoji), emojiID, CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IGuild"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildCacheKey(in Snowflake guildID)
        {
            return (typeof(IGuild), guildID);
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IChannel"/> instances from a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildChannelsCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IChannel>), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IGuildPreview"/> instance.
        /// </summary>
        /// <param name="guildPreviewID">The ID of the guild preview.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildPreviewCacheKey(in Snowflake guildPreviewID)
        {
            return (typeof(IGuildPreview), guildPreviewID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IGuildMember"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild the member is in.</param>
        /// <param name="userID">The ID of the member.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildMemberKey(in Snowflake guildID, in Snowflake userID)
        {
            return (typeof(IGuildMember), userID, CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IGuildMember"/> instances from a guild, constrained by
        /// the given input parameters. The parameters are used as components for the cache key.
        /// </summary>
        /// <param name="guildID">The ID of the guild the members are in.</param>
        /// <param name="limit">The limit parameter.</param>
        /// <param name="after">The after parameter.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildMembersKey
        (
            in Snowflake guildID,
            in Optional<int> limit,
            in Optional<Snowflake> after
        )
        {
            return (typeof(IReadOnlyList<IGuildMember>), guildID, limit, after);
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IBan"/> instances from a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildBansCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IBan>), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IBan"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild the ban is in.</param>
        /// <param name="userID">The ID of the banned user.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildBanCacheKey(in Snowflake guildID, in Snowflake userID)
        {
            return (typeof(IBan), userID, CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IRole"/> instances from a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildRolesCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IRole>), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IRole"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild the role is in.</param>
        /// <param name="roleID">The ID of the role.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildRoleCacheKey(in Snowflake guildID, in Snowflake roleID)
        {
            return (typeof(IRole), roleID, CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IVoiceRegion"/> instances from a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildVoiceRegionsCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IVoiceRegion>), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IVoiceRegion"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guildID the voice region is for.</param>
        /// <param name="voiceRegionID">The voice region ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildVoiceRegionCacheKey(in Snowflake guildID, string voiceRegionID)
        {
            return (typeof(IVoiceRegion), voiceRegionID, CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for a collection of <see cref="IIntegration"/> instances from a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildIntegrationsCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IIntegration>), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IIntegration"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild the integration is in.</param>
        /// <param name="integrationID">The ID of the integration.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildIntegrationCacheKey(in Snowflake guildID, in Snowflake integrationID)
        {
            return (typeof(IIntegration), integrationID, CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IGuildWidget"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildWidgetSettingsCacheKey(in Snowflake guildID)
        {
            return (typeof(IGuildWidget), guildID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IUser"/> instance.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        /// <returns>The cache key.</returns>
        public static object CreateUserCacheKey(in Snowflake userID)
        {
            return (typeof(IUser), userID);
        }

        /// <summary>
        /// Creates a cache key for the current <see cref="IUser"/> instance.
        /// </summary>
        /// <returns>The cache key.</returns>
        public static object CreateCurrentUserCacheKey()
        {
            return (typeof(IUser), "@me");
        }

        /// <summary>
        /// Creates a cache key for the <see cref="IConnection"/> objects of the current <see cref="IUser"/> instance.
        /// </summary>
        /// <returns>The cache key.</returns>
        public static object CreateCurrentUserConnectionsCacheKey()
        {
            return (typeof(IReadOnlyList<IConnection>), CreateCurrentUserCacheKey());
        }

        /// <summary>
        /// Creates a cache key for the <see cref="IChannel"/> DM objects of the current <see cref="IUser"/> instance.
        /// </summary>
        /// <returns>The cache key.</returns>
        public static object CreateCurrentUserDMsCacheKey()
        {
            return (typeof(IReadOnlyList<IChannel>), CreateCurrentUserCacheKey());
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IConnection"/> instance.
        /// </summary>
        /// <param name="connectionID">The ID of the connection.</param>
        /// <returns>The cache key.</returns>
        public static object CreateConnectionCacheKey(string connectionID)
        {
            return (typeof(IConnection), connectionID);
        }

        /// <summary>
        /// Creates a cache key for the current <see cref="IApplication"/> instance.
        /// </summary>
        /// <returns>The cache key.</returns>
        public static object CreateCurrentApplicationCacheKey()
        {
            return (typeof(IApplication), "@me");
        }

        /// <summary>
        /// Creates a cache key for an <see cref="ITemplate"/> instance.
        /// </summary>
        /// <param name="templateCode">The template code.</param>
        /// <returns>The cache key.</returns>
        public static object CreateTemplateCacheKey(string templateCode)
        {
            return (typeof(ITemplate), templateCode);
        }

        /// <summary>
        /// Creates a cache key for a set of <see cref="ITemplate"/> instances belonging to a guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild..</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildTemplatesCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<ITemplate>), CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for a set of available <see cref="IVoiceRegion"/> instances.
        /// </summary>
        /// <returns>The cache key.</returns>
        public static object CreateVoiceRegionsCacheKey()
        {
            return typeof(IReadOnlyList<IVoiceRegion>);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IVoiceRegion"/> instance.
        /// </summary>
        /// <param name="voiceRegionID">The voice region ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreateVoiceRegionCacheKey(string voiceRegionID)
        {
            return (typeof(IVoiceRegion), voiceRegionID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IWebhook"/> instance.
        /// </summary>
        /// <param name="webhookID">The webhook ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreateWebhookCacheKey(in Snowflake webhookID)
        {
            return (typeof(IWebhook), webhookID);
        }

        /// <summary>
        /// Creates a cache key for a set of <see cref="IWebhook"/> instances belonging to a channel.
        /// </summary>
        /// <param name="channelID">The channel ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreateChannelWebhooksCacheKey(in Snowflake channelID)
        {
            return (typeof(IReadOnlyList<IWebhook>), CreateChannelCacheKey(channelID));
        }

        /// <summary>
        /// Creates a cache key for a set of <see cref="IWebhook"/> instances belonging to a guild.
        /// </summary>
        /// <param name="guildID">The guild ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildWebhooksCacheKey(in Snowflake guildID)
        {
            return (typeof(IReadOnlyList<IWebhook>), CreateGuildCacheKey(guildID));
        }

        /// <summary>
        /// Creates a cache key for a <see cref="IPresence"/> instance.
        /// </summary>
        /// <param name="guildID">The guild ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreatePresenceCacheKey(in Snowflake guildID, in Snowflake userID)
        {
            return (typeof(IPresence), CreateGuildCacheKey(guildID), CreateUserCacheKey(userID));
        }

        /// <summary>
        /// Creates a cache key for a <see cref="IWelcomeScreen"/> instance.
        /// </summary>
        /// <param name="guildID">The guild ID.</param>
        /// <returns>The cache key.</returns>
        public static object CreateGuildWelcomeScreenCacheKey(Snowflake guildID)
        {
            return (typeof(IWelcomeScreen), CreateGuildCacheKey(guildID));
        }
    }
}
