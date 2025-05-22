//
//  ISubscription.cs
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
/// Represents a user's recurrent payment for at least one SKU.
/// </summary>
[PublicAPI]
public interface ISubscription
{
    /// <summary>
    /// Gets the ID of the subscription.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the subscribed user.
    /// </summary>
    Snowflake UserID { get; }

    /// <summary>
    /// Gets the list of SKUs the user is subscribed to.
    /// </summary>
    IReadOnlyList<Snowflake> SKUIDs { get; }

    /// <summary>
    /// Gets the list of entitlements granted for this subscription.
    /// </summary>
    IReadOnlyList<Snowflake> EntitlementIDs { get; }

    /// <summary>
    /// Gets the list of SKUs that this user will be subscribed to at renewal.
    /// </summary>
    IReadOnlyList<Snowflake>? RenewalSKUIDs { get; }

    /// <summary>
    /// Gets the time at which the current subscription period started.
    /// </summary>
    DateTimeOffset CurrentPeriodStart { get; }

    /// <summary>
    /// Gets the time at which the current subscription period ends.
    /// </summary>
    DateTimeOffset CurrentPeriodEnd { get; }

    /// <summary>
    /// Gets the status of the subscription.
    /// </summary>
    SubscriptionStatus Status { get; }

    /// <summary>
    /// Gets the time at which the subscription was canceled.
    /// </summary>
    DateTimeOffset? CanceledAt { get; }

    /// <summary>
    /// Gets the ISO3166-1-alpha-2 country code of the payment source used to purchase the subscription.
    /// </summary>
    Optional<string> Country { get; }
}
