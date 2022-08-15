//
//  AuditLogEvent.cs
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
/// Enumerates various possible audit log events.
/// </summary>
[PublicAPI]
public enum AuditLogEvent
{
    /// <summary>
    /// The guild information was updated.
    /// </summary>
    GuildUpdate = 1,

    /// <summary>
    /// A channel was created.
    /// </summary>
    ChannelCreate = 10,

    /// <summary>
    /// A channel was edited.
    /// </summary>
    ChannelUpdate = 11,

    /// <summary>
    /// A channel was deleted.
    /// </summary>
    ChannelDelete = 12,

    /// <summary>
    /// A channel permission overwrite was created.
    /// </summary>
    ChannelOverwriteCreate = 13,

    /// <summary>
    /// A channel permission overwrite was edited.
    /// </summary>
    ChannelOverwriteUpdate = 14,

    /// <summary>
    /// A channel permission overwrite was deleted.
    /// </summary>
    ChannelOverwriteDelete = 15,

    /// <summary>
    /// A member was kicked.
    /// </summary>
    MemberKick = 20,

    /// <summary>
    /// The member list was pruned.
    /// </summary>
    MemberPrune = 21,

    /// <summary>
    /// A member was banned.
    /// </summary>
    MemberBanAdd = 22,

    /// <summary>
    /// A member was unbanned.
    /// </summary>
    MemberBanRemove = 23,

    /// <summary>
    /// A member was edited.
    /// </summary>
    MemberUpdate = 24,

    /// <summary>
    /// A member's roles were updated.
    /// </summary>
    MemberRoleUpdate = 25,

    /// <summary>
    /// A member moved.
    /// </summary>
    MemberMove = 26,

    /// <summary>
    /// A member disconnected.
    /// </summary>
    MemberDisconnect = 27,

    /// <summary>
    /// A bot was added to the guild.
    /// </summary>
    BotAdd = 28,

    /// <summary>
    /// A role was created.
    /// </summary>
    RoleCreate = 30,

    /// <summary>
    /// A role was edited.
    /// </summary>
    RoleUpdate = 31,

    /// <summary>
    /// A role was deleted.
    /// </summary>
    RoleDelete = 32,

    /// <summary>
    /// An invite code was created.
    /// </summary>
    InviteCreate = 40,

    /// <summary>
    /// An invite code was edited.
    /// </summary>
    InviteUpdate = 41,

    /// <summary>
    /// An invite code was deleted.
    /// </summary>
    InviteDelete = 42,

    /// <summary>
    /// A webhook was created.
    /// </summary>
    WebhookCreate = 50,

    /// <summary>
    /// A webhook was edited.
    /// </summary>
    WebhookUpdate = 51,

    /// <summary>
    /// A webhook was deleted.
    /// </summary>
    WebhookDelete = 52,

    /// <summary>
    /// An emoji was created.
    /// </summary>
    EmojiCreate = 60,

    /// <summary>
    /// An emoji was edited.
    /// </summary>
    EmojiUpdate = 61,

    /// <summary>
    /// An emoji was deleted.
    /// </summary>
    EmojiDelete = 62,

    /// <summary>
    /// A message was deleted.
    /// </summary>
    MessageDelete = 72,

    /// <summary>
    /// A number of messages were bulk deleted.
    /// </summary>
    MessageBulkDelete = 73,

    /// <summary>
    /// A message was pinned.
    /// </summary>
    MessagePin = 74,

    /// <summary>
    /// A message was unpinned.
    /// </summary>
    MessageUnpin = 75,

    /// <summary>
    /// An integration was created.
    /// </summary>
    IntegrationCreate = 80,

    /// <summary>
    /// An integration was edited.
    /// </summary>
    IntegrationUpdate = 81,

    /// <summary>
    /// An integration was deleted.
    /// </summary>
    IntegrationDelete = 82,

    /// <summary>
    /// A stage instance was created.
    /// </summary>
    StageInstanceCreate = 83,

    /// <summary>
    /// A stage instance was updated.
    /// </summary>
    StageInstanceUpdate = 84,

    /// <summary>
    /// A stage instance was deleted.
    /// </summary>
    StageInstanceDelete = 85,

    /// <summary>
    /// A sticker was created.
    /// </summary>
    StickerCreate = 90,

    /// <summary>
    /// A sticker was updated.
    /// </summary>
    StickerUpdate = 91,

    /// <summary>
    /// A sticker was deleted.
    /// </summary>
    StickerDelete = 92,

    /// <summary>
    /// A scheduled guild event was created.
    /// </summary>
    GuildScheduledEventCreate = 100,

    /// <summary>
    /// A scheduled guild event was updated.
    /// </summary>
    GuildScheduledEventUpdate = 101,

    /// <summary>
    /// A scheduled guild event was deleted.
    /// </summary>
    GuildScheduledEventDelete = 102,

    /// <summary>
    /// A thread was created.
    /// </summary>
    ThreadCreate = 110,

    /// <summary>
    /// A thread was updated.
    /// </summary>
    ThreadUpdate = 111,

    /// <summary>
    /// A thread was deleted.
    /// </summary>
    ThreadDelete = 112,

    /// <summary>
    /// The permissions for an application command were updated.
    /// </summary>
    ApplicationCommandPermissionUpdate = 121
}
