//
//  Reaction.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class Reaction : IReaction
    {
        /// <inheritdoc />
        public int Count { get; }

        /// <inheritdoc />
        public bool HasCurrentUserReacted { get; }

        /// <inheritdoc />
        public IPartialEmoji Emoji { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reaction"/> class.
        /// </summary>
        /// <param name="count">The number of times users have reacted with this emoji.</param>
        /// <param name="hasCurrentUserReacted">Whether the current user has reacted.</param>
        /// <param name="emoji">The emoji.</param>
        public Reaction(int count, bool hasCurrentUserReacted, IPartialEmoji emoji)
        {
            this.Count = count;
            this.HasCurrentUserReacted = hasCurrentUserReacted;
            this.Emoji = emoji;
        }
    }
}
