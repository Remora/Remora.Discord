//
//  IPartialGuild.cs
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
/// Represents a partial Discord Guild.
/// </summary>
[PublicAPI]
public interface IPartialGuild
{
    /// <inheritdoc cref="IGuild.ID" />
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IGuild.Name" />
    Optional<string> Name { get; }

    /// <inheritdoc cref="IGuild.Icon" />
    Optional<IImageHash?> Icon { get; }

    /// <inheritdoc cref="IGuild.Splash" />
    Optional<IImageHash?> Splash { get; }

    /// <inheritdoc cref="IGuild.DiscoverySplash" />
    Optional<IImageHash?> DiscoverySplash { get; }

    /// <inheritdoc cref="IGuild.IsOwner" />
    Optional<bool> IsOwner { get; }

    /// <inheritdoc cref="IGuild.OwnerID" />
    Optional<Snowflake> OwnerID { get; }

    /// <inheritdoc cref="IGuild.Permissions" />
    Optional<IDiscordPermissionSet> Permissions { get; }

    /// <inheritdoc cref="IGuild.AFKChannelID" />
    Optional<Snowflake?> AFKChannelID { get; }

    /// <inheritdoc cref="IGuild.AFKTimeout" />
    Optional<TimeSpan> AFKTimeout { get; }

    /// <inheritdoc cref="IGuild.VerificationLevel" />
    Optional<VerificationLevel> VerificationLevel { get; }

    /// <inheritdoc cref="IGuild.DefaultMessageNotifications" />
    Optional<MessageNotificationLevel> DefaultMessageNotifications { get; }

    /// <inheritdoc cref="IGuild.ExplicitContentFilter" />
    Optional<ExplicitContentFilterLevel> ExplicitContentFilter { get; }

    /// <inheritdoc cref="IGuild.Roles" />
    Optional<IReadOnlyList<IRole>> Roles { get; }

    /// <inheritdoc cref="IGuild.Emojis" />
    Optional<IReadOnlyList<IEmoji>> Emojis { get; }

    /// <inheritdoc cref="IGuild.GuildFeatures" />
    Optional<IReadOnlyList<GuildFeature>> GuildFeatures { get; }

    /// <inheritdoc cref="IGuild.MFALevel" />
    Optional<MultiFactorAuthenticationLevel> MFALevel { get; }

    /// <inheritdoc cref="IGuild.ApplicationID" />
    Optional<Snowflake?> ApplicationID { get; }

    /// <inheritdoc cref="IGuild.IsWidgetEnabled" />
    Optional<bool> IsWidgetEnabled { get; }

    /// <inheritdoc cref="IGuild.WidgetChannelID" />
    Optional<Snowflake?> WidgetChannelID { get; }

    /// <inheritdoc cref="IGuild.SystemChannelID" />
    Optional<Snowflake?> SystemChannelID { get; }

    /// <inheritdoc cref="IGuild.SystemChannelFlags" />
    Optional<SystemChannelFlags> SystemChannelFlags { get; }

    /// <inheritdoc cref="IGuild.RulesChannelID" />
    Optional<Snowflake?> RulesChannelID { get; }

    /// <inheritdoc cref="IGuild.MaxPresences" />
    Optional<int?> MaxPresences { get; }

    /// <inheritdoc cref="IGuild.MaxMembers" />
    Optional<int> MaxMembers { get; }

    /// <inheritdoc cref="IGuild.VanityUrlCode" />
    Optional<string?> VanityUrlCode { get; }

    /// <inheritdoc cref="IGuild.Description" />
    Optional<string?> Description { get; }

    /// <inheritdoc cref="IGuild.Banner" />
    Optional<IImageHash?> Banner { get; }

    /// <inheritdoc cref="IGuild.PremiumTier" />
    Optional<PremiumTier> PremiumTier { get; }

    /// <inheritdoc cref="IGuild.PremiumSubscriptionCount" />
    Optional<int> PremiumSubscriptionCount { get; }

    /// <inheritdoc cref="IGuild.PreferredLocale" />
    Optional<string> PreferredLocale { get; }

    /// <inheritdoc cref="IGuild.PublicUpdatesChannelID" />
    Optional<Snowflake?> PublicUpdatesChannelID { get; }

    /// <inheritdoc cref="IGuild.MaxVideoChannelUsers" />
    Optional<int> MaxVideoChannelUsers { get; }

    /// <inheritdoc cref="IGuild.ApproximateMemberCount" />
    Optional<int> ApproximateMemberCount { get; }

    /// <inheritdoc cref="IGuild.ApproximatePresenceCount" />
    Optional<int> ApproximatePresenceCount { get; }

    /// <inheritdoc cref="IGuild.WelcomeScreen" />
    Optional<IWelcomeScreen> WelcomeScreen { get; }

    /// <inheritdoc cref="IGuild.NSFWLevel" />
    Optional<GuildNSFWLevel> NSFWLevel { get; }

    /// <inheritdoc cref="IGuild.Stickers"/>
    Optional<IReadOnlyList<ISticker>> Stickers { get; }

    /// <inheritdoc cref="IGuild.IsPremiumProgressBarEnabled"/>
    Optional<bool> IsPremiumProgressBarEnabled { get; }
}
