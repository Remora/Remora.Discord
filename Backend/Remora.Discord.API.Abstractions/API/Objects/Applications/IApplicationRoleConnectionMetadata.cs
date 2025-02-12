//
//  IApplicationRoleConnectionMetadata.cs
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
/// Represents an application role connection metadata field.
/// </summary>
[PublicAPI]
public interface IApplicationRoleConnectionMetadata
{
    /// <summary>
    /// Gets the type of metadata value.
    /// </summary>
    ApplicationRoleConnectionMetadataType Type { get; }

    /// <summary>
    /// Gets the dictionary key for the metadata field.
    /// </summary>
    /// <remarks>The key must only consist of a-z, 0-9 or _ and must have a length of max. 50 characters.</remarks>
    string Key { get; }

    /// <summary>
    /// Gets the name of the metadata field.
    /// </summary>
    /// <remarks>The length of the name must be max. 100 characters.</remarks>
    string Name { get; }

    /// <summary>
    /// Gets the localized names of the metadata field.
    /// </summary>
    Optional<IReadOnlyDictionary<string, string>> NameLocalizations { get; }

    /// <summary>
    /// Gets the description of the metadata field.
    /// </summary>
    /// <remarks>The length of the description must be max. 200 characters.</remarks>
    string Description { get; }

    /// <summary>
    /// Gets the localized descriptions of the metadata field.
    /// </summary>
    Optional<IReadOnlyDictionary<string, string>> DescriptionLocalizations { get; }
}
