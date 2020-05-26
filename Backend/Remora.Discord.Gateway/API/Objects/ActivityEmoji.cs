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

using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Objects
{
    /// <summary>
    /// Represents an emoji displayed in an activity.
    /// </summary>
    internal sealed class ActivityEmoji
    {
        /// <summary>
        /// Gets the name of the emoji.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the ID of the emoji.
        /// </summary>
        public Optional<Snowflake> ID { get; }

        /// <summary>
        /// Gets a value indicating whether the emoji is animated.
        /// </summary>
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
