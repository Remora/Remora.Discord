//
//  IEmoji.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an emoji.
/// </summary>
[PublicAPI]
public interface IEmoji : IPartialEmoji
{
    /// <summary>
    /// Gets the ID of the emoji.
    /// </summary>
    new Snowflake? ID { get; }

    /// <summary>
    /// Gets the name of the emoji.
    /// </summary>
    new string? Name { get; }

    /// <summary>
    /// Gets a list of roles this emoji is whitelisted to.
    /// </summary>
    new Optional<IReadOnlyList<Snowflake>> Roles { get; }

    /// <summary>
    /// Gets the user that created this emoji.
    /// </summary>
    new Optional<IUser> User { get; }

    /// <summary>
    /// Gets a value indicating whether this emoji must be wrapped in colons.
    /// </summary>
    new Optional<bool> RequireColons { get; }

    /// <summary>
    /// Gets a value indicating whether this emoji is managed.
    /// </summary>
    new Optional<bool> IsManaged { get; }

    /// <summary>
    /// Gets a value indicating whether this emoji is animated.
    /// </summary>
    new Optional<bool> IsAnimated { get; }

    /// <summary>
    /// Gets a value indicating whether this emoji is available. May be false due to a loss of server boosts.
    /// </summary>
    new Optional<bool> IsAvailable { get; }

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialEmoji.ID => this.ID;

    /// <inheritdoc/>
    Optional<string?> IPartialEmoji.Name => this.Name;

    /// <inheritdoc/>
    Optional<IReadOnlyList<Snowflake>> IPartialEmoji.Roles => this.Roles;

    /// <inheritdoc/>
    Optional<IUser> IPartialEmoji.User => this.User;

    /// <inheritdoc/>
    Optional<bool> IPartialEmoji.RequireColons => this.RequireColons;

    /// <inheritdoc/>
    Optional<bool> IPartialEmoji.IsManaged => this.IsManaged;

    /// <inheritdoc/>
    Optional<bool> IPartialEmoji.IsAnimated => this.IsAnimated;

    /// <inheritdoc/>
    Optional<bool> IPartialEmoji.IsAvailable => this.IsAvailable;
}
