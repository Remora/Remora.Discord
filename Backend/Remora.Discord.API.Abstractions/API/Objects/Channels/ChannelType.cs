//
//  ChannelType.cs
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

namespace Remora.Discord.API.Abstractions
{
    /// <summary>
    /// Enumerates various channel types.
    /// </summary>
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
        GuildNews = 5,

        /// <summary>
        /// A channel in which game developers can sell their game on Discord.
        /// </summary>
        GuildStore = 6
    }
}
