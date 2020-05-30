//
//  UpdateStatus.cs
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
using Newtonsoft.Json.Serialization;
using Remora.Discord.Gateway.API.Objects;

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents a command to update the status of a user.
    /// </summary>
    public sealed class UpdateStatus
    {
        /// <summary>
        /// Gets the unix time in milliseconds of when the client went idle, or null if the client is not idle.
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset? Since { get; }

        /// <summary>
        /// Gets the user's new activity.
        /// </summary>
        public Activity? Game { get; }

        /// <summary>
        /// Gets the user's status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
        public UserStatus Status { get; }

        /// <summary>
        /// Gets a value indicating whether the user is AFK.
        /// </summary>
        public bool Afk { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStatus"/> class.
        /// </summary>
        /// <param name="status">The user's status.</param>
        /// <param name="afk">Whether the user is AFK.</param>
        /// <param name="since">The time that the client went idle.</param>
        /// <param name="game">The activity that the user is performing.</param>
        public UpdateStatus
        (
            UserStatus status,
            bool afk,
            DateTimeOffset? since = null,
            Activity? game = null
        )
        {
            this.Since = since;
            this.Game = game;
            this.Status = status;
            this.Afk = afk;
        }
    }
}
