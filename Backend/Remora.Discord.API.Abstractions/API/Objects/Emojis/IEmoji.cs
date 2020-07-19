//
//  IEmoji.cs
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
using Remora.Discord.API.Abstractions.Users;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Emojis
{
    /// <summary>
    /// Represents an emoji.
    /// </summary>
    public interface IEmoji
    {
        /// <summary>
        /// Gets the ID of the emoji.
        /// </summary>
        Snowflake? ID { get; }

        /// <summary>
        /// Gets the name of the emoji.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets a list of roles this emoji is whitelisted to.
        /// </summary>
        Optional<IReadOnlyList<Snowflake>> Roles { get; }

        /// <summary>
        /// Gets the user that created this emoji.
        /// </summary>
        Optional<IUser> User { get; }

        /// <summary>
        /// Gets a value indicating whether this emoji must be wrapped in colons.
        /// </summary>
        Optional<bool> RequireColons { get; }

        /// <summary>
        /// Gets a value indicating whether this emoji is managed.
        /// </summary>
        Optional<bool> IsManaged { get; }

        /// <summary>
        /// Gets a value indicating whether this emoji is animated.
        /// </summary>
        Optional<bool> IsAnimated { get; }

        /// <summary>
        /// Gets a value indicating whether this emoji is available. May be false due to a loss of server boosts.
        /// </summary>
        Optional<bool> IsAvailable { get; }
    }
}
