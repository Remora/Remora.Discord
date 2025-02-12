//
//  IEntitlement.cs
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
/// Represents information about a user's access to monetized application features.
/// </summary>
[PublicAPI]
public interface IEntitlement : IPartialEntitlement
{
    /// <summary>
    /// Gets the ID of the entitlement.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the SKU.
    /// </summary>
    new Snowflake SKUID { get; }

    /// <summary>
    /// Gets the ID of the parent application.
    /// </summary>
    new Snowflake ApplicationID { get; }

    /// <summary>
    /// Gets the ID of the user that is granted access to the entitlement's SKU.
    /// </summary>
    new Optional<Snowflake> UserID { get; }

    /// <summary>
    /// Gets the type of the entitlement.
    /// </summary>
    new EntitlementType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the entitlement has been deleted.
    /// </summary>
    new bool IsDeleted { get; }

    /// <summary>
    /// Gets the start time at which the entitlement is valid.
    /// </summary>
    new Optional<DateTimeOffset> StartsAt { get; }

    /// <summary>
    /// Gets the end time at which the entitlement is no longer valid.
    /// </summary>
    new Optional<DateTimeOffset> EndsAt { get; }

    /// <summary>
    /// Gets the ID of the guild that is granted access to the entitlement's SKU.
    /// </summary>
    new Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets a value indicating whether the entitlement has been consumed.
    /// </summary>
    new Optional<bool> IsConsumed { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialEntitlement.ID => this.ID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialEntitlement.SKUID => this.SKUID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialEntitlement.ApplicationID => this.ApplicationID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialEntitlement.UserID => this.UserID;

    /// <inheritdoc/>
    Optional<EntitlementType> IPartialEntitlement.Type => this.Type;

    /// <inheritdoc/>
    Optional<bool> IPartialEntitlement.IsDeleted => this.IsDeleted;

    /// <inheritdoc/>
    Optional<DateTimeOffset> IPartialEntitlement.StartsAt => this.StartsAt;

    /// <inheritdoc/>
    Optional<DateTimeOffset> IPartialEntitlement.EndsAt => this.EndsAt;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialEntitlement.GuildID => this.GuildID;

    /// <inheritdoc/>
    Optional<bool> IPartialEntitlement.IsConsumed => this.IsConsumed;
}
