//
//  PermissionOverwrite.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class PermissionOverwrite : IPermissionOverwrite
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public PermissionOverwriteType Type { get; }

        /// <inheritdoc />
        public IDiscordPermissionSet Allow { get; }

        /// <inheritdoc />
        public IDiscordPermissionSet Deny { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionOverwrite"/> class.
        /// </summary>
        /// <param name="id">The ID of the overwrite.</param>
        /// <param name="type">The type of the overwrite target.</param>
        /// <param name="allow">The permissions explicitly allowed.</param>
        /// <param name="deny">The permissions explicitly denied.</param>
        public PermissionOverwrite
        (
            Snowflake id,
            PermissionOverwriteType type,
            IDiscordPermissionSet allow,
            IDiscordPermissionSet deny
        )
        {
            this.ID = id;
            this.Type = type;
            this.Allow = allow;
            this.Deny = deny;
        }
    }
}
