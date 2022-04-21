//
//  SendResultErrorEvent.cs
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
using Remora.Results;

namespace Remora.Discord.Gateway.Tests.Transport.Events;

/// <summary>
/// Represents a result error returned due to an error in the server-to-client stream of the transport layer.
/// </summary>
public class SendResultErrorEvent : IEvent
{
    private readonly Func<IResultError> _errorFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendResultErrorEvent"/> class.
    /// </summary>
    /// <param name="errorFactory">The result error factory function.</param>
    public SendResultErrorEvent(Func<IResultError> errorFactory)
    {
        _errorFactory = errorFactory;
    }

    /// <summary>
    /// Creates the exception that should be returned.
    /// </summary>
    /// <returns>The exception.</returns>
    public IResultError CreateResultError() => _errorFactory();
}
