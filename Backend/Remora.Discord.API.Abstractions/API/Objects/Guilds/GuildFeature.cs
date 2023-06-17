//
//  GuildFeature.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various guild features.
/// </summary>
[PublicAPI]
public enum GuildFeature
{
    /// <summary>
    /// The guild has access to set an animated guild banner image.
    /// </summary>
    AnimatedBanner,

    /// <summary>
    /// The guild has access to set an animated icon.
    /// </summary>
    AnimatedIcon,

    /// <summary>
    /// The guild is using the old permission configuration behaviour.
    /// </summary>
    ApplicationCommandPermissionsV2,

    /// <summary>
    /// The guild has set up Auto Moderation rules.
    /// </summary>
    AutoModeration,

    /// <summary>
    /// The guild has access to set a guild banner image.
    /// </summary>
    Banner,

    /// <summary>
    /// The guild has community features enabled.
    /// </summary>
    Community,

    /// <summary>
    /// The guild has enabled monetization.
    /// </summary>
    CreatorMonetizableProvisional,

    /// <summary>
    /// The guild has enabled the role subscriptions promo page.
    /// </summary>
    CreatorStorePage,

    /// <summary>
    /// The guild has been set as a support server on the app directory.
    /// </summary>
    DeveloperSupportServer,

    /// <summary>
    /// The guild is able to be discovered in the guild directory.
    /// </summary>
    Discoverable,

    /// <summary>
    /// The guild is able to be featured in the guild directory.
    /// </summary>
    Featurable,

    /// <summary>
    /// The guild has paused invites, preventing new users from joining.
    /// </summary>
    InvitesDisabled,

    /// <summary>
    /// The guild has access to set an invite splash background.
    /// </summary>
    InviteSplash,

    /// <summary>
    /// The guild has enabled membership screening.
    /// </summary>
    MemberVerificationGateEnabled,

    /// <summary>
    /// The guild has enabled monetization.
    /// </summary>
    MonetizationEnabled,

    /// <summary>
    /// The guild has increased custom sticker slots.
    /// </summary>
    MoreStickers,

    /// <summary>
    /// The guild has access to creating announcement channels.
    /// </summary>
    News,

    /// <summary>
    /// The guild is partnered.
    /// </summary>
    Partnered,

    /// <summary>
    /// The guild can be previewed before joining.
    /// </summary>
    PreviewEnabled,

    /// <summary>
    /// The guild has disabled alerts for join raids in the configured safety alerts channel.
    /// </summary>
    RaidAlertsDisabled,

    /// <summary>
    /// The guild is able to set role icons.
    /// </summary>
    RoleIcons,

    /// <summary>
    /// The guild has role subscriptions that can be purchased.
    /// </summary>
    RoleSubscriptionsAvailableForPurchase,

    /// <summary>
    /// The guild has enabled role subscriptions.
    /// </summary>
    RoleSubscriptionsEnabled,

    /// <summary>
    /// The guild has enabled ticketed events.
    /// </summary>
    TicketedEventsEnabled,

    /// <summary>
    /// The server has access to set a vanity URL.
    /// </summary>
    VanityURL,

    /// <summary>
    /// The guild is verified.
    /// </summary>
    Verified,

    /// <summary>
    /// The guild has access to set 384kbps bitrate in voice (previously VIP voice servers).
    /// </summary>
    VIPRegions,

    /// <summary>
    /// The guild has enabled a welcome screen.
    /// </summary>
    WelcomeScreenEnabled
}
