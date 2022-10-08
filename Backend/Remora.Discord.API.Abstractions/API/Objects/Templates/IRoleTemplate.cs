//
//  IRoleTemplate.cs
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
public interface IRoleTemplate
{
    /// <summary>
    /// Gets the relative ID of the role.
    /// </summary>
    int ID { get; }

    /// <inheritdoc cref="IRole.Name"/>
    string Name { get; }

    /// <inheritdoc cref="IRole.Colour"/>
    Color Colour { get; }

    /// <inheritdoc cref="IRole.IsHoisted"/>
    bool IsHoisted { get; }

    /// <inheritdoc cref="IRole.Icon"/>
    Optional<IImageHash?> Icon { get; }

    /// <inheritdoc cref="IRole.UnicodeEmoji"/>
    Optional<string?> UnicodeEmoji { get; }

    /// <inheritdoc cref="IRole.Position"/>
    int Position { get; }

    /// <inheritdoc cref="IRole.Permissions"/>
    IDiscordPermissionSet Permissions { get; }

    /// <inheritdoc cref="IRole.IsMentionable"/>
    bool IsMentionable { get; }

    /// <inheritdoc cref="IRole.Name"/>
    Optional<IRoleTags> Tags { get; }
}
