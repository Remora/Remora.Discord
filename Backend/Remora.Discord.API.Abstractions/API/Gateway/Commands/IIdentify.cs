//
//  IIdentify.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Commands;

/// <summary>
/// Represents an identification command sent to the Discord gateway.
/// </summary>
[PublicAPI]
public interface IIdentify : IGatewayCommand
{
    /// <summary>
    /// Gets the authentication token.
    /// </summary>
    string Token { get; }

    /// <summary>
    /// Gets the connection properties.
    /// </summary>
    IConnectionProperties Properties { get; }

    /// <summary>
    /// Gets an optional field, containing a value that indicates whether the connection supports compressed
    /// packets.
    /// </summary>
    Optional<bool> Compress { get; }

    /// <summary>
    /// Gets an optional field, containing the threshold value of total guild members before a guild is considered
    /// large, and offline members will not automatically be sent.
    /// </summary>
    Optional<byte> LargeThreshold { get; }

    /// <summary>
    /// Gets an optional field, containing the sharding ID for this connection.
    /// </summary>
    Optional<IShardIdentification> Shard { get; }

    /// <summary>
    /// Gets an optional field, containing initial presence information.
    /// </summary>
    Optional<IUpdatePresence> Presence { get; }

    /// <summary>
    /// Gets an optional field, containing the gateway intents the connection wants to receive.
    /// </summary>
    GatewayIntents Intents { get; }
}
