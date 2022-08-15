//
//  UserUpdate.cs
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

using System.Drawing;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Gateway.Events;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Gateway.Events.IUserUpdate" />
[PublicAPI]
public record UserUpdate
(
    Snowflake ID,
    string Username,
    ushort Discriminator,
    IImageHash? Avatar,
    Optional<bool> IsBot = default,
    Optional<bool> IsSystem = default,
    Optional<bool> IsMFAEnabled = default,
    Optional<IImageHash?> Banner = default,
    Optional<Color?> AccentColour = default,
    Optional<string> Locale = default,
    Optional<bool> IsVerified = default,
    Optional<string?> Email = default,
    Optional<UserFlags> Flags = default,
    Optional<PremiumType> PremiumType = default,
    Optional<UserFlags> PublicFlags = default
) : IUserUpdate;
