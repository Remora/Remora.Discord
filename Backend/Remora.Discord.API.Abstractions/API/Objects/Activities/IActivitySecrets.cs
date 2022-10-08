//
//  IActivitySecrets.cs
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
/// Represents a set of secrets used for interacting with the activity.
/// </summary>
[PublicAPI]
public interface IActivitySecrets
{
    /// <summary>
    /// Gets the secret used for joining the party.
    /// </summary>
    Optional<string> Join { get; }

    /// <summary>
    /// Gets the secret used for spectating the party.
    /// </summary>
    Optional<string> Spectate { get; }

    /// <summary>
    /// Gets the secret used for joining a specific instanced match.
    /// </summary>
    Optional<string> Match { get; }
}
