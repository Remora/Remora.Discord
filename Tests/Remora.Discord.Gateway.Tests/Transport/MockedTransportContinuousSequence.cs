//
//  MockedTransportContinuousSequence.cs
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

namespace Remora.Discord.Gateway.Tests.Transport
{
    /// <summary>
    /// Represents a sequence of events.
    /// </summary>
    public class MockedTransportContinuousSequence
    {
        private readonly MockedTransportSequence _sequence;
        private readonly TimeSpan? _timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockedTransportContinuousSequence"/> class.
        /// </summary>
        /// <param name="sequence">The operation sequence.</param>
        /// <param name="timeout">The timeout of the sequence, if any.</param>
        public MockedTransportContinuousSequence(MockedTransportSequence sequence, TimeSpan? timeout = null)
        {
            _sequence = sequence;
            _timeout = timeout;
        }
    }
}
