//
//  Connection.cs
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
    public class Connection : IConnection
    {
        /// <inheritdoc/>
        public string ID { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Type { get; }

        /// <inheritdoc/>
        public Optional<bool> IsRevoked { get; }

        /// <inheritdoc/>
        public Optional<IReadOnlyList<IPartialIntegration>> Integrations { get; }

        /// <inheritdoc/>
        public bool IsVerified { get; }

        /// <inheritdoc/>
        public bool IsFriendSyncEnabled { get; }

        /// <inheritdoc/>
        public bool ShouldShowActivity { get; }

        /// <inheritdoc/>
        public ConnectionVisibility Visibility { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="id">The ID of the connection account.</param>
        /// <param name="name">The username of the connection account.</param>
        /// <param name="type">The service of the connection.</param>
        /// <param name="isRevoked">Whether the connection is revoked.</param>
        /// <param name="integrations">The server integrations for this connection.</param>
        /// <param name="isVerified">Whether the connection is verified.</param>
        /// <param name="isFriendSyncEnabled">Whether friend sync is enabled.</param>
        /// <param name="shouldShowActivity">Whether activities related to this connection should be shown.</param>
        /// <param name="visibility">The visibility of the connection.</param>
        public Connection
        (
            string id,
            string name,
            string type,
            Optional<bool> isRevoked,
            Optional<IReadOnlyList<IPartialIntegration>> integrations,
            bool isVerified,
            bool isFriendSyncEnabled,
            bool shouldShowActivity,
            ConnectionVisibility visibility
        )
        {
            this.ID = id;
            this.Name = name;
            this.Type = type;
            this.IsRevoked = isRevoked;
            this.Integrations = integrations;
            this.IsVerified = isVerified;
            this.IsFriendSyncEnabled = isFriendSyncEnabled;
            this.ShouldShowActivity = shouldShowActivity;
            this.Visibility = visibility;
        }
    }
}
