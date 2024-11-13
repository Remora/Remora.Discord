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

using System.Text;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Abstractions;
using Remora.Rest.Core;

namespace Remora.Discord.Caching;

/// <summary>
/// Helpers for Discord-related cache keys.
/// </summary>
[PublicAPI]
public static class KeyHelpers
{
    /// <summary>
    /// Creates a cache key for an <see cref="IPermissionOverwrite"/> instance.
    /// </summary>
    /// <param name="ChannelID">The ID of the channel the overwrite is for.</param>
    /// <param name="OverwriteID">The ID of the overwrite.</param>
    /// <returns>The cache key.</returns>
    public record ChannelPermissionCacheKey(in Snowflake ChannelID, in Snowflake OverwriteID)
        : ChannelCacheKey(ChannelID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Overwrite:{this.OverwriteID}");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IInvite"/> instance.
    /// </summary>
    /// <param name="Code">The invite code.</param>
    /// <returns>The cache key.</returns>
    public record InviteCacheKey(string Code) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Invite:{this.Code}");
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IInvite"/> instances from a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildInvitesCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Invites");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IChannel"/> instance.
    /// </summary>
    /// <param name="ChannelID">The ID of the channel.</param>
    /// <returns>The cache key.</returns>
    public record ChannelCacheKey(in Snowflake ChannelID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Channel:{this.ChannelID}");
    }

    /// <summary>
    /// Creates a cache key for a list of pinned <see cref="IMessage"/> instances.
    /// </summary>
    /// <param name="ChannelID">The ID of the channel.</param>
    /// <returns>The cache key.</returns>
    public record PinnedMessagesCacheKey(in Snowflake ChannelID) : ChannelCacheKey(ChannelID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Messages:Pinned");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="ChannelID">The ID of the channel the message is in.</param>
    /// <param name="MessageID">The ID of the message.</param>
    /// <returns>The cache key.</returns>
    public record MessageCacheKey(in Snowflake ChannelID, in Snowflake MessageID) : ChannelCacheKey(ChannelID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Message:{this.MessageID}");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IEmoji"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild the emoji is in.</param>
    /// <param name="EmojiID">The ID of the emoji.</param>
    /// <returns>The cache key.</returns>
    public record EmojiCacheKey(in Snowflake GuildID, in Snowflake EmojiID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Emoji:{this.EmojiID}");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuild"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildCacheKey(in Snowflake GuildID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Guild:{this.GuildID}");
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IChannel"/> instances from a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildChannelsCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Channels");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuildPreview"/> instance.
    /// </summary>
    /// <param name="GuildPreviewID">The ID of the guild preview.</param>
    /// <returns>The cache key.</returns>
    public record GuildPreviewCacheKey(in Snowflake GuildPreviewID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"GuildPreview:{this.GuildPreviewID}");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuildMember"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild the member is in.</param>
    /// <param name="UserID">The ID of the member.</param>
    /// <returns>The cache key.</returns>
    public record GuildMemberKey(in Snowflake GuildID, in Snowflake UserID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Member:{this.UserID}");
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IGuildMember"/> instances from a guild, constrained by
    /// the given input parameters. The parameters are used as components for the cache key.
    /// </summary>
    /// <param name="GuildID">The ID of the guild the members are in.</param>
    /// <param name="Limit">The limit parameter.</param>
    /// <param name="After">The after parameter.</param>
    /// <returns>The cache key.</returns>
    public record GuildMembersKey
    (
        in Snowflake GuildID,
        in Optional<int> Limit,
        in Optional<Snowflake> After
    ) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
        {
            base.AppendToString(stringBuilder).Append(":Members");

            if (this.Limit.TryGet(out var limit))
            {
                stringBuilder.Append($":Limit:{limit}");
            }

            if (this.After.TryGet(out var after))
            {
                stringBuilder.Append($":After:{after}");
            }

            return stringBuilder;
        }
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IBan"/> instances from a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildBansCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Bans");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IBan"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild the ban is in.</param>
    /// <param name="UserID">The ID of the banned user.</param>
    /// <returns>The cache key.</returns>
    public record GuildBanCacheKey(in Snowflake GuildID, in Snowflake UserID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Ban:{this.UserID}");
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IRole"/> instances from a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildRolesCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Roles");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IRole"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild the role is in.</param>
    /// <param name="RoleID">The ID of the role.</param>
    /// <returns>The cache key.</returns>
    public record GuildRoleCacheKey(in Snowflake GuildID, in Snowflake RoleID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Role:{this.RoleID}");
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IVoiceRegion"/> instances from a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildVoiceRegionsCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":VoiceRegions");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IVoiceRegion"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guildID the voice region is for.</param>
    /// <param name="VoiceRegionID">The voice region ID.</param>
    /// <returns>The cache key.</returns>
    public record GuildVoiceRegionCacheKey(in Snowflake GuildID, string VoiceRegionID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":VoiceRegion:{this.VoiceRegionID}");
    }

    /// <summary>
    /// Creates a cache key for a collection of <see cref="IIntegration"/> instances from a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildIntegrationsCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Integrations");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IIntegration"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild the integration is in.</param>
    /// <param name="IntegrationID">The ID of the integration.</param>
    /// <returns>The cache key.</returns>
    public record GuildIntegrationCacheKey(in Snowflake GuildID, in Snowflake IntegrationID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append($":Integration:{this.IntegrationID}");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IGuildWidgetSettings"/> instance.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildWidgetSettingsCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":WidgetSettings");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IUser"/> instance.
    /// </summary>
    /// <param name="UserID">The ID of the user.</param>
    /// <returns>The cache key.</returns>
    public record UserCacheKey(in Snowflake UserID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"User:{this.UserID}");
    }

    /// <summary>
    /// Creates a cache key for the current <see cref="IUser"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public record CurrentUserCacheKey : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append("User:@me");
    }

    /// <summary>
    /// Creates a cache key for the <see cref="IConnection"/> objects of the current <see cref="IUser"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public record CurrentUserConnectionsCacheKey : CurrentUserCacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Connections");
    }

    /// <summary>
    /// Creates a cache key for the <see cref="IChannel"/> DM objects of the current <see cref="IUser"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public record CurrentUserDMsCacheKey : CurrentUserCacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Channels");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IApplicationRoleConnection" /> instance of the current
    /// <see cref="IUser"/> instance.
    /// </summary>
    /// <param name="ApplicationID">The ID of the application.</param>
    /// <returns>The cache key.</returns>
    public record CurrentUserApplicationRoleConnectionCacheKey(in Snowflake ApplicationID) : CurrentUserCacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder)
                .Append(":Application:")
                .Append(this.ApplicationID)
                .Append(":RoleConnection");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IConnection"/> instance.
    /// </summary>
    /// <param name="ConnectionID">The ID of the connection.</param>
    /// <returns>The cache key.</returns>
    public record ConnectionCacheKey(string ConnectionID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Connection:{this.ConnectionID}");
    }

