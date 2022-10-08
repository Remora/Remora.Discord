//
//  IVoiceState.cs
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
/// Represents a user's voice connection status.
/// </summary>
[PublicAPI]
public interface IVoiceState : IPartialVoiceState
{
    /// <summary>
    /// Gets the guild ID this voice state is for.
    /// </summary>
    new Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the channel ID this user is connected to.
    /// </summary>
    new Snowflake? ChannelID { get; }

    /// <summary>
    /// Gets the user ID this voice state is for.
    /// </summary>
    new Snowflake UserID { get; }

    /// <summary>
    /// Gets the guild member this voice state is for.
    /// </summary>
    new Optional<IGuildMember> Member { get; }

    /// <summary>
    /// Gets the session ID for this voice state.
    /// </summary>
    new string SessionID { get; }

    /// <summary>
    /// Gets a value indicating whether the user is deafened by the server.
    /// </summary>
    new bool IsDeafened { get; }

    /// <summary>
    /// Gets a value indicating whether the user is muted by the server.
    /// </summary>
    new bool IsMuted { get; }

    /// <summary>
    /// Gets a value indicating whether the user is locally deafened.
    /// </summary>
    new bool IsSelfDeafened { get; }

    /// <summary>
    /// Gets a value indicating whether the user is locally muted.
    /// </summary>
    new bool IsSelfMuted { get; }

    /// <summary>
    /// Gets a value indicating whether the user is currently streaming using "Go Live".
    /// </summary>
    new Optional<bool> IsStreaming { get; }

    /// <summary>
    /// Gets a value indicating whether the user's camera is enabled.
    /// </summary>
    new bool IsVideoEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether the user is muted by the current user.
    /// </summary>
    new bool IsSuppressed { get; }

    /// <summary>
    /// Gets the time at which the user requested to speak.
    /// </summary>
    new DateTimeOffset? RequestToSpeakTimestamp { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialVoiceState.GuildID => this.GuildID;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialVoiceState.ChannelID => this.ChannelID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialVoiceState.UserID => this.UserID;

    /// <inheritdoc/>
    Optional<IGuildMember> IPartialVoiceState.Member => this.Member;

    /// <inheritdoc/>
    Optional<string> IPartialVoiceState.SessionID => this.SessionID;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsDeafened => this.IsDeafened;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsMuted => this.IsMuted;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsSelfDeafened => this.IsSelfDeafened;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsSelfMuted => this.IsSelfMuted;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsStreaming => this.IsStreaming;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsVideoEnabled => this.IsVideoEnabled;

    /// <inheritdoc/>
    Optional<bool> IPartialVoiceState.IsSuppressed => this.IsSuppressed;

    /// <inheritdoc/>
    Optional<DateTimeOffset?> IPartialVoiceState.RequestToSpeakTimestamp => this.RequestToSpeakTimestamp;
}
