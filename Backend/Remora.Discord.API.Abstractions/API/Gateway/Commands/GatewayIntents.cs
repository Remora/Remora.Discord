//
//  GatewayIntents.cs
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

namespace Remora.Discord.API.Abstractions.Gateway.Commands
{
    /// <summary>
    /// Enumerates the gateway intents that can be specified.
    /// </summary>
    [Flags, PublicAPI]
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
        /// </summary>
        Guilds = 1 << 0,

        /// <summary>
        /// Subscribes to the following events:
        ///     - GUILD_MEMBER_ADD
        ///     - GUILD_MEMBER_UPDATE
        ///     - GUILD_MEMBER_REMOVE
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
        /// </summary>
        GuildEmojis = 1 << 3,

        /// <summary>
        /// Subscribes to the following events:
        ///     - GUILD_INTEGRATIONS_UPDATE
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
    }
}
