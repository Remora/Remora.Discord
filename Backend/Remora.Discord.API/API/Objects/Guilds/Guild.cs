//
//  Guild.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Guild : IGuild
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IImageHash? Icon { get; }

        /// <inheritdoc />
        public IImageHash? Splash { get; }

        /// <inheritdoc />
        public IImageHash? DiscoverySplash { get; }

        /// <inheritdoc />
        public Optional<bool> IsOwner { get; }

        /// <inheritdoc />
        public Snowflake OwnerID { get; }

        /// <inheritdoc />
        public Optional<IDiscordPermissionSet> Permissions { get; }

        /// <inheritdoc />
        public string Region { get; }

        /// <inheritdoc />
        public Snowflake? AFKChannelID { get; }

        /// <inheritdoc />
        public int AFKTimeout { get; }

        /// <inheritdoc />
        public VerificationLevel VerificationLevel { get; }

        /// <inheritdoc />
        public MessageNotificationLevel DefaultMessageNotifications { get; }

        /// <inheritdoc />
        public ExplicitContentFilterLevel ExplicitContentFilter { get; }

        /// <inheritdoc />
        public IReadOnlyList<IRole> Roles { get; }

        /// <inheritdoc />
        public IReadOnlyList<IEmoji> Emojis { get; }

        /// <inheritdoc />
        public IReadOnlyList<GuildFeature> GuildFeatures { get; }

        /// <inheritdoc />
        public MultiFactorAuthenticationLevel MFALevel { get; }

        /// <inheritdoc />
        public Snowflake? ApplicationID { get; }

        /// <inheritdoc />
        public Optional<bool> IsWidgetEnabled { get; }

        /// <inheritdoc />
        public Optional<Snowflake?> WidgetChannelID { get; }

        /// <inheritdoc />
        public Snowflake? SystemChannelID { get; }

        /// <inheritdoc />
        public SystemChannelFlags SystemChannelFlags { get; }

        /// <inheritdoc />
        public Snowflake? RulesChannelID { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> JoinedAt { get; }

        /// <inheritdoc />
        public Optional<bool> IsLarge { get; }

        /// <inheritdoc />
        public Optional<bool> IsUnavailable { get; }

        /// <inheritdoc />
        public Optional<int> MemberCount { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IPartialVoiceState>> VoiceStates { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IGuildMember>> Members { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IChannel>> Channels { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IPartialPresence>> Presences { get; }

        /// <inheritdoc />
        public Optional<int?> MaxPresences { get; }

        /// <inheritdoc />
        public Optional<int> MaxMembers { get; }

        /// <inheritdoc />
        public string? VanityUrlCode { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <inheritdoc />
        public IImageHash? Banner { get; }

        /// <inheritdoc />
        public PremiumTier PremiumTier { get; }

        /// <inheritdoc />
        public Optional<int> PremiumSubscriptionCount { get; }

        /// <inheritdoc />
        public string PreferredLocale { get; }

        /// <inheritdoc />
        public Snowflake? PublicUpdatesChannelID { get; }

        /// <inheritdoc />
        public Optional<int> MaxVideoChannelUsers { get; }

        /// <inheritdoc />
        public Optional<int> ApproximateMemberCount { get; }

        /// <inheritdoc />
        public Optional<int> ApproximatePresenceCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Guild"/> class.
        /// </summary>
        /// <param name="id">The ID of the guild.</param>
        /// <param name="name">The name of the guild.</param>
        /// <param name="icon">The icon of the guild.</param>
        /// <param name="splash">The guild's splash.</param>
        /// <param name="discoverySplash">The guild's discovery splash.</param>
        /// <param name="isOwner">Whether the current owner is the guild's owner.</param>
        /// <param name="ownerID">The ID of the guild owner.</param>
        /// <param name="permissions">The permissions the current user has.</param>
        /// <param name="region">The guild's region.</param>
        /// <param name="afkChannelID">The ID of the AFK channel.</param>
        /// <param name="afkTimeout">The AFK timeout.</param>
        /// <param name="verificationLevel">The verification level of the guild.</param>
        /// <param name="defaultMessageNotifications">The default message notification level of the guild.</param>
        /// <param name="explicitContentFilter">The content filter level.</param>
        /// <param name="roles">The roles in the guild.</param>
        /// <param name="emojis">The emojis in the guild.</param>
        /// <param name="guildFeatures">The features the guild has.</param>
        /// <param name="mfaLevel">The MFA level of the guild.</param>
        /// <param name="applicationID">The application ID of the guild's creator.</param>
        /// <param name="isWidgetEnabled">Whether the guild widget is enabled.</param>
        /// <param name="widgetChannelID">The ID of the channel that the guild widget generates invites to.</param>
        /// <param name="systemChannelID">The ID of the system message channel.</param>
        /// <param name="systemChannelFlags">The flags of the system message channel.</param>
        /// <param name="rulesChannelID">The ID of the rules channel.</param>
        /// <param name="joinedAt">The time at which the current user joined the guild.</param>
        /// <param name="isLarge">Whether the guild is considered large.</param>
        /// <param name="isUnavailable">Whether the guild is unavailable due to an outage.</param>
        /// <param name="memberCount">The number of guild members.</param>
        /// <param name="voiceStates">The voice states of the members.</param>
        /// <param name="members">The members of the guild.</param>
        /// <param name="channels">The channels of the guild.</param>
        /// <param name="presences">The presences of the members.</param>
        /// <param name="maxPresences">The maximum number of returned presences.</param>
        /// <param name="maxMembers">The maximum number of returned members.</param>
        /// <param name="vanityUrlCode">The guild's vanity URL code.</param>
        /// <param name="description">The discovery description of the guild.</param>
        /// <param name="banner">The guild banner.</param>
        /// <param name="premiumTier">The boost level of the guild.</param>
        /// <param name="premiumSubscriptionCount">The number of boosters the guild has.</param>
        /// <param name="preferredLocale">The preferred locale of the guild.</param>
        /// <param name="publicUpdatesChannelID">The ID of the public updates channel.</param>
        /// <param name="maxVideoChannelUsers">The maximum number of video channel users.</param>
        /// <param name="approximateMemberCount">The approximate member count.</param>
        /// <param name="approximatePresenceCount">The approximate non-offline member count.</param>
        public Guild
        (
            Snowflake id,
            string name,
            IImageHash? icon,
            IImageHash? splash,
            IImageHash? discoverySplash,
            Optional<bool> isOwner,
            Snowflake ownerID,
            Optional<IDiscordPermissionSet> permissions,
            string region,
            Snowflake? afkChannelID,
            int afkTimeout,
            VerificationLevel verificationLevel,
            MessageNotificationLevel defaultMessageNotifications,
            ExplicitContentFilterLevel explicitContentFilter,
            IReadOnlyList<IRole> roles,
            IReadOnlyList<IEmoji> emojis,
            IReadOnlyList<GuildFeature> guildFeatures,
            MultiFactorAuthenticationLevel mfaLevel,
            Snowflake? applicationID,
            Optional<bool> isWidgetEnabled,
            Optional<Snowflake?> widgetChannelID,
            Snowflake? systemChannelID,
            SystemChannelFlags systemChannelFlags,
            Snowflake? rulesChannelID,
            Optional<DateTimeOffset> joinedAt,
            Optional<bool> isLarge,
            Optional<bool> isUnavailable,
            Optional<int> memberCount,
            Optional<IReadOnlyList<IPartialVoiceState>> voiceStates,
            Optional<IReadOnlyList<IGuildMember>> members,
            Optional<IReadOnlyList<IChannel>> channels,
            Optional<IReadOnlyList<IPartialPresence>> presences,
            Optional<int?> maxPresences,
            Optional<int> maxMembers,
            string? vanityUrlCode,
            string? description,
            IImageHash? banner,
            PremiumTier premiumTier,
            Optional<int> premiumSubscriptionCount,
            string preferredLocale,
            Snowflake? publicUpdatesChannelID,
            Optional<int> maxVideoChannelUsers,
            Optional<int> approximateMemberCount,
            Optional<int> approximatePresenceCount
        )
        {
            this.ID = id;
            this.Name = name;
            this.Icon = icon;
            this.Splash = splash;
            this.DiscoverySplash = discoverySplash;
            this.IsOwner = isOwner;
            this.OwnerID = ownerID;
            this.Permissions = permissions;
            this.Region = region;
            this.AFKChannelID = afkChannelID;
            this.AFKTimeout = afkTimeout;
            this.VerificationLevel = verificationLevel;
            this.DefaultMessageNotifications = defaultMessageNotifications;
            this.ExplicitContentFilter = explicitContentFilter;
            this.Roles = roles;
            this.Emojis = emojis;
            this.GuildFeatures = guildFeatures;
            this.MFALevel = mfaLevel;
            this.ApplicationID = applicationID;
            this.IsWidgetEnabled = isWidgetEnabled;
            this.WidgetChannelID = widgetChannelID;
            this.SystemChannelID = systemChannelID;
            this.SystemChannelFlags = systemChannelFlags;
            this.RulesChannelID = rulesChannelID;
            this.JoinedAt = joinedAt;
            this.IsLarge = isLarge;
            this.IsUnavailable = isUnavailable;
            this.MemberCount = memberCount;
            this.VoiceStates = voiceStates;
            this.Members = members;
            this.Channels = channels;
            this.Presences = presences;
            this.MaxPresences = maxPresences;
            this.MaxMembers = maxMembers;
            this.VanityUrlCode = vanityUrlCode;
            this.Description = description;
            this.Banner = banner;
            this.PremiumTier = premiumTier;
            this.PremiumSubscriptionCount = premiumSubscriptionCount;
            this.PreferredLocale = preferredLocale;
            this.PublicUpdatesChannelID = publicUpdatesChannelID;
            this.MaxVideoChannelUsers = maxVideoChannelUsers;
            this.ApproximateMemberCount = approximateMemberCount;
            this.ApproximatePresenceCount = approximatePresenceCount;
        }
    }
}
