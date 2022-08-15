//
//  IPermissionOverwrite.cs
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
/// Represents a channel- or category-specific permission overwrite.
/// </summary>
[PublicAPI]
public interface IPermissionOverwrite : IPartialPermissionOverwrite
{
    /// <summary>
    /// Gets the ID of the role or user ID that the overwrite affects.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the type of the overwrite.
    /// </summary>
    new PermissionOverwriteType Type { get; }

    /// <summary>
    /// Gets the set of permissions that are explicitly allowed.
    /// </summary>
    new IDiscordPermissionSet Allow { get; }

    /// <summary>
    /// Gets the set of permissions that are explicitly denied.
    /// </summary>
    new IDiscordPermissionSet Deny { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialPermissionOverwrite.ID => this.ID;

    /// <inheritdoc/>
    Optional<PermissionOverwriteType> IPartialPermissionOverwrite.Type => this.Type;

    /// <inheritdoc/>
    Optional<IDiscordPermissionSet> IPartialPermissionOverwrite.Allow => new(this.Allow);

    /// <inheritdoc/>
    Optional<IDiscordPermissionSet> IPartialPermissionOverwrite.Deny => new(this.Deny);
}
