//
//  ChannelType.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various channel types.
/// </summary>
[PublicAPI]
public enum ChannelType
{
    /// <summary>
    /// A text channel within a server.
    /// </summary>
    GuildText = 0,

    /// <summary>
    /// A direct message between two users.
    /// </summary>
    DM = 1,

    /// <summary>
    /// A voice channel within a server.
    /// </summary>
    GuildVoice = 2,

    /// <summary>
    /// A direct message between three or more users.
    /// </summary>
    GroupDM = 3,

    /// <summary>
    /// An organizational category that contains up to 50 channels.
    /// </summary>
    GuildCategory = 4,

    /// <summary>
    /// A channel that users can follow and crosspost into their own servers.
    /// </summary>
    GuildAnnouncement = 5,

    /// <summary>
    /// A temporary sub-channel within a <see cref="GuildAnnouncement"/> channel.
    /// </summary>
    AnnouncementThread = 10,

    /// <summary>
    /// A temporary sub-channel within a <see cref="GuildText"/> channel.
    /// </summary>
    PublicThread = 11,

    /// <summary>
    /// A temporary sub-channel within a <see cref="GuildText"/> channel that is only viewable by those invited, and
    /// those with the <see cref="DiscordTextPermission.ManageThreads"/> permission.
    /// </summary>
    PrivateThread = 12,

    /// <summary>
    /// A voice channel for hosting events with an audience.
    /// </summary>
    GuildStageVoice = 13,

    /// <summary>
    /// A channel in a student hub that contains a list of other guilds.
    /// </summary>
    GuildDirectory = 14
}
