//
//  DiscordPermission.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Permissions
{
    /// <summary>
    /// Enumerates the various permissions in Discord.
    /// </summary>
    [PublicAPI, Flags]
    public enum DiscordPermission
    {
        /// <summary>
        /// Allows creation of instant invites.
        /// </summary>
        CreateInstantInvite = 1 << 0,

        /// <summary>
        /// Allows kicking members.
        /// </summary>
        KickMembers = 1 << 1,

        /// <summary>
        /// Allows banning members.
        /// </summary>
        BanMembers = 1 << 2,

        /// <summary>
        /// Allows all permissions and bypasses channel permission overwrites.
        /// </summary>
        Administrator = 1 << 3,

        /// <summary>
        /// Allows management and editing of channels.
        /// </summary>
        ManageChannels = 1 << 4,

        /// <summary>
        /// Allows management and editing of the guild.
        /// </summary>
        ManageGuild = 1 << 5,

        /// <summary>
        /// Allows for the addition of reactions to messages.
        /// </summary>
        AddReactions = 1 << 6,

        /// <summary>
        /// Allows for viewing of audit logs.
        /// </summary>
        ViewAuditLog = 1 << 7,

        /// <summary>
        /// Allows for using priority speaker in a voice channel.
        /// </summary>
        PrioritySpeaker = 1 << 8,

        /// <summary>
        /// Allows the user to go live.
        /// </summary>
        Stream = 1 << 9,

        /// <summary>
        /// Allows guild members to view a channel, which includes reading messages in text channels. This may be
        /// referred to as "Read Messages" in the Discord client.
        /// </summary>
        ViewChannel = 1 << 10,

        /// <summary>
        /// Allows for sending messages in a channel.
        /// </summary>
        SendMessages = 1 << 11,

        /// <summary>
        /// Allows for sending of /tts messages.
        /// </summary>
        SendTTSMessages = 1 << 12,

        /// <summary>
        /// Allows for deletion of other user's messages.
        /// </summary>
        ManageMessages = 1 << 13,

        /// <summary>
        /// Links sent by users with this permission will be auto-embedded.
        /// </summary>
        EmbedLinks = 1 << 14,

        /// <summary>
        /// Allows for uploading images and files.
        /// </summary>
        AttachFiles = 1 << 15,

        /// <summary>
        /// Allows for reading of message history.
        /// </summary>
        ReadMessageHistory = 1 << 16,

        /// <summary>
        /// Allows for using the <code>@everyone</code> tag to notify all users in a guild, and the <code>@here</code>
        /// tag to notify all online users in a channel.
        /// </summary>
        MentionEveryone = 1 << 17,

        /// <summary>
        /// Allows the usage of custom emojis from other servers.
        /// </summary>
        UseExternalEmojis = 1 << 18,

        /// <summary>
        /// Allows for viewing guild insights.
        /// </summary>
        ViewGuildInsights = 1 << 19,

        /// <summary>
        /// Allows for joining of a voice channel.
        /// </summary>
        Connect = 1 << 20,

        /// <summary>
        /// Allows for speaking in a voice channel.
        /// </summary>
        Speak = 1 << 21,

        /// <summary>
        /// Allows for muting members in a voice channel.
        /// </summary>
        MuteMembers = 1 << 22,

        /// <summary>
        /// Allows for deafening of members in a voice channel.
        /// </summary>
        DeafenMembers = 1 << 23,

        /// <summary>
        /// Allows for moving of members between voice channels.
        /// </summary>
        MoveMembers = 1 << 24,

        /// <summary>
        /// Allows for using voice activity detection in a voice channel.
        /// </summary>
        UseVoiceActivity = 1 << 25,

        /// <summary>
        /// Allows for modification of own nickname.
        /// </summary>
        ChangeNickname = 1 << 26,

        /// <summary>
        /// Allows for modification of other user's nicknames.
        /// </summary>
        ManageNicknames = 1 << 27,

        /// <summary>
        /// Allows management and editing of roles. This may be displayed as "Manage Permissions" in the Discord client.
        /// </summary>
        ManageRoles = 1 << 28,

        /// <summary>
        /// Allows management and editing of webhooks.
        /// </summary>
        ManageWebhooks = 1 << 29,

        /// <summary>
        /// Allows management and editing of emojis.
        /// </summary>
        ManageEmojis = 1 << 30
    }
}
