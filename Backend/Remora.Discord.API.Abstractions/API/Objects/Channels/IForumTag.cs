//
//  IForumTag.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a tag that can be attached to posts in a forum.
/// </summary>
[PublicAPI]
public interface IForumTag : IPartialForumTag
{
    /// <summary>
    /// Gets the ID of the tag.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    new string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the tag can only be added or removed by someone with the
    /// <see cref="DiscordPermission.ManageThreads"/> permission.
    /// </summary>
    new bool IsModerated { get; }

    /// <summary>
    /// Gets the ID of the custom guild emoji to use.
    /// </summary>
    /// <remarks>
    /// Discord's documentation is somewhat unclear as to whether this field is optional, nullable, or both. As as
    /// result, we're hedging our bets here.
    /// </remarks>
    new Optional<Snowflake?> EmojiID { get; }

    /// <summary>
    /// Gets the unicode string of the emoji to use.
    /// </summary>
    /// <remarks>
    /// Discord's documentation is somewhat unclear as to whether this field is optional, nullable, or both. As as
    /// result, we're hedging our bets here.
    /// </remarks>
    new Optional<string?> EmojiName { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialForumTag.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialForumTag.Name => this.Name;

    /// <inheritdoc/>
    Optional<bool> IPartialForumTag.IsModerated => this.IsModerated;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialForumTag.EmojiID => this.EmojiID;

    /// <inheritdoc/>
    Optional<string?> IPartialForumTag.EmojiName => this.EmojiName;
}
