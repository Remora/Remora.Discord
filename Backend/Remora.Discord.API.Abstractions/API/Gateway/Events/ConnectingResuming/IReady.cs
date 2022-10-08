//
//  IReady.cs
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
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents initial gateway state information.
/// </summary>
[PublicAPI]
public interface IReady : IGatewayEvent
{
    /// <summary>
    /// Gets the gateway version.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Gets information about the current user.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// Gets a list of guilds the user is in.
    /// </summary>
    IReadOnlyList<IUnavailableGuild> Guilds { get; }

    /// <summary>
    /// Gets the session ID. Used for resuming.
    /// </summary>
    string SessionID { get; }

    /// <summary>
    /// Gets the resume gateway URL.
    /// </summary>
    string ResumeGatewayUrl { get; }

    /// <summary>
    /// Gets the shard information associated with this session.
    /// </summary>
    Optional<IShardIdentification> Shard { get; }

    /// <summary>
    /// Gets the OAuth2 application associated with this session.
    /// </summary>
    IPartialApplication Application { get; }
}
