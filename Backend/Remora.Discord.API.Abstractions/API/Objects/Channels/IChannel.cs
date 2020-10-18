//
//  IChannel.cs
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
using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a channel.
    /// </summary>
    [PublicAPI]
    public interface IChannel
    {
        /// <summary>
        /// Gets the ID of the channel.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the type of the channel.
        /// </summary>
        ChannelType Type { get; }

        /// <summary>
        /// Gets the ID of the guild the channel is in.
        /// </summary>
        Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Gets the sorting position of the channel.
        /// </summary>
        Optional<int> Position { get; }

        /// <summary>
        /// Gets a list of explicit permission overwrites for members and roles.
        /// </summary>
        Optional<IReadOnlyList<IPermissionOverwrite>> PermissionOverwrites { get; }

        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        Optional<string> Name { get; }

        /// <summary>
        /// Gets the topic of the channel.
        /// </summary>
        Optional<string?> Topic { get; }

        /// <summary>
        /// Gets a value indicating whether the channel is NSFW.
        /// </summary>
        Optional<bool> IsNsfw { get; }

        /// <summary>
        /// Gets the ID of the last message sent in the channel.
        /// </summary>
        Optional<Snowflake?> LastMessageID { get; }

        /// <summary>
        /// Gets the bitrate (in bits) of the channel.
        /// </summary>
        Optional<int> Bitrate { get; }

        /// <summary>
        /// Gets the user limit of the voice channel.
        /// </summary>
        Optional<int> UserLimit { get; }

        /// <summary>
        /// Gets the number of seconds a user has to wait before sending another message (0-21600); bots, as well as
        /// users with the permission <see cref="DiscordPermission.ManageMessages"/> or
        /// <see cref="DiscordPermission.ManageChannels"/> are unaffected. This is colloquially known as "slow mode".
        /// </summary>
        Optional<int> RateLimitPerUser { get; }

        /// <summary>
        /// Gets the recipients of the DM.
        /// </summary>
        Optional<IReadOnlyList<IUser>> Recipients { get; }

        /// <summary>
        /// Gets the icon of the channel.
        /// </summary>
        Optional<IImageHash?> Icon { get; }

        /// <summary>
        /// Gets the ID of the DM creator.
        /// </summary>
        Optional<Snowflake> OwnerID { get; }

        /// <summary>
        /// Gets the application ID of the group DM creator, if it is bot-created.
        /// </summary>
        Optional<Snowflake> ApplicationID { get; }

        /// <summary>
        /// Gets the ID of the parent category for a channel. Each category can contain up to 50 channels.
        /// </summary>
        Optional<Snowflake?> ParentID { get; }

        /// <summary>
        /// Gets the time when the last pinned message was pinned.
        /// </summary>
        Optional<DateTimeOffset?> LastPinTimestamp { get; }
    }
}
