//
//  IPresence.cs
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
using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Activities;
using Remora.Discord.API.Abstractions.Users;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Presence
{
    /// <summary>
    /// Represents a user's presence.
    /// </summary>
    public interface IPresence
    {
        /// <summary>
        /// Gets the user the presence is being updated for.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// Gets the roles the user is in.
        /// </summary>
        IReadOnlyList<Snowflake> Roles { get; }

        /// <summary>
        /// Gets the user's current activity.
        /// </summary>
        IActivity? Game { get; }

        /// <summary>
        /// Gets the ID of the guild.
        /// </summary>
        Snowflake GuildID { get; }

        /// <summary>
        /// Gets the current status of the user.
        /// </summary>
        ClientStatus Status { get; }

        /// <summary>
        /// Gets the user's current activities.
        /// </summary>
        IReadOnlyList<IActivity> Activities { get; }

        /// <summary>
        /// Gets the user's platform-dependent status.
        /// </summary>
        IClientStatuses ClientStatus { get; }

        /// <summary>
        /// Gets the time when the user started boosting the guild.
        /// </summary>
        Optional<DateTime?> PremiumSince { get; }

        /// <summary>
        /// Gets the user's nickname, if one is set.
        /// </summary>
        Optional<string?> Nickname { get; }
    }
}
