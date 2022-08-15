//
//  Identify.cs
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
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Gateway.Commands;

/// <summary>
/// Represents an identification command sent to the Discord gateway.
/// </summary>
[PublicAPI]
public record Identify
(
    string Token,
    IConnectionProperties Properties,
    Optional<bool> Compress = default,
    Optional<byte> LargeThreshold = default,
    Optional<IShardIdentification> Shard = default,
    Optional<IUpdatePresence> Presence = default,
    GatewayIntents Intents = default
) : IIdentify;
