//
//  PartialGuild.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;
using Remora.Discord.Generators.Support;

#pragma warning disable CS1591

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Objects.IPartialGuild" />
    [PublicAPI]
    public record PartialGuild
    (
        Optional<Snowflake> ID,
        Optional<string> Name,
        Optional<IImageHash?> Icon,
        Optional<IImageHash?> Splash,
        Optional<IImageHash?> DiscoverySplash,
        Optional<bool> IsOwner,
        Optional<Snowflake> OwnerID,
        Optional<IDiscordPermissionSet> Permissions,
        Optional<string> Region,
        Optional<Snowflake?> AFKChannelID,
        Optional<TimeSpan> AFKTimeout,
        Optional<VerificationLevel> VerificationLevel,
        Optional<MessageNotificationLevel> DefaultMessageNotifications,
        Optional<ExplicitContentFilterLevel> ExplicitContentFilter,
        Optional<IReadOnlyList<IRole>> Roles,
        Optional<IReadOnlyList<IEmoji>> Emojis,
        Optional<IReadOnlyList<GuildFeature>> GuildFeatures,
        Optional<MultiFactorAuthenticationLevel> MFALevel,
        Optional<Snowflake?> ApplicationID,
        Optional<bool> IsWidgetEnabled,
        Optional<Snowflake?> WidgetChannelID,
        Optional<Snowflake?> SystemChannelID,
        Optional<SystemChannelFlags> SystemChannelFlags,
        Optional<Snowflake?> RulesChannelID,
        Optional<DateTimeOffset> JoinedAt,
        Optional<bool> IsLarge,
        Optional<bool> IsUnavailable,
        Optional<int> MemberCount,
        Optional<IReadOnlyList<IPartialVoiceState>> VoiceStates,
        Optional<IReadOnlyList<IGuildMember>> Members,
        Optional<IReadOnlyList<IChannel>> Channels,
        Optional<IReadOnlyList<IPartialPresence>> Presences,
        Optional<int?> MaxPresences,
        Optional<int> MaxMembers,
        Optional<string?> VanityUrlCode,
        Optional<string?> Description,
        Optional<IImageHash?> Banner,
        Optional<PremiumTier> PremiumTier,
        Optional<int> PremiumSubscriptionCount,
        Optional<string> PreferredLocale,
        Optional<Snowflake?> PublicUpdatesChannelID,
        Optional<int> MaxVideoChannelUsers,
        Optional<int> ApproximateMemberCount,
        Optional<int> ApproximatePresenceCount
    ) : IPartialGuild;
}
