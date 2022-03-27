//
//  GatewayHeartbeatData.cs
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
using System.Threading;

namespace Remora.Discord.Gateway;

/// <summary>
/// Represents data relevant to the heart-beating process of the default gateway.
/// </summary>
internal class GatewayHeartbeatData
{
    private long _lastSentTime;
    private long _lastReceivedAckTime;
    private int _lastSequenceNumber;

    /// <summary>
    /// Gets or sets the interval at which a heartbeat should be sent.
    /// </summary>
    /// <remarks>
    /// This property is NOT thread-safe.
    /// </remarks>
    public TimeSpan Interval { get; set; }

    /// <summary>
    /// Gets or sets the time that a heartbeat acknowledgement was last received.
    /// </summary>
    /// <remarks>
    /// This property is thread-safe.
    /// </remarks>
    public DateTimeOffset? LastReceivedAckTime
    {
        get => GetTime(ref _lastReceivedAckTime);
        set => SetTime(ref _lastReceivedAckTime, value);
    }

    /// <summary>
    /// Gets or sets the time that a heartbeat was last sent.
    /// </summary>
    /// <remarks>
    /// This property is thread-safe.
    /// </remarks>
    public DateTimeOffset? LastSentTime
    {
        get => GetTime(ref _lastSentTime);
        set => SetTime(ref _lastSentTime, value);
    }

    /// <summary>
    /// Gets or sets the most recently received sequence number.
    /// </summary>
    public int LastSequenceNumber
    {
        get => _lastSequenceNumber; // 32-bit reads are atomic
        set => Interlocked.Exchange(ref _lastSequenceNumber, value);
    }

    private static DateTimeOffset? GetTime(ref long container)
    {
        var value = Interlocked.Read(ref container);
        return value > 0
            ? new DateTimeOffset(value, TimeSpan.Zero)
            : null;
    }

    private static void SetTime(ref long container, DateTimeOffset? value)
    {
        if (value is null)
        {
            return;
        }

        Interlocked.Exchange(ref container, value.Value.Ticks);
    }
}
