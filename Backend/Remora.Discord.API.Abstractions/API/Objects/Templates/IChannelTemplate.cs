//
//  IChannelTemplate.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a channel.
/// </summary>
[PublicAPI]
public interface IChannelTemplate
{
    /// <summary>
    /// Gets the relative ID of the channel.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Gets the type of the channel.
    /// </summary>
    ChannelType Type { get; }

    /// <summary>
    /// Gets the name of the channel.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the sorting position of the channel.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Gets the topic of the channel.
    /// </summary>
    string? Topic { get; }

    /// <summary>
    /// Gets the bitrate (in bits) of the channel.
    /// </summary>
    int Bitrate { get; }

    /// <summary>
    /// Gets the user limit of the voice channel.
    /// </summary>
    int UserLimit { get; }

    /// <summary>
    /// Gets a value indicating whether the channel is NSFW.
    /// </summary>
    bool IsNsfw { get; }

    /// <summary>
    /// Gets the number of seconds a user has to wait before sending another message (0-21600); bots, as well as
    /// users with the permission <see cref="DiscordPermission.ManageMessages"/> or
    /// <see cref="DiscordPermission.ManageChannels"/> are unaffected. This is colloquially known as "slow mode".
    /// </summary>
    int RateLimitPerUser { get; }

    /// <summary>
    /// Gets the relative ID of the parent category for a channel. Each category can contain up to 50 channels.
    /// </summary>
    int? ParentID { get; }

    /// <summary>
    /// Gets a list of explicit permission overwrites for members and roles.
    /// </summary>
    IReadOnlyList<IPermissionOverwriteTemplate> PermissionOverwrites { get; }
}
