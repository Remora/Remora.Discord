//
//  IPartialPresence.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a user's presence.
    /// </summary>
    [PublicAPI]
    public interface IPartialPresence
    {
        /// <summary>
        /// Gets the user the presence is being updated for.
        /// </summary>
        Optional<IPartialUser> User { get; }

        /// <summary>
        /// Gets the ID of the guild.
        /// </summary>
        Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Gets the current status of the user.
        /// </summary>
        Optional<ClientStatus> Status { get; }

        /// <summary>
        /// Gets the user's current activities.
        /// </summary>
        Optional<IReadOnlyList<IActivity>?> Activities { get; }

        /// <summary>
        /// Gets the user's platform-dependent status.
        /// </summary>
        Optional<IClientStatuses> ClientStatus { get; }
    }
}
