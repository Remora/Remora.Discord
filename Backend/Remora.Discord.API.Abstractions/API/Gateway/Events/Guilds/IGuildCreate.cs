//
//  IGuildCreate.cs
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

using Remora.Discord.API.Abstractions.Guilds;

namespace Remora.Discord.API.Abstractions.Events
{
    /// <summary>
    /// Represents the creation of a guild. This event is sent in one of three scenarios:
    ///     1. When a user is initially connecting to lazily load and backfill information for all unavailable guilds
    ///        sent in the <see cref="IReady"/> event.
    ///     2. When a guild becomes available again to the client.
    ///     3. When the current user joins a new guild.
    /// </summary>
    public interface IGuildCreate : IGatewayEvent, IGuild
    {
    }
}
