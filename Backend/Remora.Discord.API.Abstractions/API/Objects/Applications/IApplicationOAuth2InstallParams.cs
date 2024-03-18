//
//  IApplicationOAuth2InstallParams.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents the OAuth2 install parameters for an application.
/// </summary>
public interface IApplicationOAuth2InstallParams
{
    /// <summary>
    /// Gets the permissions required for the application.
    /// </summary>
    /// <remarks>
    /// Only applicable if <see cref="IApplicationOAuth2InstallParams.Scopes"/> includes `bot`.
    /// </remarks>
    IDiscordPermissionSet Permissions { get; }

    /// <summary>
    /// Gets the list of OAuth2 scopes required for the application.
    /// </summary>
    IReadOnlyList<string> Scopes { get; }
}
