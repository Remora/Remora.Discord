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
using System.Collections.Generic;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Gateway;
using Remora.Discord.Gateway.Tests.Transport.Events;
using Xunit.Sdk;

namespace Remora.Discord.Gateway.Tests.Transport
{
    /// <summary>
    /// Builds an action sequence for a mocked transport service.
    /// </summary>
    public class MockedTransportSequenceBuilder
    {
        private readonly List<IEvent> _sequence = new();

        /// <summary>
        /// Expects a connection to the following URI.
        /// </summary>
        /// <param name="connectionUri">The connection URI.</param>
        /// <returns>The builder, with the expectation.</returns>
        public MockedTransportSequenceBuilder ExpectConnection(Uri? connectionUri = null)
        {
            _sequence.Add
            (
                new ConnectEvent
                (
                    u =>
                    {
                        if (connectionUri is null)
                        {
                            // always passes
                            return EventMatch.Pass;
                        }

                        if (connectionUri != u)
                        {
                            throw new EqualException(connectionUri, u);
                        }

                        return EventMatch.Pass;
                    }
                )
            );

            return this;
        }

        /// <summary>
        /// Expects a connection to the following URI.
        /// </summary>
        /// <returns>The builder, with the expectation.</returns>
        public MockedTransportSequenceBuilder ExpectDisconnect()
        {
            _sequence.Add(new DisconnectEvent());
            return this;
        }

        /// <summary>
        /// Adds an expected incoming payload.
        /// </summary>
        /// <param name="expectation">A predicate that matches the expected payload.</param>
        /// <typeparam name="TExpected">The expected type.</typeparam>
        /// <returns>The action builder, with the expectation.</returns>
        public MockedTransportSequenceBuilder Expect<TExpected>(Func<TExpected?, bool>? expectation = null)
            where TExpected : IGatewayCommand
        {
            _sequence.Add
            (
                new ReceiveEvent
                (
                    (p, ignoreUnexpected) =>
                    {
                        if (p is not IPayload<TExpected> expected)
                        {
                            if (ignoreUnexpected)
                            {
                                return EventMatch.Ignore;
                            }

                            var actualTypename = p.GetType().IsGenericType
                                ? p.GetType().GetGenericArguments()[0].Name
                                : p.GetType().Name;

                            throw new IsTypeException(typeof(TExpected).Name, actualTypename);
                        }

                        if (expectation is null)
                        {
                            return EventMatch.Pass;
                        }

                        return expectation(expected.Data)
                            ? EventMatch.Pass
                            : EventMatch.Fail;
                    }
                )
            );

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
            _sequence.Add(new SendEvent(() => new Payload<TResponse>(payload)));
            return this;
        }

        /// <summary>
        /// Adds a sent payload.
        /// </summary>
        /// <typeparam name="TResponse">The type of the payload to send.</typeparam>
        /// <returns>The action builder, with the payload.</returns>
        public MockedTransportSequenceBuilder Send<TResponse>()
            where TResponse : IGatewayEvent, new()
        {
            _sequence.Add(new SendEvent(() => new Payload<TResponse>(new TResponse())));
            return this;
        }

        /// <summary>
        /// Adds an instruction to throw an exception in the server-to-client transport stream.
        /// </summary>
        /// <param name="exceptionFactory">A factory function for the exception to be thrown.</param>
        /// <returns>The action builder, with the exception.</returns>
        public MockedTransportSequenceBuilder SendException(Func<Exception> exceptionFactory)
        {
            _sequence.Add(new SendExceptionEvent(exceptionFactory));
            return this;
        }

        /// <summary>
        /// Builds the transport sequence.
        /// </summary>
        /// <returns>The transport sequence.</returns>
        public MockedTransportSequence Build() => new(_sequence);
    }
}
