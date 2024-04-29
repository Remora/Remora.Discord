//
//  IDiscordRestMonetizationAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Monetization API.
/// </summary>
[PublicAPI]
public interface IDiscordRestMonetizationAPI
{
    /// <summary>
    /// Gets all entitlements for a given application.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="userID">The ID of the user to limit the search to.</param>
    /// <param name="skuIDs">The SKUs to limit the search to.</param>
    /// <param name="before">The entitlement to search before.</param>
    /// <param name="after">The entitlement to search after.</param>
    /// <param name="limit">The maximum number of entitlements to return (1-100). Defaults to 100.</param>
    /// <param name="guildID">The ID of the guild to limit the search to.</param>
    /// <param name="excludeEnded">Whether to exclude expired entitlements.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The entitlements.</returns>
    Task<Result<IReadOnlyList<IEntitlement>>> ListEntitlementsAsync
    (
        Snowflake applicationID,
        Optional<Snowflake> userID = default,
        Optional<IReadOnlyList<Snowflake>> skuIDs = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        Optional<Snowflake> guildID = default,
        Optional<bool> excludeEnded = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Marks the given one-time purchase item as consumed.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="entitlementID">The ID of the entitlement.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A value representing the result of the operation.</returns>
    Task<Result> ConsumeEntitlementAsync
    (
        Snowflake applicationID,
        Snowflake entitlementID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a test entitlement to a given SKU for a given guild or user.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="skuID">The ID of the SKU to grant the entitlement for.</param>
    /// <param name="ownerID">The ID of the guild or user to grant the entitlement to.</param>
    /// <param name="ownerType">The type of the owner.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The test entitlement.</returns>
    Task<Result<IPartialEntitlement>> CreateTestEntitlementAsync
    (
        Snowflake applicationID,
        Snowflake skuID,
        Snowflake ownerID,
        EntitlementOwnerType ownerType,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given test entitlement.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="entitlementID">The ID of the entitlement.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A value representing the result of the operation.</returns>
    Task<Result> DeleteTestEntitlementAsync
    (
        Snowflake applicationID,
        Snowflake entitlementID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets all SKUs for the given application.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The SKUs.</returns>
    Task<Result<IReadOnlyList<ISKU>>> ListSKUsAsync(Snowflake applicationID, CancellationToken ct = default);
}
