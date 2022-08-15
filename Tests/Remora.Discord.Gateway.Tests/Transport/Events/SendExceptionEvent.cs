//
//  SendExceptionEvent.cs
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

namespace Remora.Discord.Gateway.Tests.Transport.Events;

/// <summary>
/// Represents an exception occurring in server-to-client stream of the transport layer.
/// </summary>
public class SendExceptionEvent : IEvent
{
    private readonly Func<Exception> _exceptionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendExceptionEvent"/> class.
    /// </summary>
    /// <param name="exceptionFactory">The exception factory function.</param>
    public SendExceptionEvent(Func<Exception> exceptionFactory)
    {
        _exceptionFactory = exceptionFactory;
    }

    /// <summary>
    /// Creates the exception that should be sent.
    /// </summary>
    /// <returns>The exception.</returns>
    public Exception CreateException() => _exceptionFactory();
}
