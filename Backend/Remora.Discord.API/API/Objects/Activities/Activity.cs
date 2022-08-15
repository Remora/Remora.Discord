//
//  Activity.cs
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
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IActivity" />
[PublicAPI]
public record Activity
(
    string Name,
    ActivityType Type,
    Optional<Uri?> Url = default,
    Optional<DateTimeOffset> CreatedAt = default,
    Optional<Snowflake> ApplicationID = default,
    Optional<IActivityTimestamps> Timestamps = default,
    Optional<string?> Details = default,
    Optional<string?> State = default,
    Optional<IActivityEmoji?> Emoji = default,
    Optional<IActivityParty> Party = default,
    Optional<IActivityAssets> Assets = default,
    Optional<IActivitySecrets> Secrets = default,
    Optional<bool> Instance = default,
    Optional<ActivityFlags> Flags = default,
    Optional<OneOf<IReadOnlyList<string>, IReadOnlyList<IActivityButton>>> Buttons = default
) : IActivity;
