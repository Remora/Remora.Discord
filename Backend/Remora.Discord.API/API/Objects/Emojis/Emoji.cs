//
//  Emoji.cs
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
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Emoji : IEmoji
    {
        /// <inheritdoc />
        public Snowflake? ID { get; }

        /// <inheritdoc />
        public string? Name { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<Snowflake>> Roles { get; }

        /// <inheritdoc />
        public Optional<IUser> User { get; }

        /// <inheritdoc />
        public Optional<bool> RequireColons { get; }

        /// <inheritdoc />
        public Optional<bool> IsManaged { get; }

        /// <inheritdoc />
        public Optional<bool> IsAnimated { get; }

        /// <inheritdoc />
        public Optional<bool> IsAvailable { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Emoji"/> class.
        /// </summary>
        /// <param name="id">The ID of the emoji, if it is custom.</param>
        /// <param name="name">The name of the emoji, if it is custom; Otherwise, the unicode representation.</param>
        /// <param name="roles">The roles that may use the emoji.</param>
        /// <param name="user">The user that made the emoji.</param>
        /// <param name="requireColons">Whether the emoji must be surrounded with colons.</param>
        /// <param name="isManaged">Whether the emoji is managed by an integration.</param>
        /// <param name="isAnimated">Whether the emoji is animated.</param>
        /// <param name="isAvailable">Whether the emoji is currently available.</param>
        public Emoji
        (
            Snowflake? id,
            string? name,
            Optional<IReadOnlyList<Snowflake>> roles,
            Optional<IUser> user,
            Optional<bool> requireColons,
            Optional<bool> isManaged,
            Optional<bool> isAnimated,
            Optional<bool> isAvailable
        )
        {
            this.ID = id;
            this.Name = name;
            this.Roles = roles;
            this.User = user;
            this.RequireColons = requireColons;
            this.IsManaged = isManaged;
            this.IsAnimated = isAnimated;
            this.IsAvailable = isAvailable;
        }
    }
}
