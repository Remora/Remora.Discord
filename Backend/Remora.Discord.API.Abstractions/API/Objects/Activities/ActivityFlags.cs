//
//  ActivityFlags.cs
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
/// Enumerates the flags an activity can have.
/// </summary>
[PublicAPI, Flags]
public enum ActivityFlags
{
    /// <summary>
    /// The activity has a specific instance.
    /// </summary>
    Instance = 1 << 0,

    /// <summary>
    /// The activity can be joined.
    /// </summary>
    Join = 1 << 1,

    /// <summary>
    /// The activity can be spectated.
    /// </summary>
    Spectate = 1 << 2,

    /// <summary>
    /// The activity can be sent a join request.
    /// </summary>
    JoinRequest = 1 << 3,

    /// <summary>
    /// The activity is synchronized? Discord's documentation is unclear.
    /// </summary>
    Sync = 1 << 4,

    /// <summary>
    /// The activity is currently ongoing? Discord's documentation is unclear.
    /// </summary>
    Play = 1 << 5,

    /// <summary>
    /// The activity is restricted to friends only.
    /// </summary>
    PartyPrivacyFriends = 1 << 6,

    /// <summary>
    /// The activity is restricted to members of the same voice channel.
    /// </summary>
    PartyPrivacyVoiceChannel = 1 << 7,

    /// <summary>
    /// The activity is embedded.
    /// </summary>
    Embedded = 1 << 8
}
