//
//  GatewayIntents.cs
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

using System;
using JetBrains.Annotations;

#pragma warning disable SA1629

namespace Remora.Discord.API.Abstractions.Gateway.Commands;

/// <summary>
/// Enumerates the gateway intents that can be specified.
/// </summary>
[PublicAPI, Flags]
public enum GatewayIntents
{
    /// <summary>
    /// Subscribes to the following events:
    ///     - GUILD_CREATE
    ///     - GUILD_UPDATE
    ///     - GUILD_DELETE
    ///     - GUILD_ROLE_CREATE
    ///     - GUILD_ROLE_UPDATE
    ///     - GUILD_ROLE_DELETE
    ///     - CHANNEL_CREATE
    ///     - CHANNEL_UPDATE
    ///     - CHANNEL_DELETE
    ///     - CHANNEL_PINS_UPDATE
    ///     - THREAD_CREATE
    ///     - THREAD_UPDATE
    ///     - THREAD_DELETE
    ///     - THREAD_LIST_SYNC
    ///     - THREAD_MEMBER_UPDATE
    ///     - THREAD_MEMBERS_UPDATE
    ///     - STAGE_INSTANCE_UPDATE
    ///     - STAGE_INSTANCE_DELETE
    /// </summary>
    Guilds = 1 << 0,

    /// <summary>
    /// Subscribes to the following events:
    ///     - GUILD_MEMBER_ADD
    ///     - GUILD_MEMBER_UPDATE
    ///     - GUILD_MEMBER_REMOVE
    ///     - THREAD_MEMBERS_UPDATE
    /// </summary>
    GuildMembers = 1 << 1,

    /// <summary>
    /// Subscribes to the following events:
    ///     - GUILD_BAN_ADD
    ///     - GUILD_BAN_REMOVE
    /// </summary>
    GuildBans = 1 << 2,

    /// <summary>
    /// Subscribes to the following events:
    ///     - GUILD_EMOJIS_UPDATE
    ///     - GUILD_STICKERS_UPDATE
    /// </summary>
    GuildEmojisAndStickers = 1 << 3,

    /// <summary>
    /// Subscribes to the following events:
    ///     - GUILD_INTEGRATIONS_UPDATE
    ///     - INTEGRATION_CREATE
    ///     - INTEGRATION_UPDATE
    ///     - INTEGRATION_DELETE
    /// </summary>
    GuildIntegrations = 1 << 4,

    /// <summary>
    /// Subscribes to the following events:
    ///     - WEBHOOKS_UPDATE
    /// </summary>
    GuildWebhooks = 1 << 5,

    /// <summary>
    /// Subscribes to the following events:
    ///     - INVITE_CREATE
    ///     - INVITE_DELETE
    /// </summary>
    GuildInvites = 1 << 6,

    /// <summary>
    /// Subscribes to the following events:
    ///     - VOICE_STATE_UPDATE
    /// </summary>
    GuildVoiceStates = 1 << 7,

    /// <summary>
    /// Subscribes to the following events:
    ///     - PRESENCE_UPDATE
    /// </summary>
    GuildPresences = 1 << 8,

    /// <summary>
    /// Subscribes to the following events:
    ///     - MESSAGE_CREATE
    ///     - MESSAGE_UPDATE
    ///     - MESSAGE_DELETE
    ///     - MESSAGE_DELETE_BULK
    /// </summary>
    GuildMessages = 1 << 9,

    /// <summary>
    /// Subscribes to the following events:
    ///     - MESSAGE_REACTION_ADD
    ///     - MESSAGE_REACTION_REMOVE
    ///     - MESSAGE_REACTION_REMOVE_ALL
    ///     - MESSAGE_REACTION_REMOVE_EMOJI
    /// </summary>
    GuildMessageReactions = 1 << 10,

    /// <summary>
    /// Subscribes to the following events:
    ///     - TYPING_START
    /// </summary>
    GuildMessageTyping = 1 << 11,

    /// <summary>
    /// Subscribes to the following events:
    ///     - MESSAGE_CREATE
    ///     - MESSAGE_UPDATE
    ///     - MESSAGE_DELETE
    ///     - CHANNEL_PINS_UPDATE
    /// </summary>
    DirectMessages = 1 << 12,

    /// <summary>
    /// Subscribes to the following events:
    ///     - MESSAGE_REACTION_ADD
    ///     - MESSAGE_REACTION_REMOVE
    ///     - MESSAGE_REACTION_REMOVE_ALL
    ///     - MESSAGE_REACTION_REMOVE_EMOJI
    /// </summary>
    DirectMessageReactions = 1 << 13,

    /// <summary>
    /// Subscribes to the following events:
    ///     - TYPING_START
    /// </summary>
    DirectMessageTyping = 1 << 14,

    /// <summary>
    /// Does not subscribe to any particular events, but requests that message contents be sent along with the following
    /// events:
    ///     - MESSAGE_CREATE
    ///     - MESSAGE_UPDATE
    ///     - MESSAGE_DELETE
    ///
    /// If this intent is not specified, all raw message content strings will be empty. You may still receive user input
    /// using interactions, such as slash commands or modals.
    /// </summary>
    MessageContents = 1 << 15,

    /// <summary>
    /// Subscribes to the following events:
    ///     - GUILD_SCHEDULED_EVENT_CREATE
    ///     - GUILD_SCHEDULED_EVENT_UPDATE
    ///     - GUILD_SCHEDULED_EVENT_DELETE
    ///     - GUILD_SCHEDULED_EVENT_USER_ADD **
    ///     - GUILD_SCHEDULED_EVENT_USER_REMOVE **
    /// </summary>
    /// <remarks>
    /// GUILD_SCHEDULED_EVENT_USER_ADD and GUILD_SCHEDULED_EVENT_USER_REMOVE are currently experimental and not
    /// officially supported.
    /// </remarks>
    GuildScheduledEvents = 1 << 16
}
