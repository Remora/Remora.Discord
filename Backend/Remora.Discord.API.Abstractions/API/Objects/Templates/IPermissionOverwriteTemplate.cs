//
//  IPermissionOverwriteTemplate.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a channel- or category-specific permission overwrite.
/// </summary>
[PublicAPI]
public interface IPermissionOverwriteTemplate
{
    /// <summary>
    /// Gets the ID of the role or user ID that the overwrite affects.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Gets the type of the overwrite.
    /// </summary>
    PermissionOverwriteType Type { get; }

    /// <summary>
    /// Gets the set of permissions that are explicitly allowed.
    /// </summary>
    IDiscordPermissionSet Allow { get; }

    /// <summary>
    /// Gets the set of permissions that are explicitly denied.
    /// </summary>
    IDiscordPermissionSet Deny { get; }
}
