//
//  IConnection.cs
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
/// Represents a connection between a user account and an external service.
/// </summary>
[PublicAPI]
public interface IConnection
{
    /// <summary>
    /// Gets the ID of the connection account.
    /// </summary>
    string ID { get; }

    /// <summary>
    /// Gets the username of the connection account.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of connection.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets a value indicating whether the connection has been revoked.
    /// </summary>
    Optional<bool> IsRevoked { get; }

    /// <summary>
    /// Gets a list of server integrations.
    /// </summary>
    Optional<IReadOnlyList<IPartialIntegration>> Integrations { get; }

    /// <summary>
    /// Gets a value indicating whether the connection is verified.
    /// </summary>
    bool IsVerified { get; }

    /// <summary>
    /// Gets a value indicating whether friend synchronization is enabled.
    /// </summary>
    bool IsFriendSyncEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether activities related to this connection are shown in presence updates.
    /// </summary>
    bool ShouldShowActivity { get; }

    /// <summary>
    /// Gets the visibility of this connection.
    /// </summary>
    ConnectionVisibility Visibility { get; }
}
