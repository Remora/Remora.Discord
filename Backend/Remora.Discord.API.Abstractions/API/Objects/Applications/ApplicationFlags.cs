//
//  ApplicationFlags.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various application flags.
/// </summary>
[PublicAPI, Flags]
public enum ApplicationFlags
{
    /// <summary>
    /// The application is allowed to receive presence information over the gateway.
    /// </summary>
    GatewayPresence = 1 << 12,

    /// <summary>
    /// The application is allowed to receive limited presence information over the gateway.
    /// </summary>
    GatewayPresenceLimited = 1 << 13,

    /// <summary>
    /// The application is allowed to receive guild members over the gateway.
    /// </summary>
    GatewayGuildMembers = 1 << 14,

    /// <summary>
    /// The application is allowed to receive limited guild members over the gateway.
    /// </summary>
    GatewayGuildMembersLimited = 1 << 15,

    /// <summary>
    /// The application is currently pending verification and has hit the guild limit.
    /// </summary>
    VerificationPendingGuildLimit = 1 << 16,

    /// <summary>
    /// The application is embedded.
    /// </summary>
    Embedded = 1 << 17,

    /// <summary>
    /// The application has access to message contents over the gateway.
    /// </summary>
    GatewayMessageContent = 1 << 18,

    /// <summary>
    /// The application's access to message contents over the gateway is limited.
    /// </summary>
    GatewayMessageContentLimited = 1 << 19
}
