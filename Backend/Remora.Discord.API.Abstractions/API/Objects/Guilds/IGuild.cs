//
//  IGuild.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a Discord Guild.
/// </summary>
[PublicAPI]
public interface IGuild : IPartialGuild
{
    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the guild.
    /// </summary>
    new string Name { get; }

    /// <summary>
    /// Gets the guild's icon.
    /// </summary>
    new IImageHash? Icon { get; }

    /// <summary>
    /// Gets the guild's splash banner.
    /// </summary>
    new IImageHash? Splash { get; }

    /// <summary>
    /// Gets the guild's Discovery splash banner.
    /// </summary>
    new IImageHash? DiscoverySplash { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is the guild's owner.
    /// </summary>
    new Optional<bool> IsOwner { get; }

    /// <summary>
    /// Gets the ID of the owner.
    /// </summary>
    new Snowflake OwnerID { get; }

    /// <summary>
    /// Gets the permissions for the current user in the guild.
    /// </summary>
    new Optional<IDiscordPermissionSet> Permissions { get; }

    /// <summary>
    /// Gets the ID of the AFK channel.
    /// </summary>
    new Snowflake? AFKChannelID { get; }

    /// <summary>
    /// Gets the AFK timeout (in seconds).
    /// </summary>
    new TimeSpan AFKTimeout { get; }

    /// <summary>
    /// Gets the verification level required for the guild.
    /// </summary>
    new VerificationLevel VerificationLevel { get; }

    /// <summary>
    /// Gets the default notification level for the guild.
    /// </summary>
    new MessageNotificationLevel DefaultMessageNotifications { get; }

    /// <summary>
    /// Gets the explicit content level.
    /// </summary>
    new ExplicitContentFilterLevel ExplicitContentFilter { get; }

    /// <summary>
    /// Gets a list of the roles in the server.
    /// </summary>
    new IReadOnlyList<IRole> Roles { get; }

    /// <summary>
    /// Gets a list of emojis in the server.
    /// </summary>
    new IReadOnlyList<IEmoji> Emojis { get; }

    /// <summary>
    /// Gets a list of guild features.
    /// </summary>
    new IReadOnlyList<GuildFeature> GuildFeatures { get; }

    /// <summary>
    /// Gets the required MFA level for the guild.
    /// </summary>
    new MultiFactorAuthenticationLevel MFALevel { get; }

    /// <summary>
    /// Gets the application ID of the guild creator if it is bot-created.
    /// </summary>
    new Snowflake? ApplicationID { get; }

    /// <summary>
    /// Gets a value indicating whether the server widget is enabled.
    /// </summary>
    new Optional<bool> IsWidgetEnabled { get; }

    /// <summary>
    /// Gets the ID of the channel the widget generates invites to.
    /// </summary>
    new Optional<Snowflake?> WidgetChannelID { get; }

    /// <summary>
    /// Gets the ID of the channel that system messages are sent to.
    /// </summary>
    new Snowflake? SystemChannelID { get; }

    /// <summary>
    /// Gets the flags on the system channel.
    /// </summary>
    new SystemChannelFlags SystemChannelFlags { get; }

    /// <summary>
    /// Gets the ID of the rules channel, if any. This is the channel where community-enabled guilds can display
    /// rules and/or guidelines.
    /// </summary>
    new Snowflake? RulesChannelID { get; }

    /// <summary>
    /// Gets the maximum number of presences for the guild. This is null for all but the largest of guilds.
    /// </summary>
    new Optional<int?> MaxPresences { get; }

    /// <summary>
    /// Gets the maximum number of members for the guild.
    /// </summary>
    new Optional<int> MaxMembers { get; }

    /// <summary>
    /// Gets the vanity url code for the guild.
    /// </summary>
    new string? VanityUrlCode { get; }

    /// <summary>
    /// Gets the description of the guild.
    /// </summary>
    new string? Description { get; }

    /// <summary>
    /// Gets the hash of the guild banner.
    /// </summary>
    new IImageHash? Banner { get; }

    /// <summary>
    /// Gets the boost level of the guild.
    /// </summary>
    new PremiumTier PremiumTier { get; }

    /// <summary>
    /// Gets the number of boosts the guild currently has.
    /// </summary>
    new Optional<int> PremiumSubscriptionCount { get; }

    /// <summary>
    /// Gets the preferred locale of a public-enabled guild.
    /// </summary>
    new string PreferredLocale { get; }

    /// <summary>
    /// Gets the ID of the channel where admins and moderators of community-enabled guilds receive notices from
    /// Discord.
    /// </summary>
    new Snowflake? PublicUpdatesChannelID { get; }

    /// <summary>
    /// Gets the maximum number of users in a video channel.
    /// </summary>
    new Optional<int> MaxVideoChannelUsers { get; }

    /// <summary>
    /// Gets the approximate number of members in the guild.
    /// </summary>
    new Optional<int> ApproximateMemberCount { get; }

    /// <summary>
    /// Gets the approximate number of non-offline members in the guild.
    /// </summary>
    new Optional<int> ApproximatePresenceCount { get; }

    /// <summary>
    /// Gets the welcome screen shown to new members.
    /// </summary>
    new Optional<IWelcomeScreen> WelcomeScreen { get; }

    /// <summary>
    /// Gets the guild's NSFW level.
    /// </summary>
    new GuildNSFWLevel NSFWLevel { get; }

    /// <summary>
    /// Gets the stickers in the guild.
    /// </summary>
    new Optional<IReadOnlyList<ISticker>> Stickers { get; }

    /// <summary>
    /// Gets a value indicating whether the guild has the boost progress bar enabled.
    /// </summary>
    new bool IsPremiumProgressBarEnabled { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialGuild.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialGuild.Name => this.Name;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialGuild.Icon => new(this.Icon);

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialGuild.Splash => new(this.Splash);

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialGuild.DiscoverySplash => new(this.DiscoverySplash);

    /// <inheritdoc/>
    Optional<bool> IPartialGuild.IsOwner => this.IsOwner;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialGuild.OwnerID => this.OwnerID;

    /// <inheritdoc/>
    Optional<IDiscordPermissionSet> IPartialGuild.Permissions => this.Permissions;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialGuild.AFKChannelID => this.AFKChannelID;

    /// <inheritdoc/>
    Optional<TimeSpan> IPartialGuild.AFKTimeout => this.AFKTimeout;

    /// <inheritdoc/>
    Optional<VerificationLevel> IPartialGuild.VerificationLevel => this.VerificationLevel;

    /// <inheritdoc/>
    Optional<MessageNotificationLevel> IPartialGuild.DefaultMessageNotifications => this.DefaultMessageNotifications;

    /// <inheritdoc/>
    Optional<ExplicitContentFilterLevel> IPartialGuild.ExplicitContentFilter => this.ExplicitContentFilter;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IRole>> IPartialGuild.Roles => new(this.Roles);

    /// <inheritdoc/>
    Optional<IReadOnlyList<IEmoji>> IPartialGuild.Emojis => new(this.Emojis);

    /// <inheritdoc/>
    Optional<IReadOnlyList<GuildFeature>> IPartialGuild.GuildFeatures => new(this.GuildFeatures);

    /// <inheritdoc/>
    Optional<MultiFactorAuthenticationLevel> IPartialGuild.MFALevel => this.MFALevel;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialGuild.ApplicationID => this.ApplicationID;

    /// <inheritdoc/>
    Optional<bool> IPartialGuild.IsWidgetEnabled => this.IsWidgetEnabled;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialGuild.WidgetChannelID => this.WidgetChannelID;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialGuild.SystemChannelID => this.SystemChannelID;

    /// <inheritdoc/>
    Optional<SystemChannelFlags> IPartialGuild.SystemChannelFlags => this.SystemChannelFlags;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialGuild.RulesChannelID => this.RulesChannelID;

    /// <inheritdoc/>
    Optional<int?> IPartialGuild.MaxPresences => this.MaxPresences;

    /// <inheritdoc/>
    Optional<int> IPartialGuild.MaxMembers => this.MaxMembers;

    /// <inheritdoc/>
    Optional<string?> IPartialGuild.VanityUrlCode => this.VanityUrlCode;

    /// <inheritdoc/>
    Optional<string?> IPartialGuild.Description => this.Description;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialGuild.Banner => new(this.Banner);

    /// <inheritdoc/>
    Optional<PremiumTier> IPartialGuild.PremiumTier => this.PremiumTier;

    /// <inheritdoc/>
    Optional<int> IPartialGuild.PremiumSubscriptionCount => this.PremiumSubscriptionCount;

    /// <inheritdoc/>
    Optional<string> IPartialGuild.PreferredLocale => this.PreferredLocale;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialGuild.PublicUpdatesChannelID => this.PublicUpdatesChannelID;

    /// <inheritdoc/>
    Optional<int> IPartialGuild.MaxVideoChannelUsers => this.MaxVideoChannelUsers;

    /// <inheritdoc/>
    Optional<int> IPartialGuild.ApproximateMemberCount => this.ApproximateMemberCount;

    /// <inheritdoc/>
    Optional<int> IPartialGuild.ApproximatePresenceCount => this.ApproximatePresenceCount;

    /// <inheritdoc/>
    Optional<IWelcomeScreen> IPartialGuild.WelcomeScreen => this.WelcomeScreen;

    /// <inheritdoc/>
    Optional<GuildNSFWLevel> IPartialGuild.NSFWLevel => this.NSFWLevel;

    /// <inheritdoc/>
    Optional<IReadOnlyList<ISticker>> IPartialGuild.Stickers => this.Stickers;

    /// <inheritdoc/>
    Optional<bool> IPartialGuild.IsPremiumProgressBarEnabled => this.IsPremiumProgressBarEnabled;
}
