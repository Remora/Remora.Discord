//
//  GuildFeature.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Enumerates various guild features.
    /// </summary>
    [PublicAPI]
    public static class GuildFeature
    {
        /// <summary>
        /// The guild has access to set an invite splash background.
        /// </summary>
        public const string InviteSplash
            = "INVITE_SPLASH";

        /// <summary>
        /// The guild has access to set 384kbps bitrate in voice (previously VIP voice servers).
        /// </summary>
        public const string VIPRegions
            = "VIP_REGIONS";

        /// <summary>
        /// The server has access to set a vanity URL.
        /// </summary>
        public const string VanityURL
            = "VANITY_URL";

        /// <summary>
        /// The guild is verified.
        /// </summary>
        public const string Verified
            = "VERIFIED";

        /// <summary>
        /// The guild is partnered.
        /// </summary>
        public const string Partnered
            = "PARTNERED";

        /// <summary>
        /// The guild has community features enabled.
        /// </summary>
        public const string Community
            = "COMMUNITY";

        /// <summary>
        /// The guild has access to use commerce features (i.e, create store channels).
        /// </summary>
        public const string Commerce
            = "COMMERCE";

        /// <summary>
        /// The guild has access to creating news channels.
        /// </summary>
        public const string News
            = "NEWS";

        /// <summary>
        /// The guild is able to be discovered in the guild directory.
        /// </summary>
        public const string Discoverable
            = "DISCOVERABLE";

        /// <summary>
        /// The guild is able to be featured in the guild directory.
        /// </summary>
        public const string Featurable
            = "FEATURABLE";

        /// <summary>
        /// The guild has access to set an animated icon.
        /// </summary>
        public const string AnimatedIcon
            = "ANIMATED_ICON";

        /// <summary>
        /// The guild has access to set a guild banner image.
        /// </summary>
        public const string Banner
            = "BANNER";

        /// <summary>
        /// The guild has enabled a welcome screen.
        /// </summary>
        public const string WelcomeScreenEnabled
            = "WELCOME_SCREEN_ENABLED";

        /// <summary>
        /// The guild has enabled membership screening.
        /// </summary>
        public const string MemberVerificationGateEnabled
            = "MEMBER_VERIFICATION_GATE_ENABLED";

        /// <summary>
        /// The guild can be previewed before joining.
        /// </summary>
        public const string PreviewEnabled
            = "PREVIEW_ENABLED";

        /// <summary>
        /// The guild was discoverable before the "Discovery Checklist" feature was launched.
        /// </summary>
        public const string EnabledDiscoverableBefore
            = "ENABLED_DISCOVERABLE_BEFORE";
    }
}
