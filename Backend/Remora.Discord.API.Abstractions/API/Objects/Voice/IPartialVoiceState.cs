//
//  IPartialVoiceState.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a user's partial voice connection status.
    /// </summary>
    [PublicAPI]
    public interface IPartialVoiceState
    {
        /// <summary>
        /// Gets the guild ID this voice state is for.
        /// </summary>
        Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Gets the channel ID this user is connected to.
        /// </summary>
        Optional<Snowflake?> ChannelID { get; }

        /// <summary>
        /// Gets the user ID this voice state is for.
        /// </summary>
        Optional<Snowflake> UserID { get; }

        /// <summary>
        /// Gets the guild member this voice state is for.
        /// </summary>
        Optional<IGuildMember> Member { get; }

        /// <summary>
        /// Gets the session ID for this voice state.
        /// </summary>
        Optional<string> SessionID { get; }

        /// <summary>
        /// Gets a value indicating whether the user is deafened by the server.
        /// </summary>
        Optional<bool> IsDeafened { get; }

        /// <summary>
        /// Gets a value indicating whether the user is muted by the server.
        /// </summary>
        Optional<bool> IsMuted { get; }

        /// <summary>
        /// Gets a value indicating whether the user is locally deafened.
        /// </summary>
        Optional<bool> IsSelfDeafened { get; }

        /// <summary>
        /// Gets a value indicating whether the user is locally muted.
        /// </summary>
        Optional<bool> IsSelfMuted { get; }

        /// <summary>
        /// Gets a value indicating whether the user is currently streaming using "Go Live".
        /// </summary>
        Optional<bool> IsStreaming { get; }

        /// <summary>
        /// Gets a value indicating whether the user's camera is enabled.
        /// </summary>
        Optional<bool> IsVideoEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the user is muted by the current user.
        /// </summary>
        Optional<bool> IsSuppressed { get; }

        /// <summary>
        /// Gets the time at which the user requested to speak.
        /// </summary>
        Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; }
    }
}
