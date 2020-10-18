//
//  AuditLog.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class AuditLog : IAuditLog
    {
        /// <inheritdoc />
        public IReadOnlyList<IWebhook> Webhooks { get; }

        /// <inheritdoc />
        public IReadOnlyList<IUser> Users { get; }

        /// <inheritdoc />
        public IReadOnlyList<IAuditLogEntry> AuditLogEntries { get; }

        /// <inheritdoc />
        public IReadOnlyList<IPartialIntegration> Integrations { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLog"/> class.
        /// </summary>
        /// <param name="webhooks">The webhooks in the log.</param>
        /// <param name="users">The users in the log.</param>
        /// <param name="auditLogEntries">The entries in the log.</param>
        /// <param name="integrations">The integrations for the log.</param>
        public AuditLog
        (
            IReadOnlyList<IWebhook> webhooks,
            IReadOnlyList<IUser> users,
            IReadOnlyList<IAuditLogEntry> auditLogEntries,
            IReadOnlyList<IPartialIntegration> integrations
        )
        {
            this.Webhooks = webhooks;
            this.Users = users;
            this.AuditLogEntries = auditLogEntries;
            this.Integrations = integrations;
        }
    }
}
