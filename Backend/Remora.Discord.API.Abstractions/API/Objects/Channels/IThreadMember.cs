//
//  IThreadMember.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a thread member.
    /// </summary>
    public interface IThreadMember
    {
        /// <summary>
        /// Gets the ID of the thread.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        Snowflake UserID { get; }

        /// <summary>
        /// Gets the time the current user last joined the thread.
        /// </summary>
        DateTimeOffset JoinTimestamp { get; }

        /// <summary>
        /// Gets any user-thread settings.
        /// </summary>
        ThreadMemberFlags Flags { get; }
    }
}
