//
//  IAuthorizationInformation.cs
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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about OAuth2 authorizations.
/// </summary>
[PublicAPI]
public interface IAuthorizationInformation
{
    /// <summary>
    /// Gets the current application.
    /// </summary>
    IPartialApplication Application { get; }

    /// <summary>
    /// Gets the scopes the user has authorized the application for.
    /// </summary>
    IReadOnlyList<string> Scopes { get; }

    /// <summary>
    /// Gets the time when the access token expires.
    /// </summary>
    DateTimeOffset Expires { get; }

    /// <summary>
    /// Gets the user who has authorized the application, if the user has authorized with the "identity" scope.
    /// </summary>
    Optional<IUser> User { get; }
}
