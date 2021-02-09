//
//  DiscordVoicePermission.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Enumerates a subset of the full <see cref="DiscordPermission"/> enumeration, containing only the permissions
    /// applicable to voice channels.
    /// </summary>
    [PublicAPI]
    public enum DiscordVoicePermission
    {
        /// <summary>
        /// Allows creation of instant invites.
        /// </summary>
        CreateInstantInvite = 0,

        /// <summary>
        /// Allows management and editing of channels.
        /// </summary>
        ManageChannels = 4,

        /// <summary>
        /// Allows guild members to view a channel, which includes reading messages in text channels. This may be
        /// referred to as "Read Messages" in the Discord client.
        /// </summary>
        ViewChannel = 10,

        /// <summary>
        /// Allows for joining of a voice channel.
        /// </summary>
        Connect = 20,

        /// <summary>
        /// Allows for speaking in a voice channel.
        /// </summary>
        Speak = 21,

        /// <summary>
        /// Allows for muting members in a voice channel.
        /// </summary>
        MuteMembers = 22,

        /// <summary>
        /// Allows for deafening of members in a voice channel.
        /// </summary>
        DeafenMembers = 23,

        /// <summary>
        /// Allows for moving of members between voice channels.
        /// </summary>
        MoveMembers = 24,

        /// <summary>
        /// Allows for using voice activity detection in a voice channel.
        /// </summary>
        UseVoiceActivity = 25,

        /// <summary>
        /// Allows management and editing of roles. This may be displayed as "Manage Permissions" in the Discord client.
        /// </summary>
        ManageRoles = 28,

        /// <summary>
        /// Allows management and editing of webhooks.
        /// </summary>
        ManageWebhooks = 29
    }
}
