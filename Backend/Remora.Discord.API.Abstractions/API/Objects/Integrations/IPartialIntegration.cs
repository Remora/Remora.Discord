//
//  IPartialIntegration.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a partial integration object.
/// </summary>
[PublicAPI]
public interface IPartialIntegration
{
    /// <inheritdoc cref="IIntegration.ID" />
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IIntegration.Name" />
    Optional<string> Name { get; }

    /// <inheritdoc cref="IIntegration.Type" />
    Optional<string> Type { get; }

    /// <inheritdoc cref="IIntegration.IsEnabled" />
    Optional<bool> IsEnabled { get; }

    /// <inheritdoc cref="IIntegration.IsSyncing" />
    Optional<bool> IsSyncing { get; }

    /// <inheritdoc cref="IIntegration.RoleID" />
    Optional<Snowflake> RoleID { get; }

    /// <inheritdoc cref="IIntegration.EnableEmoticons" />
    Optional<bool> EnableEmoticons { get; }

    /// <inheritdoc cref="IIntegration.ExpireBehaviour" />
    Optional<IntegrationExpireBehaviour> ExpireBehaviour { get; }

    /// <inheritdoc cref="IIntegration.ExpireGracePeriod" />
    Optional<TimeSpan> ExpireGracePeriod { get; }

    /// <inheritdoc cref="IIntegration.User" />
    Optional<IUser> User { get; }

    /// <inheritdoc cref="IIntegration.Account" />
    Optional<IAccount> Account { get; }

    /// <inheritdoc cref="IIntegration.SyncedAt" />
    Optional<DateTimeOffset> SyncedAt { get; }

    /// <inheritdoc cref="IIntegration.SubscriberCount" />
    Optional<int> SubscriberCount { get; }

    /// <inheritdoc cref="IIntegration.IsRevoked" />
    Optional<bool> IsRevoked { get; }

    /// <inheritdoc cref="IIntegration.Application" />
    Optional<IIntegrationApplication> Application { get; }
}
