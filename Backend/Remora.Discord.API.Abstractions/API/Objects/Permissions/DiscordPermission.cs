//
//  DiscordPermission.cs
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

using System.Numerics;
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates the various permissions in Discord.
/// </summary>
/// <remarks>
/// Note that the values of the enumeration members do not map to the actual values of Discord's permissions - instead,
/// they mark how many bits a value of 1 should be shifted into a <see cref="BigInteger"/> to get the actual value.
/// </remarks>
[PublicAPI]
public enum DiscordPermission
{
    /// <summary>
    /// Allows creation of instant invites.
    /// </summary>
    CreateInstantInvite = 0,

    /// <summary>
    /// Allows kicking members.
    /// </summary>
    KickMembers = 1,

    /// <summary>
    /// Allows banning members.
    /// </summary>
    BanMembers = 2,

    /// <summary>
    /// Allows all permissions and bypasses channel permission overwrites.
    /// </summary>
    Administrator = 3,

    /// <summary>
    /// Allows management and editing of channels.
    /// </summary>
    ManageChannels = 4,

    /// <summary>
    /// Allows management and editing of the guild.
    /// </summary>
    ManageGuild = 5,

    /// <summary>
    /// Allows for the addition of reactions to messages.
    /// </summary>
    AddReactions = 6,

    /// <summary>
    /// Allows for viewing of audit logs.
    /// </summary>
    ViewAuditLog = 7,

    /// <summary>
    /// Allows for using priority speaker in a voice channel.
    /// </summary>
    PrioritySpeaker = 8,

    /// <summary>
    /// Allows the user to go live.
    /// </summary>
    Stream = 9,

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
    /// Allows for using the @everyone tag to notify all users in a guild, and the @here tag to notify all online
    /// users in a channel.
    /// </summary>
    MentionEveryone = 17,

    /// <summary>
    /// Allows the usage of custom emojis from other servers.
    /// </summary>
    UseExternalEmojis = 18,

    /// <summary>
    /// Allows for viewing guild insights.
    /// </summary>
    ViewGuildInsights = 19,

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
    /// Allows for modification of own nickname.
    /// </summary>
    ChangeNickname = 26,

    /// <summary>
    /// Allows for modification of other user's nicknames.
    /// </summary>
    ManageNicknames = 27,

    /// <summary>
    /// Allows management and editing of roles. This may be displayed as "Manage Permissions" in the Discord client.
    /// </summary>
    ManageRoles = 28,

    /// <summary>
    /// Allows management and editing of webhooks.
    /// </summary>
    ManageWebhooks = 29,

    /// <summary>
    /// Allows management and editing of emojis and stickers.
    /// </summary>
    ManageEmojisAndStickers = 30,

    /// <summary>
    /// Allows usage of slash commands.
    /// </summary>
    UseApplicationCommands = 31,

    /// <summary>
    /// Allows the user to request to speak in a stage channel.
    /// </summary>
    RequestToSpeak = 32,

    /// <summary>
    /// Allows the user to manage scheduled events.
    /// </summary>
    ManageEvents = 33,

    /// <summary>
    /// Allows the user to manage threads.
    /// </summary>
    ManageThreads = 34,

    /// <summary>
    /// Allows the user to create public threads.
    /// </summary>
    CreatePublicThreads = 35,

    /// <summary>
    /// Allows the user to create private threads.
    /// </summary>
    CreatePrivateThreads = 36,

    /// <summary>
    /// Allows the user to use stickers from other servers.
    /// </summary>
    UseExternalStickers = 37,

    /// <summary>
    /// Allows the user to send messages in threads.
    /// </summary>
    SendMessagesInThreads = 38,

    /// <summary>
    /// Allows for launching activities in a voice channel.
    /// </summary>
    UseEmbeddedActivities = 39,

    /// <summary>
    /// Allows for timing out users to prevent them from sending or reacting to messages in chat and threads, and from
    /// speaking in voice and stage channels.
    /// </summary>
    ModerateMembers = 40
}
