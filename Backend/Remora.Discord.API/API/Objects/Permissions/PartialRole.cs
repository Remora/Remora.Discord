//
//  PartialRole.cs
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

using System.Drawing;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class PartialRole : IPartialRole
    {
        /// <inheritdoc />
        public Optional<Snowflake> ID { get; }

        /// <inheritdoc />
        public Optional<string> Name { get; }

        /// <inheritdoc />
        public Optional<Color> Colour { get; }

        /// <inheritdoc />
        public Optional<bool> IsHoisted { get; }

        /// <inheritdoc />
        public Optional<int> Position { get; }

        /// <inheritdoc />
        public Optional<IDiscordPermissionSet> Permissions { get; }

        /// <inheritdoc />
        public Optional<bool> IsManaged { get; }

        /// <inheritdoc />
        public Optional<bool> IsMentionable { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialRole"/> class.
        /// </summary>
        /// <param name="id">The ID of the role.</param>
        /// <param name="name">The name of the role.</param>
        /// <param name="colour">The colour of the role.</param>
        /// <param name="isHoisted">Whether the role is displayed separately from other roles.</param>
        /// <param name="position">The position of the role.</param>
        /// <param name="permissions">The permissions of the role.</param>
        /// <param name="isManaged">Whether the role is managed by an integration.</param>
        /// <param name="isMentionable">Whether the role is mentionable.</param>
        public PartialRole
        (
            Optional<Snowflake> id,
            Optional<string> name,
            Optional<Color> colour,
            Optional<bool> isHoisted,
            Optional<int> position,
            Optional<IDiscordPermissionSet> permissions,
            Optional<bool> isManaged,
            Optional<bool> isMentionable
        )
        {
            this.ID = id;
            this.Name = name;
            this.Colour = colour;
            this.IsHoisted = isHoisted;
            this.Position = position;
            this.Permissions = permissions;
            this.IsManaged = isManaged;
            this.IsMentionable = isMentionable;
        }
    }
}