    /// <summary>
    /// Creates a cache key for the current <see cref="IApplication"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public record CurrentApplicationCacheKey : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append("Application:@me");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="ITemplate"/> instance.
    /// </summary>
    /// <param name="TemplateCode">The template code.</param>
    /// <returns>The cache key.</returns>
    public record TemplateCacheKey(string TemplateCode) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Template:{this.TemplateCode}");
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="ITemplate"/> instances belonging to a guild.
    /// </summary>
    /// <param name="GuildID">The ID of the guild..</param>
    /// <returns>The cache key.</returns>
    public record GuildTemplatesCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Templates");
    }

    /// <summary>
    /// Creates a cache key for a set of available <see cref="IVoiceRegion"/> instances.
    /// </summary>
    /// <returns>The cache key.</returns>
    public record VoiceRegionsCacheKey : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append("VoiceRegions");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IVoiceRegion"/> instance.
    /// </summary>
    /// <param name="VoiceRegionID">The voice region ID.</param>
    /// <returns>The cache key.</returns>
    public record VoiceRegionCacheKey(string VoiceRegionID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"VoiceRegion:{this.VoiceRegionID}");
    }

    /// <summary>
    /// Creates a cache key for an <see cref="IWebhook"/> instance.
    /// </summary>
    /// <param name="WebhookID">The webhook ID.</param>
    /// <returns>The cache key.</returns>
    public record WebhookCacheKey(in Snowflake WebhookID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Webhook:{this.WebhookID}");
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="IWebhook"/> instances belonging to a channel.
    /// </summary>
    /// <param name="ChannelID">The channel ID.</param>
    /// <returns>The cache key.</returns>
    public record ChannelWebhooksCacheKey(in Snowflake ChannelID) : ChannelCacheKey(ChannelID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Webhooks");
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="IWebhook"/> instances belonging to a guild.
    /// </summary>
    /// <param name="GuildID">The guild ID.</param>
    /// <returns>The cache key.</returns>
    public record GuildWebhooksCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Webhooks");
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IPresence"/> instance.
    /// </summary>
    /// <param name="GuildID">The guild ID.</param>
    /// <param name="UserID">The user ID.</param>
    /// <returns>The cache key.</returns>
    public record PresenceCacheKey(in Snowflake GuildID, in Snowflake UserID) : GuildMemberKey(GuildID, UserID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Presence");
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IWelcomeScreen"/> instance.
    /// </summary>
    /// <param name="GuildID">The guild ID.</param>
    /// <returns>The cache key.</returns>
    public record GuildWelcomeScreenCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":WelcomeScreen");
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IGuildOnboarding"/> instance.
    /// </summary>
    /// <param name="GuildID">The guild ID.</param>
    /// <returns>The cache key.</returns>
    public record GuildOnboardingCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Onboarding");
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IThreadMember"/> instance.
    /// </summary>
    /// <param name="ThreadID">The ID of the thread.</param>
    /// <param name="UserID">The ID of the user.</param>
    /// <returns>The cache key.</returns>
    public record ThreadMemberCacheKey(in Snowflake ThreadID, in Snowflake UserID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Thread:{this.ThreadID}:Member:{this.UserID}");
    }

    /// <summary>
    /// Creates a cache key for a <see cref="IAuthorizationInformation"/> instance.
    /// </summary>
    /// <returns>The cache key.</returns>
    public record CurrentAuthorizationInformationCacheKey : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append("CurrentAuthorizationInformation");
    }

    /// <summary>
    /// Creates a cache key for an original interaction <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="Token">The interaction token.</param>
    /// <returns>The cache key.</returns>
    public record OriginalInteractionMessageCacheKey(string Token) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Interaction:{this.Token}:Original");
    }

    /// <summary>
    /// Creates a cache key for an interaction followup <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="Token">The interaction token.</param>
    /// <param name="MessageID">The message ID.</param>
    /// <returns>The cache key.</returns>
    public record FollowupMessageCacheKey(string Token, in Snowflake MessageID)
        : WebhookMessageCacheKey(Token, MessageID);

    /// <summary>
    /// Creates a cache key for a webhook <see cref="IMessage"/> instance.
    /// </summary>
    /// <param name="Token">The webhook token.</param>
    /// <param name="MessageID">The message ID.</param>
    /// <returns>The cache key.</returns>
    public record WebhookMessageCacheKey(string Token, in Snowflake MessageID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Webhook:{this.Token}:Message:{this.MessageID}");
    }

    /// <summary>
    /// Creates a cache key for a set of channel-scoped <see cref="IInvite"/> instances.
    /// </summary>
    /// <param name="ChannelID">The ID of the queried channel.</param>
    /// <returns>The cache key.</returns>
    public record ChannelInvitesCacheKey(in Snowflake ChannelID) : ChannelCacheKey(ChannelID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Invites");
    }

    /// <summary>
    /// Creates a cache key for a set of <see cref="IThreadMember"/> instances.
    /// </summary>
    /// <param name="ChannelID">The ID of the queried channel.</param>
    /// <returns>The cache key.</returns>
    public record ThreadMembersCacheKey(in Snowflake ChannelID) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append($"Thread:{this.ChannelID}:Members");
    }

    /// <summary>
    /// Creates a cache key for a set of guild-scoped <see cref="IEmoji"/> instances.
    /// </summary>
    /// <param name="GuildID">The ID of the guild.</param>
    /// <returns>The cache key.</returns>
    public record GuildEmojisCacheKey(in Snowflake GuildID) : GuildCacheKey(GuildID)
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => base.AppendToString(stringBuilder).Append(":Emojis");
    }

    /// <summary>
    /// Creates a cache key for an evicted entity, identified by the given key.
    /// </summary>
    /// <param name="Key">The original key.</param>
    /// <returns>The eviction key.</returns>
    public record EvictionCacheKey(CacheKey Key) : CacheKey
    {
        /// <inheritdoc/>
        protected override StringBuilder AppendToString(StringBuilder stringBuilder)
            => stringBuilder.Append("Evicted:").Append(this.Key.ToCanonicalString());
    }
}
