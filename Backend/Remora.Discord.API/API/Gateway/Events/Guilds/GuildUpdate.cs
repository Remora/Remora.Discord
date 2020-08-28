//
//  GuildUpdate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc cref="IGuildUpdate"/>
    public class GuildUpdate : Guild, IGuildUpdate
    {
        /// <inheritdoc cref="Guild"/>
        public GuildUpdate
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
            Optional<bool> isEmbedEnabled,
            Optional<Snowflake?> embedChannelID,
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
            Optional<IReadOnlyList<IVoiceState>> voiceStates,
            Optional<IReadOnlyList<IGuildMember>> members,
            Optional<IReadOnlyList<IChannel>> channels,
            Optional<IReadOnlyList<IPresence>> presences,
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
            : base
            (
                id,
                name,
                icon,
                splash,
                discoverySplash,
                isOwner,
                ownerID,
                permissions,
                region,
                afkChannelID,
                afkTimeout,
                isEmbedEnabled,
                embedChannelID,
                verificationLevel,
                defaultMessageNotifications,
                explicitContentFilter,
                roles,
                emojis,
                guildFeatures,
                mfaLevel,
                applicationID,
                isWidgetEnabled,
                widgetChannelID,
                systemChannelID,
                systemChannelFlags,
                rulesChannelID,
                joinedAt,
                isLarge,
                isUnavailable,
                memberCount,
                voiceStates,
                members,
                channels,
                presences,
                maxPresences,
                maxMembers,
                vanityUrlCode,
                description,
                banner,
                premiumTier,
                premiumSubscriptionCount,
                preferredLocale,
                publicUpdatesChannelID,
                maxVideoChannelUsers,
                approximateMemberCount,
                approximatePresenceCount
            )
        {
        }
    }
}
