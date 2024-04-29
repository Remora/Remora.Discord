//
//  IPartialEntitlement.cs
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
/// Represents partial information about a user's access to monetized application features.
/// </summary>
[PublicAPI]
public interface IPartialEntitlement
{
    /// <inheritdoc cref="IEntitlement.ID"/>
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IEntitlement.SKUID"/>
    Optional<Snowflake> SKUID { get; }

    /// <inheritdoc cref="IEntitlement.ApplicationID"/>
    Optional<Snowflake> ApplicationID { get; }

    /// <inheritdoc cref="IEntitlement.UserID"/>
    Optional<Snowflake> UserID { get; }

    /// <inheritdoc cref="IEntitlement.Type"/>
    Optional<EntitlementType> Type { get; }

    /// <inheritdoc cref="IEntitlement.IsDeleted"/>
    Optional<bool> IsDeleted { get; }

    /// <inheritdoc cref="IEntitlement.StartsAt"/>
    Optional<DateTimeOffset> StartsAt { get; }

    /// <inheritdoc cref="IEntitlement.EndsAt"/>
    Optional<DateTimeOffset> EndsAt { get; }

    /// <inheritdoc cref="IEntitlement.GuildID"/>
    Optional<Snowflake> GuildID { get; }
}
