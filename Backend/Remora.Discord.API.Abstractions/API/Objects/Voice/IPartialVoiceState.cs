//
//  IPartialVoiceState.cs
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
/// Represents a user's partial voice connection status.
/// </summary>
[PublicAPI]
public interface IPartialVoiceState
{
    /// <inheritdoc cref="IVoiceState.GuildID" />
    Optional<Snowflake> GuildID { get; }

    /// <inheritdoc cref="IVoiceState.ChannelID" />
    Optional<Snowflake?> ChannelID { get; }

    /// <inheritdoc cref="IVoiceState.UserID" />
    Optional<Snowflake> UserID { get; }

    /// <inheritdoc cref="IVoiceState.Member" />
    Optional<IGuildMember> Member { get; }

    /// <inheritdoc cref="IVoiceState.SessionID" />
    Optional<string> SessionID { get; }

    /// <inheritdoc cref="IVoiceState.IsDeafened" />
    Optional<bool> IsDeafened { get; }

    /// <inheritdoc cref="IVoiceState.IsMuted" />
    Optional<bool> IsMuted { get; }

    /// <inheritdoc cref="IVoiceState.IsSelfDeafened" />
    Optional<bool> IsSelfDeafened { get; }

    /// <inheritdoc cref="IVoiceState.IsSelfMuted" />
    Optional<bool> IsSelfMuted { get; }

    /// <inheritdoc cref="IVoiceState.IsStreaming" />
    Optional<bool> IsStreaming { get; }

    /// <inheritdoc cref="IVoiceState.IsVideoEnabled" />
    Optional<bool> IsVideoEnabled { get; }

    /// <inheritdoc cref="IVoiceState.IsSuppressed" />
    Optional<bool> IsSuppressed { get; }

    /// <inheritdoc cref="IVoiceState.RequestToSpeakTimestamp" />
    Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; }
}
