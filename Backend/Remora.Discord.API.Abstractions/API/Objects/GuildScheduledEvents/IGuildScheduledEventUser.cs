//
//  IGuildScheduledEventUser.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a user subscribed to a guild event.
/// </summary>
[PublicAPI]
public interface IGuildScheduledEventUser
{
    /// <summary>
    /// Gets the ID of the event the user subscribed to.
    /// </summary>
    Snowflake GuildScheduledEventID { get; }

    /// <summary>
    /// Gets the user which subscribed to the event.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// Gets the member information associated with the user.
    /// </summary>
    Optional<IGuildMember> GuildMember { get; }
}
