//
//  Activity.cs
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
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <summary>
    /// Represents information about an activity Discord is aware of.
    /// </summary>
    public class Activity : IActivity
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public ActivityType Type { get; }

        /// <inheritdoc />
        public Optional<Uri?> Url { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> CreatedAt { get; }

        /// <inheritdoc />
        public Optional<IActivityTimestamps> Timestamps { get; }

        /// <inheritdoc />
        public Optional<string?> Details { get; }

        /// <inheritdoc />
        public Optional<string?> State { get; }

        /// <inheritdoc />
        public Optional<IActivityEmoji?> Emoji { get; }

        /// <inheritdoc />
        public Optional<IActivityParty> Party { get; }

        /// <inheritdoc />
        public Optional<IActivityAssets> Assets { get; }

        /// <inheritdoc />
        public Optional<IActivitySecrets> Secrets { get; }

        /// <inheritdoc />
        public Optional<bool> Instance { get; }

        /// <inheritdoc />
        public Optional<ActivityFlags> Flags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="type">The type of activity.</param>
        /// <param name="createdAt">The time when the activity was created.</param>
        /// <param name="url">The URL of the stream.</param>
        /// <param name="timestamps">The start and stop timestamps of the activity.</param>
        /// <param name="details">The user-readable details of the activity.</param>
        /// <param name="state">The user-readable status of the activity.</param>
        /// <param name="emoji">The emoji used for a custom status.</param>
        /// <param name="party">The party.</param>
        /// <param name="assets">The activity assets.</param>
        /// <param name="secrets">The activity secrets.</param>
        /// <param name="instance">Whether the activity is in an instance.</param>
        /// <param name="flags">The payload flags.</param>
        public Activity
        (
            string name,
            ActivityType type,
            Optional<DateTimeOffset> createdAt,
            Optional<Uri?> url = default,
            Optional<IActivityTimestamps> timestamps = default,
            Optional<string?> details = default,
            Optional<string?> state = default,
            Optional<IActivityEmoji?> emoji = default,
            Optional<IActivityParty> party = default,
            Optional<IActivityAssets> assets = default,
            Optional<IActivitySecrets> secrets = default,
            Optional<bool> instance = default,
            Optional<ActivityFlags> flags = default
        )
        {
            this.Name = name;
            this.Type = type;
            this.Url = url;
            this.CreatedAt = createdAt;
            this.Timestamps = timestamps;
            this.Details = details;
            this.State = state;
            this.Emoji = emoji;
            this.Party = party;
            this.Assets = assets;
            this.Secrets = secrets;
            this.Instance = instance;
            this.Flags = flags;
        }
    }
}
