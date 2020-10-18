//
//  ChannelTemplate.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class ChannelTemplate : IChannelTemplate
    {
        /// <inheritdoc />
        public int ID { get; }

        /// <inheritdoc />
        public ChannelType Type { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public int Position { get; }

        /// <inheritdoc />
        public string? Topic { get; }

        /// <inheritdoc />
        public int Bitrate { get; }

        /// <inheritdoc />
        public int UserLimit { get; }

        /// <inheritdoc />
        public bool IsNsfw { get; }

        /// <inheritdoc />
        public int RateLimitPerUser { get; }

        /// <inheritdoc />
        public int? ParentID { get; }

        /// <inheritdoc />
        public IReadOnlyList<IPermissionOverwriteTemplate> PermissionOverwrites { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelTemplate"/> class.
        /// </summary>
        /// <param name="id">The relative ID of the channel.</param>
        /// <param name="type">The channel type.</param>
        /// <param name="name">The name of the channel.</param>
        /// <param name="position">The sorting position of the channel.</param>
        /// <param name="topic">The channel topic.</param>
        /// <param name="bitrate">The channel bitrate, if it is a voice channel.</param>
        /// <param name="userLimit">The user limit of the channel, if it is a voice channel.</param>
        /// <param name="isNsfw">Whether the channel is NSFW.</param>
        /// <param name="rateLimitPerUser">The per-user rate limit.</param>
        /// <param name="parentID">The ID of the parent category, if any.</param>
        /// <param name="permissionOverwrites">The permission overwrites of the channel, if any.</param>
        public ChannelTemplate
        (
            int id,
            ChannelType type,
            string name,
            int position,
            string? topic,
            int bitrate,
            int userLimit,
            bool isNsfw,
            int rateLimitPerUser,
            int? parentID,
            IReadOnlyList<IPermissionOverwriteTemplate> permissionOverwrites
        )
        {
            this.ID = id;
            this.Type = type;
            this.Name = name;
            this.Position = position;
            this.Topic = topic;
            this.Bitrate = bitrate;
            this.UserLimit = userLimit;
            this.IsNsfw = isNsfw;
            this.RateLimitPerUser = rateLimitPerUser;
            this.ParentID = parentID;
            this.PermissionOverwrites = permissionOverwrites;
        }
    }
}
