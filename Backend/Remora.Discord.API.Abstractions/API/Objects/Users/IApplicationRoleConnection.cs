//
//  IApplicationRoleConnection.cs
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
/// Represents a role connection that an application has attached to an user.
/// </summary>
[PublicAPI]
public interface IApplicationRoleConnection
{
    /// <summary>
    /// Gets the vanity name of the platform a bot has connected.
    /// </summary>
    /// <remarks>The length of the platform username must be max. 50 characters.</remarks>
    Optional<string> PlatformName { get; }

    /// <summary>
    /// Gets the username on the platform a bot has connected.
    /// </summary>
    /// <remarks>The length of the platform username must be max. 100 characters.</remarks>
    Optional<string> PlatformUsername { get; }

    /// <summary>
    /// Gets the object mapping of <see cref="IApplicationRoleConnectionMetadata.Key"/> to their stringified value
    /// for the user on the platform a bot has connected.
    /// </summary>
    /// <remarks>The length of the stringified value must max. 100 characters.</remarks>
    Optional<IReadOnlyDictionary<string, string>> Metadata { get; }
}
