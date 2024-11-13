//
//  IntegrationUpdate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Gateway.Events;

/// <inheritdoc cref="IIntegrationUpdate" />
[PublicAPI]
public record IntegrationUpdate
(
    Snowflake ID,
    string Name,
    string Type,
    bool IsEnabled,
    Optional<bool> IsSyncing,
    Optional<Snowflake> RoleID,
    Optional<bool> EnableEmoticons,
    Optional<IntegrationExpireBehaviour> ExpireBehaviour,
    Optional<TimeSpan> ExpireGracePeriod,
    Optional<IUser> User,
    IIntegrationAccount Account,
    Optional<DateTimeOffset> SyncedAt,
    Optional<int> SubscriberCount,
    Optional<bool> IsRevoked,
    Optional<IIntegrationApplication> Application,
    Optional<IReadOnlyList<string>> Scopes,
    Snowflake GuildID
) : IIntegrationUpdate;
