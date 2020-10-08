//
//  MockedTransportServiceBuilder.cs
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
using Remora.Discord.Gateway.Transport;

namespace Remora.Discord.Gateway.Tests.Transport
{
    /// <summary>
    /// Builds a mocked transport service.
    /// </summary>
    public class MockedTransportServiceBuilder
    {
        private readonly MockedTransportServiceOptions _serviceOptions = new MockedTransportServiceOptions();
        private readonly List<MockedTransportSequence> _sequences = new List<MockedTransportSequence>();

        private readonly List<MockedTransportContinuousSequence> _continuousSequences =
            new List<MockedTransportContinuousSequence>();

        /// <summary>
        /// Sets whether the service should ignore unexpected receivals.
        /// </summary>
        /// <param name="ignoreUnexpected">true if unexpected receivals should be ignored; otherwise, false.</param>
        /// <returns>The builder, with the option configured.</returns>
        public MockedTransportServiceBuilder IgnoreUnexpected(bool ignoreUnexpected = true)
        {
            return this;
        }

        /// <summary>
        /// Adds a continuous expected action to the builder.
        /// </summary>
        /// <param name="continuousBuilder">The builder.</param>
        /// <param name="timeout">The timeout before the continuous action fails, if it has not advanced.</param>
        /// <returns>The builder, with the action.</returns>
        public MockedTransportServiceBuilder Continuously
        (
            Action<MockedTransportSequenceBuilder> continuousBuilder,
            TimeSpan? timeout = null
        )
        {
            return this;
        }

        /// <summary>
        /// Adds a single sequential expected action to the builder.
        /// </summary>
        /// <param name="sequenceBuilder">The builder.</param>
        /// <returns>The builder, with the action.</returns>
        public MockedTransportServiceBuilder Sequence
        (
            Action<MockedTransportSequenceBuilder> sequenceBuilder
        )
        {
            return this;
        }

        /// <summary>
        /// Builds the transport service.
        /// </summary>
        /// <returns>The transport service.</returns>
        public IPayloadTransportService Build() => new MockedTransportService
        (
            _sequences,
            _continuousSequences,
            _serviceOptions
        );
    }
}
