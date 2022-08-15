//
//  IRole.cs
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

using System.Drawing;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a Discord role.
/// </summary>
[PublicAPI]
public interface IRole : IPartialRole
{
    /// <summary>
    /// Gets the ID of the role.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the role.
    /// </summary>
    new string Name { get; }

    /// <summary>
    /// Gets the colour of the role.
    /// </summary>
    new Color Colour { get; }

    /// <summary>
    /// Gets a value indicating whether the role is displayed separately in the sidebar.
    /// </summary>
    new bool IsHoisted { get; }

    /// <summary>
    /// Gets the role's icon, if any.
    /// </summary>
    new Optional<IImageHash?> Icon { get; }

    /// <summary>
    /// Gets the role's unicode emoji icon, if any.
    /// </summary>
    new Optional<string?> UnicodeEmoji { get; }

    /// <summary>
    /// Gets the position of the role.
    /// </summary>
    new int Position { get; }

    /// <summary>
    /// Gets the permission set for this role.
    /// </summary>
    new IDiscordPermissionSet Permissions { get; }

    /// <summary>
    /// Gets a value indicating whether this role is managed by an integration.
    /// </summary>
    new bool IsManaged { get; }

    /// <summary>
    /// Gets a value indicating whether this role is mentionable.
    /// </summary>
    new bool IsMentionable { get; }

    /// <summary>
    /// Gets the tags the role has.
    /// </summary>
    new Optional<IRoleTags> Tags { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialRole.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialRole.Name => this.Name;

    /// <inheritdoc/>
    Optional<Color> IPartialRole.Colour => this.Colour;

    /// <inheritdoc/>
    Optional<bool> IPartialRole.IsHoisted => this.IsHoisted;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialRole.Icon => this.Icon;

    /// <inheritdoc/>
    Optional<string?> IPartialRole.UnicodeEmoji => this.UnicodeEmoji;

    /// <inheritdoc/>
    Optional<int> IPartialRole.Position => this.Position;

    /// <inheritdoc/>
    Optional<IDiscordPermissionSet> IPartialRole.Permissions => new(this.Permissions);

    /// <inheritdoc/>
    Optional<bool> IPartialRole.IsManaged => this.IsManaged;

    /// <inheritdoc/>
    Optional<bool> IPartialRole.IsMentionable => this.IsMentionable;

    /// <inheritdoc/>
    Optional<IRoleTags> IPartialRole.Tags => this.Tags;
}
