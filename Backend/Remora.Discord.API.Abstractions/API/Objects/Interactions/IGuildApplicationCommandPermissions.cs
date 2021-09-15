//
//  IGuildApplicationCommandPermissions.cs
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
    /// Represents a set of permissions for a command in a guild.
    /// </summary>
    [PublicAPI]
    public interface IGuildApplicationCommandPermissions : IPartialGuildApplicationCommandPermissions
    {
        /// <summary>
        /// Gets the ID of the command.
        /// </summary>
        new Snowflake ID { get; }

        /// <summary>
        /// Gets the ID of the application the command belongs to.
        /// </summary>
        new Snowflake ApplicationID { get; }

        /// <summary>
        /// Gets the ID of the guild.
        /// </summary>
        new Snowflake GuildID { get; }

        /// <summary>
        /// Gets the permissions for the command in the guild.
        /// </summary>
        new IReadOnlyList<IApplicationCommandPermissions> Permissions { get; }

        /// <inheritdoc/>
        Optional<Snowflake> IPartialGuildApplicationCommandPermissions.ID => this.ID;

        /// <inheritdoc/>
        Optional<Snowflake> IPartialGuildApplicationCommandPermissions.ApplicationID => this.ApplicationID;

        /// <inheritdoc/>
        Optional<Snowflake> IPartialGuildApplicationCommandPermissions.GuildID => this.GuildID;

        /// <inheritdoc/>
        Optional<IReadOnlyList<IApplicationCommandPermissions>> IPartialGuildApplicationCommandPermissions.Permissions => throw new System.NotImplementedException();
    }
}
