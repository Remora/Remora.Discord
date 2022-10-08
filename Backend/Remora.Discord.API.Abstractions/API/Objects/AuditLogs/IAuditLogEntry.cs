//
//  IAuditLogEntry.cs
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
/// Represents an entry in the audit log.
/// </summary>
[PublicAPI]
public interface IAuditLogEntry
{
    /// <summary>
    /// Gets the ID of the log entry target. Typically, this is a snowflake.
    /// </summary>
    string? TargetID { get; }

    /// <summary>
    /// Gets a list of audit log changes.
    /// </summary>
    Optional<IReadOnlyList<IAuditLogChange>> Changes { get; }

    /// <summary>
    /// Gets the user who made the changes.
    /// </summary>
    Snowflake? UserID { get; }

    /// <summary>
    /// Gets the ID of the entry.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the type of action that occurred.
    /// </summary>
    AuditLogEvent ActionType { get; }

    /// <summary>
    /// Gets additional info for certain action types.
    /// </summary>
    Optional<IOptionalAuditEntryInfo> Options { get; }

    /// <summary>
    /// Gets the reason for the change (0-512 characters).
    /// </summary>
    Optional<string> Reason { get; }
}
