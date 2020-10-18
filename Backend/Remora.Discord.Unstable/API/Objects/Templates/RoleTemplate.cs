//
//  RoleTemplate.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class RoleTemplate : IRoleTemplate
    {
        /// <inheritdoc />
        public int ID { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Color Colour { get; }

        /// <inheritdoc />
        public bool IsHoisted { get; }

        /// <inheritdoc />
        public bool IsMentionable { get; }

        /// <inheritdoc />
        public IDiscordPermissionSet Permissions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleTemplate"/> class.
        /// </summary>
        /// <param name="id">The ID of the role.</param>
        /// <param name="name">The name of the role.</param>
        /// <param name="colour">The colour of the role.</param>
        /// <param name="isHoisted">Whether the role is displayed separately from other roles.</param>
        /// <param name="permissions">The permissions of the role.</param>
        /// <param name="isMentionable">Whether the role is mentionable.</param>
        public RoleTemplate
        (
            int id,
            string name,
            Color colour,
            bool isHoisted,
            bool isMentionable,
            IDiscordPermissionSet permissions
        )
        {
            this.ID = id;
            this.Name = name;
            this.Colour = colour;
            this.IsHoisted = isHoisted;
            this.IsMentionable = isMentionable;
            this.Permissions = permissions;
        }
    }
}
