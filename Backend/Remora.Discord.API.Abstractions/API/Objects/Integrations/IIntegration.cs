//
//  IIntegration.cs
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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an integration object.
/// </summary>
[PublicAPI]
public interface IIntegration : IPartialIntegration
{
    /// <summary>
    /// Gets the ID of the integration.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the integration.
    /// </summary>
    new string Name { get; }

    /// <summary>
    /// Gets the type of integration.
    /// </summary>
    new string Type { get; }

    /// <summary>
    /// Gets a value indicating whether the integration is enabled.
    /// </summary>
    new bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether the integration is syncing.
    /// </summary>
    new Optional<bool> IsSyncing { get; }

    /// <summary>
    /// Gets the ID of the role that this integration uses for subscribers.
    /// </summary>
    new Optional<Snowflake> RoleID { get; }

    /// <summary>
    /// Gets a value indicating whether emoticons should be synced for this integration (twitch only, currently).
    /// </summary>
    new Optional<bool> EnableEmoticons { get; }

    /// <summary>
    /// Gets the behaviour of expiring subscribers.
    /// </summary>
    new Optional<IntegrationExpireBehaviour> ExpireBehaviour { get; }

    /// <summary>
    /// Gets the grace period (in days) before expiring subscribers.
    /// </summary>
    new Optional<TimeSpan> ExpireGracePeriod { get; }

    /// <summary>
    /// Gets the user for this integration.
    /// </summary>
    new Optional<IUser> User { get; }

    /// <summary>
    /// Gets the integration's account information.
    /// </summary>
    new IIntegrationAccount Account { get; }

    /// <summary>
    /// Gets the time when the integration was last synced.
    /// </summary>
    new Optional<DateTimeOffset> SyncedAt { get; }

    /// <summary>
    /// Gets the number of subscribers this integration has.
    /// </summary>
    new Optional<int> SubscriberCount { get; }

    /// <summary>
    /// Gets a value indicating whether this integration has been revoked.
    /// </summary>
    new Optional<bool> IsRevoked { get; }

    /// <summary>
    /// Gets the bot/OAuth2 application for Discord integrations.
    /// </summary>
    new Optional<IIntegrationApplication> Application { get; }

    /// <summary>
    /// Gets the OAuth2 scopes the integration has been authorized for.
    /// </summary>
    new Optional<IReadOnlyList<string>> Scopes { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialIntegration.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialIntegration.Name => this.Name;

    /// <inheritdoc/>
    Optional<string> IPartialIntegration.Type => this.Type;

    /// <inheritdoc/>
    Optional<bool> IPartialIntegration.IsEnabled => this.IsEnabled;

    /// <inheritdoc/>
    Optional<bool> IPartialIntegration.IsSyncing => this.IsSyncing;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialIntegration.RoleID => this.RoleID;

    /// <inheritdoc/>
    Optional<bool> IPartialIntegration.EnableEmoticons => this.EnableEmoticons;

    /// <inheritdoc/>
    Optional<IntegrationExpireBehaviour> IPartialIntegration.ExpireBehaviour => this.ExpireBehaviour;

    /// <inheritdoc/>
    Optional<TimeSpan> IPartialIntegration.ExpireGracePeriod => this.ExpireGracePeriod;

    /// <inheritdoc/>
    Optional<IUser> IPartialIntegration.User => this.User;

    /// <inheritdoc/>
    Optional<IIntegrationAccount> IPartialIntegration.Account => new(this.Account);

    /// <inheritdoc/>
    Optional<DateTimeOffset> IPartialIntegration.SyncedAt => this.SyncedAt;

    /// <inheritdoc/>
    Optional<int> IPartialIntegration.SubscriberCount => this.SubscriberCount;

    /// <inheritdoc/>
    Optional<bool> IPartialIntegration.IsRevoked => this.IsRevoked;

    /// <inheritdoc/>
    Optional<IIntegrationApplication> IPartialIntegration.Application => this.Application;

    /// <inheritdoc/>
    Optional<IReadOnlyList<string>> IPartialIntegration.Scopes => this.Scopes;
}
