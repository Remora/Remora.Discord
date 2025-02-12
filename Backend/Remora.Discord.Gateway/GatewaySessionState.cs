//
//  GatewaySessionState.cs
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
using System.Threading;

namespace Remora.Discord.Gateway;

/// <summary>
/// Holds internal state fields related to an active session of the gateway. The data in this type is only valid during
/// the time at which its associated session is also valid.
/// </summary>
internal class GatewaySessionState
{
    /// <summary>
    /// Holds the time when the last heartbeat was sent, using <see cref="DateTime.ToBinary"/>.
    /// </summary>
    private long _lastSentHeartbeat;

    /// <summary>
    /// Holds the time when the last event acknowledgement was received, encoded using
    /// <see cref="DateTime.ToBinary()"/>.
    /// </summary>
    private long _lastReceivedEvent;

    /// <summary>
    /// Holds the time when the last heartbeat acknowledgement was received, encoded using
    /// <see cref="DateTime.ToBinary()"/>.
    /// </summary>
    private long _lastReceivedHeartbeatAck;

    /// <summary>
    /// Gets or sets the time at which the last heartbeat was sent.
    /// </summary>
    public DateTime? LastSentHeartbeat
    {
        get => ReadTimeAtomic(ref _lastSentHeartbeat);
        set => WriteTimeAtomic(ref _lastSentHeartbeat, value);
    }

    /// <summary>
    /// Gets or sets the time at which the last event was received.
    /// </summary>
    public DateTime? LastReceivedEvent
    {
        get => ReadTimeAtomic(ref _lastReceivedEvent);
        set => WriteTimeAtomic(ref _lastReceivedEvent, value);
    }

    /// <summary>
    /// Gets or sets the time at which the last heartbeat acknowledgement was received.
    /// </summary>
    public DateTime? LastReceivedHeartbeatAck
    {
        get => ReadTimeAtomic(ref _lastReceivedHeartbeatAck);
        set => WriteTimeAtomic(ref _lastReceivedHeartbeatAck, value);
    }

    private static DateTime? ReadTimeAtomic(ref long field)
    {
        var binary = Interlocked.Read(ref field);
        return binary > 0
            ? DateTime.FromBinary(binary)
            : null;
    }

    private static void WriteTimeAtomic(ref long field, DateTime? value)
    {
        Interlocked.Exchange(ref field, value?.ToBinary() ?? 0);
    }
}
