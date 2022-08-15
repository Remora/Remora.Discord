//
//  UserFlags.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various user account flags.
/// </summary>
[PublicAPI, Flags]
public enum UserFlags
{
    /// <summary>
    /// The user is a Discord employee.
    /// </summary>
    DiscordEmployee = 1 << 0,

    /// <summary>
    /// The user owns a a Discord-partnered server.
    /// </summary>
    PartneredServerOwner = 1 << 1,

    /// <summary>
    /// The user is a member of a HypeSquad event.
    /// </summary>
    HypeSquad = 1 << 2,

    /// <summary>
    /// The user is a hunter (level 1).
    /// </summary>
    BugHunterLevel1 = 1 << 3,

    /// <summary>
    /// The user is part of House Bravery.
    /// </summary>
    HouseBravery = 1 << 6,

    /// <summary>
    /// The user is part of House Brilliance.
    /// </summary>
    HouseBrilliance = 1 << 7,

    /// <summary>
    /// The user is part of House Balance.
    /// </summary>
    HouseBalance = 1 << 8,

    /// <summary>
    /// The user is an early supporter.
    /// </summary>
    EarlySupporter = 1 << 9,

    /// <summary>
    /// The user is a team user.
    /// </summary>
    TeamUser = 1 << 10,

    /// <summary>
    /// The user is a hunter (level 2).
    /// </summary>
    BugHunterLevel2 = 1 << 14,

    /// <summary>
    /// The user is a verified bot.
    /// </summary>
    VerifiedBot = 1 << 16,

    /// <summary>
    /// The user is a verified bot developer.
    /// </summary>
    EarlyVerifiedBotDeveloper = 1 << 17,

    /// <summary>
    /// The user is a Discord-certified moderator.
    /// </summary>
    DiscordCertifiedModerator = 1 << 18,

    /// <summary>
    /// The user is a bot that only uses outgoing webhook interactions, and should always be shown as online.
    /// </summary>
    BotHttpInteractions = 1 << 19
}
