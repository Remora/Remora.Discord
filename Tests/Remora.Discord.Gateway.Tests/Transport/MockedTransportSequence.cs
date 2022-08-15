//
//  MockedTransportSequence.cs
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
using System.Collections;
using System.Collections.Generic;
using Remora.Discord.Gateway.Tests.Transport.Events;

namespace Remora.Discord.Gateway.Tests.Transport;

/// <summary>
/// Represents a sequence of events.
/// </summary>
public class MockedTransportSequence : IEnumerator<IEvent>
{
    private readonly IEnumerator<IEvent> _currentState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockedTransportSequence"/> class.
    /// </summary>
    /// <param name="sequence">The event sequence.</param>
    public MockedTransportSequence(IEnumerable<IEvent> sequence)
    {
        _currentState = sequence.GetEnumerator();
        _currentState.MoveNext();
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        return _currentState.MoveNext();
    }

    /// <inheritdoc />
    public void Reset()
    {
        _currentState.Reset();
    }

    /// <inheritdoc />
    public IEvent Current => _currentState.Current;

    /// <inheritdoc/>
    object? IEnumerator.Current => ((IEnumerator)_currentState).Current;

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _currentState.Dispose();
    }
}
