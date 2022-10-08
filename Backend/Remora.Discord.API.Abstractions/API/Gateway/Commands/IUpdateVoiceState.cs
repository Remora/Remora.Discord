//
//  IUpdateVoiceState.cs
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
/// Represents a command to update the voice state of the client.
/// </summary>
[PublicAPI]
public interface IUpdateVoiceState : IGatewayCommand
{
    /// <summary>
    /// Gets the guild that the status should be updated in.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the channel the client wants to join, or null if disconnecting.
    /// </summary>
    Snowflake? ChannelID { get; }

    /// <summary>
    /// Gets a value indicating whether the client is muted.
    /// </summary>
    bool IsSelfMuted { get; }

    /// <summary>
    /// Gets a value indicating whether the client is deafened.
    /// </summary>
    bool IsSelfDeafened { get; }
}
