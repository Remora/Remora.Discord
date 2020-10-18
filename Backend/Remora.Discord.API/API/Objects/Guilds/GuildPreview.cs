//
//  GuildPreview.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class GuildPreview : IGuildPreview
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
        public IReadOnlyList<IEmoji> Emojis { get; }

        /// <inheritdoc />
        public IReadOnlyList<GuildFeature> Features { get; }

        /// <inheritdoc />
        public Optional<int> ApproximatePresenceCount { get; }

        /// <inheritdoc />
        public Optional<int> ApproximateMemberCount { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildPreview"/> class.
        /// </summary>
        /// <param name="id">The ID of the guild.</param>
        /// <param name="name">The name of the guild.</param>
        /// <param name="icon">The guild's icon.</param>
        /// <param name="splash">The guild's splash.</param>
        /// <param name="discoverySplash">The guild's discovery splash.</param>
        /// <param name="emojis">The emojis in the guild.</param>
        /// <param name="features">The features the guild has.</param>
        /// <param name="approximatePresenceCount">The approximate presence count.</param>
        /// <param name="approximateMemberCount">The approximate member count.</param>
        /// <param name="description">The guild's description.</param>
        public GuildPreview
        (
            Snowflake id,
            string name,
            IImageHash? icon,
            IImageHash? splash,
            IImageHash? discoverySplash,
            IReadOnlyList<IEmoji> emojis,
            IReadOnlyList<GuildFeature> features,
            Optional<int> approximatePresenceCount,
            Optional<int> approximateMemberCount,
            string? description
        )
        {
            this.ID = id;
            this.Name = name;
            this.Icon = icon;
            this.Splash = splash;
            this.DiscoverySplash = discoverySplash;
            this.Emojis = emojis;
            this.Features = features;
            this.ApproximatePresenceCount = approximatePresenceCount;
            this.ApproximateMemberCount = approximateMemberCount;
            this.Description = description;
        }
    }
}
