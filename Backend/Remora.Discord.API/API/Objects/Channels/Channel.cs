//
//  Channel.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Channel : IChannel
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public ChannelType Type { get; }

        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc />
        public Optional<int> Position { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IPermissionOverwrite>> PermissionOverwrites { get; }

        /// <inheritdoc />
        public Optional<string> Name { get; }

        /// <inheritdoc />
        public Optional<string?> Topic { get; }

        /// <inheritdoc />
        public Optional<bool> IsNsfw { get; }

        /// <inheritdoc />
        public Optional<Snowflake?> LastMessageID { get; }

        /// <inheritdoc />
        public Optional<int> Bitrate { get; }

        /// <inheritdoc />
        public Optional<int> UserLimit { get; }

        /// <inheritdoc />
        public Optional<int> RateLimitPerUser { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IUser>> Recipients { get; }

        /// <inheritdoc />
        public Optional<IImageHash?> Icon { get; }

        /// <inheritdoc />
        public Optional<Snowflake> OwnerID { get; }

        /// <inheritdoc />
        public Optional<Snowflake> ApplicationID { get; }

        /// <inheritdoc />
        public Optional<Snowflake?> ParentID { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset?> LastPinTimestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel"/> class.
        /// </summary>
        /// <param name="id">The ID of the channel.</param>
        /// <param name="type">The channel type.</param>
        /// <param name="guildID">The ID of the guild the channel belongs to.</param>
        /// <param name="position">The sorting position of the channel.</param>
        /// <param name="permissionOverwrites">The permission overwrites of the channel.</param>
        /// <param name="name">The name of the channel.</param>
        /// <param name="topic">The topic of the channel.</param>
        /// <param name="isNsfw">Whether the channel is NSFW.</param>
        /// <param name="lastMessageID">The ID of the last message in the channel.</param>
        /// <param name="bitrate">The bitrate of the channel.</param>
        /// <param name="userLimit">The user limit of the channel.</param>
        /// <param name="rateLimitPerUser">The per-user message rate limit.</param>
        /// <param name="recipients">The recipients of the channel.</param>
        /// <param name="icon">The icon of the channel.</param>
        /// <param name="ownerID">The ID of the channel's owner.</param>
        /// <param name="applicationID">The ID of the application that manages the channel.</param>
        /// <param name="parentID">The ID of the parent category.</param>
        /// <param name="lastPinTimestamp">The time when the last message was pinned.</param>
        public Channel
        (
            Snowflake id,
            ChannelType type,
            Optional<Snowflake> guildID,
            Optional<int> position,
            Optional<IReadOnlyList<IPermissionOverwrite>> permissionOverwrites,
            Optional<string> name,
            Optional<string?> topic,
            Optional<bool> isNsfw,
            Optional<Snowflake?> lastMessageID,
            Optional<int> bitrate,
            Optional<int> userLimit,
            Optional<int> rateLimitPerUser,
            Optional<IReadOnlyList<IUser>> recipients,
            Optional<IImageHash?> icon,
            Optional<Snowflake> ownerID,
            Optional<Snowflake> applicationID,
            Optional<Snowflake?> parentID,
            Optional<DateTimeOffset?> lastPinTimestamp
        )
        {
            this.ID = id;
            this.Type = type;
            this.GuildID = guildID;
            this.Position = position;
            this.PermissionOverwrites = permissionOverwrites;
            this.Name = name;
            this.Topic = topic;
            this.IsNsfw = isNsfw;
            this.LastMessageID = lastMessageID;
            this.Bitrate = bitrate;
            this.UserLimit = userLimit;
            this.RateLimitPerUser = rateLimitPerUser;
            this.Recipients = recipients;
            this.Icon = icon;
            this.OwnerID = ownerID;
            this.ApplicationID = applicationID;
            this.ParentID = parentID;
            this.LastPinTimestamp = lastPinTimestamp;
        }
    }
}
