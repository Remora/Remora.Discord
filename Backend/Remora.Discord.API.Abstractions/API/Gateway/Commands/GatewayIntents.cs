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
    /// <list type="bullet">
    /// <item>GUILD_CREATE</item>
    /// <item>GUILD_UPDATE</item>
    /// <item>GUILD_DELETE</item>
    /// <item>GUILD_ROLE_CREATE</item>
    /// <item>GUILD_ROLE_UPDATE</item>
    /// <item>GUILD_ROLE_DELETE</item>
    /// <item>CHANNEL_CREATE</item>
    /// <item>CHANNEL_UPDATE</item>
    /// <item>CHANNEL_DELETE</item>
    /// <item>CHANNEL_PINS_UPDATE</item>
    /// <item>THREAD_CREATE</item>
    /// <item>THREAD_UPDATE</item>
    /// <item>THREAD_DELETE</item>
    /// <item>THREAD_LIST_SYNC</item>
    /// <item>THREAD_MEMBER_UPDATE</item>
    /// <item>THREAD_MEMBERS_UPDATE</item>
    /// <item>STAGE_INSTANCE_CREATE</item>
    /// <item>STAGE_INSTANCE_UPDATE</item>
    /// <item>STAGE_INSTANCE_DELETE</item>
    /// <item>VOICE_CHANNEL_STATUS_UPDATE</item>
    /// <item>VOICE_CHANNEL_START_TIME_UPDATE</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// THREAD_MEMBERS_UPDATE contains different data depending on which intents are used.
    /// </remarks>
    Guilds = 1 << 0,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>GUILD_MEMBER_ADD</item>
    /// <item>GUILD_MEMBER_UPDATE</item>
    /// <item>GUILD_MEMBER_REMOVE</item>
    /// <item>THREAD_MEMBERS_UPDATE</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// THREAD_MEMBERS_UPDATE contains different data depending on which intents are used.
    /// </remarks>
    GuildMembers = 1 << 1,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>GUILD_AUDIT_LOG_ENTRY_CREATE</item>
    /// <item>GUILD_BAN_ADD</item>
    /// <item>GUILD_BAN_REMOVE</item>
    /// </list>
    /// </summary>
    GuildModeration = 1 << 2,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>GUILD_EMOJIS_UPDATE</item>
    /// <item>GUILD_STICKERS_UPDATE</item>
    /// <item>GUILD_SOUNDBOARD_SOUND_CREATE</item>
    /// <item>GUILD_SOUNDBOARD_SOUND_UPDATE</item>
    /// <item>GUILD_SOUNDBOARD_SOUND_DELETE</item>
    /// <item>GUILD_SOUNDBOARD_SOUNDS_UPDATE</item>
    /// </list>
    /// </summary>
    GuildExpressions = 1 << 3,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>GUILD_INTEGRATIONS_UPDATE</item>
    /// <item>INTEGRATION_CREATE</item>
    /// <item>INTEGRATION_UPDATE</item>
    /// <item>INTEGRATION_DELETE</item>
    /// </list>
    /// </summary>
    GuildIntegrations = 1 << 4,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>WEBHOOKS_UPDATE</item>
    /// </list>
    /// </summary>
    GuildWebhooks = 1 << 5,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>INVITE_CREATE</item>
    /// <item>INVITE_DELETE</item>
    /// </list>
    /// </summary>
    GuildInvites = 1 << 6,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>VOICE_CHANNEL_EFFECT_SEND</item>
    /// <item>VOICE_STATE_UPDATE</item>
    /// </list>
    /// </summary>
    GuildVoiceStates = 1 << 7,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>PRESENCE_UPDATE</item>
    /// </list>
    /// </summary>
    GuildPresences = 1 << 8,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>MESSAGE_CREATE</item>
    /// <item>MESSAGE_UPDATE</item>
    /// <item>MESSAGE_DELETE</item>
    /// <item>MESSAGE_DELETE_BULK</item>
    /// </list>
    /// </summary>
    GuildMessages = 1 << 9,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>MESSAGE_REACTION_ADD</item>
    /// <item>MESSAGE_REACTION_REMOVE</item>
    /// <item>MESSAGE_REACTION_REMOVE_ALL</item>
    /// <item>MESSAGE_REACTION_REMOVE_EMOJI</item>
    /// </list>
    /// </summary>
    GuildMessageReactions = 1 << 10,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>TYPING_START</item>
    /// </list>
    /// </summary>
    GuildMessageTyping = 1 << 11,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>MESSAGE_CREATE</item>
    /// <item>MESSAGE_UPDATE</item>
    /// <item>MESSAGE_DELETE</item>
    /// <item>CHANNEL_PINS_UPDATE</item>
    /// </list>
    /// </summary>
    DirectMessages = 1 << 12,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>MESSAGE_REACTION_ADD</item>
    /// <item>MESSAGE_REACTION_REMOVE</item>
    /// <item>MESSAGE_REACTION_REMOVE_ALL</item>
    /// <item>MESSAGE_REACTION_REMOVE_EMOJI</item>
    /// </list>
    /// </summary>
    DirectMessageReactions = 1 << 13,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>TYPING_START</item>
    /// </list>
    /// </summary>
    DirectMessageTyping = 1 << 14,

    /// <summary>
    /// Does not subscribe to any particular events, but requests that message contents be sent along with the following
    /// events:
    /// <list type="bullet">
    /// <item>MESSAGE_CREATE</item>
    /// <item>MESSAGE_UPDATE</item>
    /// <item>MESSAGE_DELETE</item>
    /// </list>
    ///
    /// If this intent is not specified, all raw message content strings will be empty. You may still receive user input
    /// using interactions, such as slash commands or modals.
    /// </summary>
    MessageContent = 1 << 15,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>GUILD_SCHEDULED_EVENT_CREATE</item>
    /// <item>GUILD_SCHEDULED_EVENT_UPDATE</item>
    /// <item>GUILD_SCHEDULED_EVENT_DELETE</item>
    /// <item>GUILD_SCHEDULED_EVENT_USER_ADD</item>
    /// <item>GUILD_SCHEDULED_EVENT_USER_REMOVE</item>
    /// </list>
    /// </summary>
    GuildScheduledEvents = 1 << 16,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>AUTO_MODERATION_RULE_CREATE</item>
    /// <item>AUTO_MODERATION_RULE_UPDATE</item>
    /// <item>AUTO_MODERATION_RULE_DELETE</item>
    /// </list>
    /// </summary>
    AutoModerationConfiguration = 1 << 20,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>AUTO_MODERATION_ACTION_EXECUTION</item>
    /// </list>
    /// </summary>
    AutoModerationExecution = 1 << 21,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>MESSAGE_POLL_VOTE_ADD</item>
    /// <item>MESSAGE_POLL_VOTE_REMOVE</item>
    /// </list>
    /// </summary>
    GuildMessagePolls = 1 << 24,

    /// <summary>
    /// Subscribes to the following events:
    /// <list type="bullet">
    /// <item>MESSAGE_POLL_VOTE_ADD</item>
    /// <item>MESSAGE_POLL_VOTE_REMOVE</item>
    /// </list>
    /// </summary>
    DirectMessagePolls = 1 << 25,
}
