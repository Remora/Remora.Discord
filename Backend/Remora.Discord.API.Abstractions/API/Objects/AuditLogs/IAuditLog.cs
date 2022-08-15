//
//  IAuditLog.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an audit log page.
/// </summary>
[PublicAPI]
public interface IAuditLog
{
    /// <summary>
    /// Gets a list of audit log entries.
    /// </summary>
    IReadOnlyList<IAuditLogEntry> AuditLogEntries { get; }

    /// <summary>
    /// Gets a list of scheduled events found in the audit log.
    /// </summary>
    IReadOnlyList<IGuildScheduledEvent> GuildScheduledEvents { get; }

    /// <summary>
    /// Gets a list of partial integration objects found in the audit log.
    /// </summary>
    IReadOnlyList<IPartialIntegration> Integrations { get; }

    /// <summary>
    /// Gets a list of threads found in the audit log.
    /// </summary>
    IReadOnlyList<IChannel> Threads { get; }

    /// <summary>
    /// Gets a list of users found in the audit log.
    /// </summary>
    IReadOnlyList<IUser> Users { get; }

    /// <summary>
    /// Gets a list of webhooks found in the audit log.
    /// </summary>
    IReadOnlyList<IWebhook> Webhooks { get; }
}
