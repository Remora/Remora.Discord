//
//  IReaction.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using System.Drawing;
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a reaction to a message.
/// </summary>
[PublicAPI]
public interface IReaction
{
    /// <summary>
    /// Gets the total number of times this emoji has been used to react, including burst (super) reactions.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets detailed information about the reaction counts.
    /// </summary>
    IReactionCountDetails CountDetails { get; }

    /// <summary>
    /// Gets a value indicating whether the current user has added a reaction using this emoji.
    /// </summary>
    bool HasCurrentUserReacted { get; }

    /// <summary>
    /// Gets a value indicating whether the current user has added a burst (super) reaction using this emoji.
    /// </summary>
    bool HasCurrentUserBurstReacted { get; }

    /// <summary>
    /// Gets the partial emoji information.
    /// </summary>
    IPartialEmoji Emoji { get; }

    /// <summary>
    /// Gets the colors used for the super reaction.
    /// </summary>
    IReadOnlyList<Color> BurstColours { get; }
}
