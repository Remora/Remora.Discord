//
//  IPartialGuildApplicationCommandPermissions.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a set of permissions for a command in a guild.
    /// </summary>
    public interface IPartialGuildApplicationCommandPermissions
    {
        /// <summary>
        /// Gets the ID of the command.
        /// </summary>
        Optional<Snowflake> ID { get; }

        /// <summary>
        /// Gets the ID of the application the command belongs to.
        /// </summary>
        Optional<Snowflake> ApplicationID { get; }

        /// <summary>
        /// Gets the ID of the guild.
        /// </summary>
        Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Gets the permissions for the command in the guild.
        /// </summary>
        Optional<IReadOnlyList<IApplicationCommandPermissions>> Permissions { get; }
    }
}
