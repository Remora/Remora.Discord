//
//  AuditLogEntry.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class AuditLogEntry : IAuditLogEntry
    {
        /// <inheritdoc />
        public string? TargetID { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IAuditLogChange>> Changes { get; }

        /// <inheritdoc />
        public Snowflake UserID { get; }

        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public AuditLogEvent ActionType { get; }

        /// <inheritdoc />
        public Optional<IOptionalAuditEntryInfo> Options { get; }

        /// <inheritdoc />
        public Optional<string> Reason { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogEntry"/> class.
        /// </summary>
        /// <param name="targetID">The ID of the affected entity.</param>
        /// <param name="changes">The changes made to the target.</param>
        /// <param name="userID">The ID of the user who made the changes.</param>
        /// <param name="id">The ID of the entry itself.</param>
        /// <param name="actionType">The type of action that occurred.</param>
        /// <param name="options">Additional info for certain action types.</param>
        /// <param name="reason">The reason for the change.</param>
        public AuditLogEntry
        (
            string? targetID,
            Optional<IReadOnlyList<IAuditLogChange>> changes,
            Snowflake userID,
            Snowflake id,
            AuditLogEvent actionType,
            Optional<IOptionalAuditEntryInfo> options,
            Optional<string> reason
        )
        {
            this.TargetID = targetID;
            this.Changes = changes;
            this.UserID = userID;
            this.ID = id;
            this.ActionType = actionType;
            this.Options = options;
            this.Reason = reason;
        }
    }
}
