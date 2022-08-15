//
//  MessageType.cs
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
/// Enumerates message types.
/// </summary>
[PublicAPI]
public enum MessageType
{
    /// <summary>
    /// A normal message.
    /// </summary>
    Default = 0,

    /// <summary>
    /// A member has joined the group DM.
    /// </summary>
    RecipientAdd = 1,

    /// <summary>
    /// A member has left the group DM.
    /// </summary>
    RecipientRemove = 2,

    /// <summary>
    /// Someone is calling.
    /// </summary>
    Call = 3,

    /// <summary>
    /// The name of the channel changed.
    /// </summary>
    ChannelNameChange = 4,

    /// <summary>
    /// The channel's icon changed.
    /// </summary>
    ChannelIconChange = 5,

    /// <summary>
    /// A message was pinned.
    /// </summary>
    ChannelPinnedMessage = 6,

    /// <summary>
    /// A user joined.
    /// </summary>
    UserJoin = 7,

    /// <summary>
    /// A user boosted the server.
    /// </summary>
    GuildBoost = 8,

    /// <summary>
    /// A user boosted the server to tier 1.
    /// </summary>
    GuildBoostTier1 = 9,

    /// <summary>
    /// A user boosted the server to tier 2.
    /// </summary>
    GuildBoostTier2 = 10,

    /// <summary>
    /// A user boosted the server to tier 3.
    /// </summary>
    GuildBoostTier3 = 11,

    /// <summary>
    /// Someone followed the channel.
    /// </summary>
    ChannelFollowAdd = 12,

    /// <summary>
    /// The server has been disqualified for inclusion into guild discovery.
    /// </summary>
    GuildDiscoveryDisqualified = 14,

    /// <summary>
    /// The server has qualified for inclusion into guild discovery.
    /// </summary>
    GuildDiscoveryQualified = 15,

    /// <summary>
    /// The server is going to disqualify from guild discovery soon.
    /// </summary>
    GuildDiscoveryGracePeriodInitialWarning = 16,

    /// <summary>
    /// The server is going to disqualify from guild discovery very soon.
    /// </summary>
    GuildDiscoveryGracePeriodFinalWarning = 17,

    /// <summary>
    /// A message that indicates a thread was created.
    /// </summary>
    ThreadCreated = 18,

    /// <summary>
    /// A reply to another message.
    /// </summary>
    InlineReply = 19,

    /// <summary>
    /// An application command.
    /// </summary>
    ChatInputCommand = 20,

    /// <summary>
    /// A message that starts a thread.
    /// </summary>
    ThreadStarterMessage = 21,

    /// <summary>
    /// A reminder message about a guild invite.
    /// </summary>
    GuildInviteReminder = 22,

    /// <summary>
    /// A context menu command.
    /// </summary>
    ContextMenuCommand = 23
}
