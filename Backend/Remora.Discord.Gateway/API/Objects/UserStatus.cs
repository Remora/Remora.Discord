//
//  UserStatus.cs
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

namespace Remora.Discord.Gateway.API.Objects
{
    /// <summary>
    /// Enumerates the various statuses a user can have.
    /// </summary>
    internal enum UserStatus
    {
        /// <summary>
        /// The user is online and active.
        /// </summary>
        Online,

        /// <summary>
        /// The user is in "do not disturb" mode.
        /// </summary>
        DND,

        /// <summary>
        /// The user is online, but idle.
        /// </summary>
        Idle,

        /// <summary>
        /// The user is online, but has hidden that status.
        /// </summary>
        Invisible,

        /// <summary>
        /// The user is offline.
        /// </summary>
        Offline
    }
}
