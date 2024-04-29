//
//  EntitlementType.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various entitlement types.
/// </summary>
[PublicAPI]
public enum EntitlementType
{
    /// <summary>
    /// The entitlement was purchased by a user.
    /// </summary>
    Purchase = 1,

    /// <summary>
    /// The entitlement is a Nitro subscription.
    /// </summary>
    PremiumSubscription = 2,

    /// <summary>
    /// The entitlement was gifted by a developer.
    /// </summary>
    DeveloperGift = 3,

    /// <summary>
    /// The entitlement was purchased by a developer in application test mode.
    /// </summary>
    TestModePurchase = 4,

    /// <summary>
    /// The entitlement was granted when the SKU was free.
    /// </summary>
    FreePurchase = 5,

    /// <summary>
    /// The entitlement was gifted by another user.
    /// </summary>
    UserGift = 6,

    /// <summary>
    /// The entitlement was claimed by a user for free as part of their Nitro subscription.
    /// </summary>
    PremiumPurchase = 7,

    /// <summary>
    /// The entitlement was purchased as an app subscription.
    /// </summary>
    ApplicationSubscription = 8
}
