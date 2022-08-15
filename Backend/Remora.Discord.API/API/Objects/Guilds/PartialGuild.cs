//
//  PartialGuild.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IPartialGuild" />
[PublicAPI]
public record PartialGuild
(
    Optional<Snowflake> ID = default,
    Optional<string> Name = default,
    Optional<IImageHash?> Icon = default,
    Optional<IImageHash?> Splash = default,
    Optional<IImageHash?> DiscoverySplash = default,
    Optional<bool> IsOwner = default,
    Optional<Snowflake> OwnerID = default,
    Optional<IDiscordPermissionSet> Permissions = default,
    Optional<Snowflake?> AFKChannelID = default,
    Optional<TimeSpan> AFKTimeout = default,
    Optional<VerificationLevel> VerificationLevel = default,
    Optional<MessageNotificationLevel> DefaultMessageNotifications = default,
    Optional<ExplicitContentFilterLevel> ExplicitContentFilter = default,
    Optional<IReadOnlyList<IRole>> Roles = default,
    Optional<IReadOnlyList<IEmoji>> Emojis = default,
    Optional<IReadOnlyList<GuildFeature>> GuildFeatures = default,
    Optional<MultiFactorAuthenticationLevel> MFALevel = default,
    Optional<Snowflake?> ApplicationID = default,
    Optional<bool> IsWidgetEnabled = default,
    Optional<Snowflake?> WidgetChannelID = default,
    Optional<Snowflake?> SystemChannelID = default,
    Optional<SystemChannelFlags> SystemChannelFlags = default,
    Optional<Snowflake?> RulesChannelID = default,
    Optional<int?> MaxPresences = default,
    Optional<int> MaxMembers = default,
    Optional<string?> VanityUrlCode = default,
    Optional<string?> Description = default,
    Optional<IImageHash?> Banner = default,
    Optional<PremiumTier> PremiumTier = default,
    Optional<int> PremiumSubscriptionCount = default,
    Optional<string> PreferredLocale = default,
    Optional<Snowflake?> PublicUpdatesChannelID = default,
    Optional<int> MaxVideoChannelUsers = default,
    Optional<int> ApproximateMemberCount = default,
    Optional<int> ApproximatePresenceCount = default,
    Optional<IWelcomeScreen> WelcomeScreen = default,
    Optional<GuildNSFWLevel> NSFWLevel = default,
    Optional<IReadOnlyList<ISticker>> Stickers = default,
    Optional<bool> IsPremiumProgressBarEnabled = default
) : IPartialGuild;
