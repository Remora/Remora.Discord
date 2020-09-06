//
//  Integration.cs
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

using System;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Integration : IIntegration
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public bool IsEnabled { get; }

        /// <inheritdoc />
        public bool IsSyncing { get; }

        /// <inheritdoc />
        public Snowflake RoleID { get; }

        /// <inheritdoc />
        public Optional<bool> EnableEmoticons { get; }

        /// <inheritdoc />
        public IntegrationExpireBehaviour ExpireBehaviour { get; }

        /// <inheritdoc />
        public int ExpireGracePeriod { get; }

        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public IAccount Account { get; }

        /// <inheritdoc />
        public DateTime SyncedAt { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Integration"/> class.
        /// </summary>
        /// <param name="id">The ID of the integration.</param>
        /// <param name="name">The name of the integration.</param>
        /// <param name="type">The integration's type.</param>
        /// <param name="isEnabled">Whether the integration is enabled.</param>
        /// <param name="isSyncing">Whether the integration is syncing.</param>
        /// <param name="roleID">The ID of the role the integration is associated with.</param>
        /// <param name="enableEmoticons">Whether emoticons should be synced for this integration.</param>
        /// <param name="expireBehaviour">The behaviour of expiring subscribers.</param>
        /// <param name="expireGracePeriod">The grace period for an expired subscriber.</param>
        /// <param name="user">The user for this integration.</param>
        /// <param name="account">The integration account information.</param>
        /// <param name="syncedAt">The last time when the integration was synced.</param>
        public Integration
        (
            Snowflake id,
            string name,
            string type,
            bool isEnabled,
            bool isSyncing,
            Snowflake roleID,
            Optional<bool> enableEmoticons,
            IntegrationExpireBehaviour expireBehaviour,
            int expireGracePeriod,
            IUser user,
            IAccount account,
            DateTime syncedAt
        )
        {
            this.ID = id;
            this.Name = name;
            this.Type = type;
            this.IsEnabled = isEnabled;
            this.IsSyncing = isSyncing;
            this.RoleID = roleID;
            this.EnableEmoticons = enableEmoticons;
            this.ExpireBehaviour = expireBehaviour;
            this.ExpireGracePeriod = expireGracePeriod;
            this.User = user;
            this.Account = account;
            this.SyncedAt = syncedAt;
        }
    }
}
