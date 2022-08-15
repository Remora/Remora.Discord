//
//  IDiscordRestOAuth2API.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord OAuth2 API.
/// </summary>
[PublicAPI]
public interface IDiscordRestOAuth2API
{
    /// <summary>
    /// Gets the bot's OAuth2 application object. Flags are typically not included.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IApplication>> GetCurrentBotApplicationInformationAsync
    (
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets information about the bot's current authorizations.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IAuthorizationInformation>> GetCurrentAuthorizationInformationAsync
    (
        CancellationToken ct = default
    );
}
