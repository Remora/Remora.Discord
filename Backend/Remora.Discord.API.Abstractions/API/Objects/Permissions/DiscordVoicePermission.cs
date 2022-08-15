//
//  DiscordVoicePermission.cs
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
/// Enumerates a subset of the full <see cref="DiscordPermission"/> enumeration, containing only the permissions
/// applicable to voice channels.
/// </summary>
[PublicAPI]
public enum DiscordVoicePermission
{
    /// <inheritdoc cref="DiscordPermission.CreateInstantInvite"/>
    CreateInstantInvite = DiscordPermission.CreateInstantInvite,

    /// <inheritdoc cref="DiscordPermission.ManageChannels"/>
    ManageChannels = DiscordPermission.ManageChannels,

    /// <inheritdoc cref="DiscordPermission.AddReactions"/>
    AddReactions = DiscordPermission.AddReactions,

    /// <inheritdoc cref="DiscordPermission.ViewChannel"/>
    ViewChannel = DiscordPermission.ViewChannel,

    /// <inheritdoc cref="DiscordPermission.SendMessages"/>
    SendMessages = DiscordPermission.SendMessages,

    /// <inheritdoc cref="DiscordPermission.SendTTSMessages"/>
    SendTTSMessages = DiscordPermission.SendTTSMessages,

    /// <inheritdoc cref="DiscordPermission.ManageMessages"/>
    ManageMessages = DiscordPermission.ManageMessages,

    /// <inheritdoc cref="DiscordPermission.EmbedLinks"/>
    EmbedLinks = DiscordPermission.EmbedLinks,

    /// <inheritdoc cref="DiscordPermission.AttachFiles"/>
    AttachFiles = DiscordPermission.AttachFiles,

    /// <inheritdoc cref="DiscordPermission.ReadMessageHistory"/>
    ReadMessageHistory = DiscordPermission.ReadMessageHistory,

    /// <inheritdoc cref="DiscordPermission.MentionEveryone"/>
    MentionEveryone = DiscordPermission.MentionEveryone,

    /// <inheritdoc cref="DiscordPermission.UseExternalEmojis"/>
    UseExternalEmojis = DiscordPermission.UseExternalEmojis,

    /// <inheritdoc cref="DiscordPermission.Connect"/>
    Connect = DiscordPermission.Connect,

    /// <inheritdoc cref="DiscordPermission.Speak"/>
    Speak = DiscordPermission.Speak,

    /// <inheritdoc cref="DiscordPermission.MuteMembers"/>
    MuteMembers = DiscordPermission.MuteMembers,

    /// <inheritdoc cref="DiscordPermission.DeafenMembers"/>
    DeafenMembers = DiscordPermission.DeafenMembers,

    /// <inheritdoc cref="DiscordPermission.MoveMembers"/>
    MoveMembers = DiscordPermission.MoveMembers,

    /// <inheritdoc cref="DiscordPermission.UseVoiceActivity"/>
    UseVoiceActivity = DiscordPermission.UseVoiceActivity,

    /// <inheritdoc cref="DiscordPermission.ManageRoles"/>
    ManageRoles = DiscordPermission.ManageRoles,

    /// <inheritdoc cref="DiscordPermission.ManageWebhooks"/>
    ManageWebhooks = DiscordPermission.ManageWebhooks,

    /// <inheritdoc cref="DiscordPermission.UseApplicationCommands"/>
    UseApplicationCommands = DiscordPermission.UseApplicationCommands,

    /// <inheritdoc cref="DiscordPermission.RequestToSpeak"/>
    RequestToSpeak = DiscordPermission.RequestToSpeak,

    /// <inheritdoc cref="DiscordPermission.ManageEvents"/>
    ManageEvents = DiscordPermission.ManageEvents,

    /// <inheritdoc cref="DiscordPermission.UseExternalStickers"/>
    UseExternalStickers = DiscordPermission.UseExternalStickers,

    /// <inheritdoc cref="DiscordPermission.UseEmbeddedActivities"/>
    UseEmbeddedActivities = DiscordPermission.UseEmbeddedActivities
}
