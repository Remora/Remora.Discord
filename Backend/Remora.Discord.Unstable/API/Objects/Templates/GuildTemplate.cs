//
//  GuildTemplate.cs
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

using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class GuildTemplate : IGuildTemplate
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <inheritdoc />
        public string Region { get; }

        /// <inheritdoc />
        public IImageHash? IconHash { get; }

        /// <inheritdoc />
        public VerificationLevel VerificationLevel { get; }

        /// <inheritdoc />
        public MessageNotificationLevel DefaultMessageNotifications { get; }

        /// <inheritdoc />
        public ExplicitContentFilterLevel ExplicitContentFilter { get; }

        /// <inheritdoc />
        public string PreferredLocale { get; }

        /// <inheritdoc />
        public int AFKTimeout { get; }

        /// <inheritdoc />
        public IReadOnlyList<IRoleTemplate> Roles { get; }

        /// <inheritdoc />
        public IReadOnlyList<IChannelTemplate> Channels { get; }

        /// <inheritdoc />
        public int? AFKChannelID { get; }

        /// <inheritdoc />
        public int? SystemChannelID { get; }

        /// <inheritdoc />
        public SystemChannelFlags SystemChannelFlags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildTemplate"/> class.
        /// </summary>
        /// <param name="name">The name of the guild.</param>
        /// <param name="iconHash">The icon of the guild.</param>
        /// <param name="region">The guild's region.</param>
        /// <param name="afkChannelID">The ID of the AFK channel.</param>
        /// <param name="afkTimeout">The AFK timeout.</param>
        /// <param name="verificationLevel">The verification level of the guild.</param>
        /// <param name="defaultMessageNotifications">The default message notification level of the guild.</param>
        /// <param name="explicitContentFilter">The content filter level.</param>
        /// <param name="roles">The roles in the guild.</param>
        /// <param name="systemChannelID">The ID of the system message channel.</param>
        /// <param name="systemChannelFlags">The flags of the system message channel.</param>
        /// <param name="channels">The channels of the guild.</param>
        /// <param name="description">The discovery description of the guild.</param>
        /// <param name="preferredLocale">The preferred locale of the guild.</param>
        public GuildTemplate
        (
            string name,
            string? description,
            string region,
            IImageHash? iconHash,
            VerificationLevel verificationLevel,
            MessageNotificationLevel defaultMessageNotifications,
            ExplicitContentFilterLevel explicitContentFilter,
            string preferredLocale,
            int afkTimeout,
            IReadOnlyList<IRoleTemplate> roles,
            IReadOnlyList<IChannelTemplate> channels,
            int? afkChannelID,
            int? systemChannelID,
            SystemChannelFlags systemChannelFlags
        )
        {
            this.Name = name;
            this.Description = description;
            this.Region = region;
            this.IconHash = iconHash;
            this.VerificationLevel = verificationLevel;
            this.DefaultMessageNotifications = defaultMessageNotifications;
            this.ExplicitContentFilter = explicitContentFilter;
            this.PreferredLocale = preferredLocale;
            this.AFKTimeout = afkTimeout;
            this.Roles = roles;
            this.Channels = channels;
            this.AFKChannelID = afkChannelID;
            this.SystemChannelID = systemChannelID;
            this.SystemChannelFlags = systemChannelFlags;
        }
    }
}
