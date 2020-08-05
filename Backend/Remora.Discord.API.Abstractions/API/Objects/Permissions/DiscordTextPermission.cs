//
//  DiscordTextPermission.cs
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

namespace Remora.Discord.API.Abstractions.Permissions
{
    /// <summary>
    /// Enumerates a subset of the full <see cref="DiscordPermission"/> enumeration, containing only the permissions
    /// applicable to text channels.
    /// </summary>
    [PublicAPI]
    public enum DiscordTextPermission
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
        /// Allows for the addition of reactions to messages.
        /// </summary>
        AddReactions = 6,

        /// <summary>
        /// Allows guild members to view a channel, which includes reading messages in text channels. This may be
        /// referred to as "Read Messages" in the Discord client.
        /// </summary>
        ViewChannel = 10,

        /// <summary>
        /// Allows for sending messages in a channel.
        /// </summary>
        SendMessages = 11,

        /// <summary>
        /// Allows for sending of /tts messages.
        /// </summary>
        SendTTSMessages = 12,

        /// <summary>
        /// Allows for deletion of other user's messages.
        /// </summary>
        ManageMessages = 13,

        /// <summary>
        /// Links sent by users with this permission will be auto-embedded.
        /// </summary>
        EmbedLinks = 14,

        /// <summary>
        /// Allows for uploading images and files.
        /// </summary>
        AttachFiles = 15,

        /// <summary>
        /// Allows for reading of message history.
        /// </summary>
        ReadMessageHistory = 16,

        /// <summary>
        /// Allows for using the <code>@everyone</code> tag to notify all users in a guild, and the <code>@here</code>
        /// tag to notify all online users in a channel.
        /// </summary>
        MentionEveryone = 17,

        /// <summary>
        /// Allows the usage of custom emojis from other servers.
        /// </summary>
        UseExternalEmojis = 18,

        /// <summary>
        /// Allows management and editing of roles. This may be displayed as "Manage Permissions" in the Discord client.
        /// </summary>
        ManageRoles = 28,

        /// <summary>
        /// Allows management and editing of webhooks.
        /// </summary>
        ManageWebhooks = 29,
    }
}
