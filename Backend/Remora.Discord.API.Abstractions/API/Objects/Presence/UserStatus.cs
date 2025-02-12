//
//  UserStatus.cs
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
/// Enumerates various presence states for a user.
/// </summary>
[PublicAPI]
public enum UserStatus
{
    /// <summary>
    /// The user is idle.
    /// </summary>
    Idle,

    /// <summary>
    /// The user is invisible.
    /// </summary>
    Invisible,

    /// <summary>
    /// The user is not to be disturbed.
    /// </summary>
    DND,

    /// <summary>
    /// The user is online.
    /// </summary>
    Online,

    /// <summary>
    /// The user is offline.
    /// </summary>
    Offline
}
