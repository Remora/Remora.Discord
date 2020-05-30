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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Objects
{
    /// <summary>
    /// Represents information about an activity Discord is aware of.
    /// </summary>
    public sealed class Activity
    {
        /// <summary>
        /// Gets the name of the activity.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the activity.
        /// </summary>
        public ActivityType Type { get; }

        /// <summary>
        /// Gets the stream URL. Used when <see cref="Type"/> is <see cref="ActivityType.Streaming"/>.
        /// </summary>
        public Optional<Uri?> Url { get; }

        /// <summary>
        /// Gets the time when the activity was added to the user's session.
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public Optional<DateTimeOffset> CreatedAt { get; }

        /// <summary>
        /// Gets the time at which the activity started and/or ended.
        /// </summary>
        public Optional<ActivityTimestamps> Timestamps { get; }

        /// <summary>
        /// Gets a detail string about what the user is currently doing.
        /// </summary>
        public Optional<string?> Details { get; }

        /// <summary>
        /// Gets the user's current party status.
        /// </summary>
        public Optional<string?> State { get; }

        /// <summary>
        /// Gets the emoji used for a custom status.
        /// </summary>
        public Optional<ActivityEmoji?> Emoji { get; }

        /// <summary>
        /// Gets information about the user's current party.
        /// </summary>
        public Optional<ActivityParty> Party { get; }

        /// <summary>
        /// Gets information about the assets associated with the user's current activity.
        /// </summary>
        public Optional<ActivityAssets> Assets { get; }

        /// <summary>
        /// Gets the secrets associated with the user's current activity.
        /// </summary>
        public Optional<ActivitySecrets> Secrets { get; }

        /// <summary>
        /// Gets a value indicating whether the activity is in an instanced session.
        /// </summary>
        public Optional<bool> Instance { get; }

        /// <summary>
        /// Gets a set of descriptive flags that detail what the payload includes.
        /// </summary>
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
            Optional<ActivityTimestamps> timestamps = default,
            Optional<string?> details = default,
            Optional<string?> state = default,
            Optional<ActivityEmoji?> emoji = default,
            Optional<ActivityParty> party = default,
            Optional<ActivityAssets> assets = default,
            Optional<ActivitySecrets> secrets = default,
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
