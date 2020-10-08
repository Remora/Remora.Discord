//
//  MockedTransportSequenceBuilder.cs
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
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;

namespace Remora.Discord.Gateway.Tests.Transport
{
    /// <summary>
    /// Builds an action sequence for a mocked transport service.
    /// </summary>
    public class MockedTransportSequenceBuilder
    {
        /// <summary>
        /// Expects a connection to the following URI.
        /// </summary>
        /// <param name="connectionUri">The connection URI.</param>
        /// <returns>The builder, with the expectation.</returns>
        public MockedTransportSequenceBuilder ExpectConnection(Uri connectionUri)
        {
            return this;
        }

        /// <summary>
        /// Expects a connection to the following URI.
        /// </summary>
        /// <returns>The builder, with the expectation.</returns>
        public MockedTransportSequenceBuilder ExpectDisconnect()
        {
            return this;
        }

        /// <summary>
        /// Adds an expected incoming payload.
        /// </summary>
        /// <param name="expectation">A predicate that matches the expected payload.</param>
        /// <typeparam name="TExpected">The expected type.</typeparam>
        /// <returns>The action builder, with the expectation.</returns>
        public MockedTransportSequenceBuilder Expect<TExpected>(Func<TExpected, bool>? expectation = null)
            where TExpected : IGatewayCommand
        {
            return this;
        }

        /// <summary>
        /// Adds a sent payload.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        /// <typeparam name="TResponse">The type of the payload to send.</typeparam>
        /// <returns>The action builder, with the payload.</returns>
        public MockedTransportSequenceBuilder Send<TResponse>(TResponse payload)
            where TResponse : IGatewayEvent
        {
            return this;
        }

        /// <summary>
        /// Finishes the sequence, signalling the cancellation token source that it should stop.
        /// </summary>
        /// <param name="tokenSource">The token source.</param>
        /// <returns>The sequence builder, with the finalizer.</returns>
        public MockedTransportSequenceBuilder Finish(CancellationTokenSource tokenSource)
        {
            return this;
        }

        /// <summary>
        /// Builds the transport sequence.
        /// </summary>
        /// <returns>The transport sequence.</returns>
        public MockedTransportSequence Build() => new MockedTransportSequence();
    }
}
