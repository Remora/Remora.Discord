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

namespace Remora.Discord.API.Abstractions
{
    /// <summary>
    /// Enumerates various guild features.
    /// </summary>
    public enum GuildFeature
    {
        /// <summary>
        /// The guild has access to set an invite splash background.
        /// </summary>
        InviteSplash,

        /// <summary>
        /// The guild has access to set 384kbps bitrate in voice (previously VIP voice servers).
        /// </summary>
        VIPRegions,

        /// <summary>
        /// The server has access to set a vanity URL.
        /// </summary>
        VanityURL,

        /// <summary>
        /// The guild is verified.
        /// </summary>
        Verified,

        /// <summary>
        /// The guild is partnered.
        /// </summary>
        Partnered,

        /// <summary>
        /// The guild is public.
        /// </summary>
        Public,

        /// <summary>
        /// The guild has access to use commerce features (i.e, create store channels).
        /// </summary>
        Commerce,

        /// <summary>
        /// The guild has access to creating news channels.
        /// </summary>
        News,

        /// <summary>
        /// The guild is able to be discovered in the guild directory.
        /// </summary>
        Discoverable,

        /// <summary>
        /// The guild is able to be featured in the guild directory.
        /// </summary>
        Featurable,

        /// <summary>
        /// The guild has access to set an animated icon.
        /// </summary>
        AnimatedIcon,

        /// <summary>
        /// The guild has access to set a guild banner image.
        /// </summary>
        Banner,

        /// <summary>
        /// The guild cannot be public.
        /// </summary>
        PublicDisabled,

        /// <summary>
        /// The guild has enabled a welcome screen.
        /// </summary>
        WelcomeScreenEnabled
    }
}
