//
//  ActivityEmoji.cs
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

using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <summary>
    /// Represents an emoji displayed in an activity.
    /// </summary>
    public class ActivityEmoji : IActivityEmoji
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Optional<Snowflake> ID { get; }

        /// <inheritdoc />
        public Optional<bool> Animated { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEmoji"/> class.
        /// </summary>
        /// <param name="name">The name of the emoji.</param>
        /// <param name="id">The ID of the emoji.</param>
        /// <param name="animated">Whether the emoji is animated.</param>
        public ActivityEmoji(string name, Optional<Snowflake> id = default, Optional<bool> animated = default)
        {
            this.Name = name;
            this.ID = id;
            this.Animated = animated;
        }
    }
}
