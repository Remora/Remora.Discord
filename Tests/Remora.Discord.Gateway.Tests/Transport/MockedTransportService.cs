//
//  MockedTransportService.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Transport;

namespace Remora.Discord.Gateway.Tests.Transport
{
    /// <summary>
    /// Represents a mocked transport service.
    /// </summary>
    public class MockedTransportService : IPayloadTransportService
    {
        private readonly IReadOnlyList<MockedTransportSequence> _sequences;
        private readonly IReadOnlyList<MockedTransportContinuousSequence> _continuousSequences;
        private readonly MockedTransportServiceOptions _serviceOptions;

        private readonly ConcurrentQueue<IPayload> _receivedPayloads = new ConcurrentQueue<IPayload>();
        private readonly ConcurrentQueue<IPayload> _payloadsToSend = new ConcurrentQueue<IPayload>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MockedTransportService"/> class.
        /// </summary>
        /// <param name="sequences">The sequences in use.</param>
        /// <param name="continuousSequences">The continuous sequences in use.</param>
        /// <param name="serviceOptions">The service options.</param>
        public MockedTransportService
        (
            IReadOnlyList<MockedTransportSequence> sequences,
            IReadOnlyList<MockedTransportContinuousSequence> continuousSequences,
            MockedTransportServiceOptions serviceOptions
        )
        {
            _sequences = sequences;
            _continuousSequences = continuousSequences;
            _serviceOptions = serviceOptions;
        }

        /// <inheritdoc />
        public Task<GatewayConnectionResult> ConnectAsync(Uri endpoint, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<SendPayloadResult> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested)
            {
                return Task.FromCanceled<SendPayloadResult>(ct);
            }

            _receivedPayloads.Enqueue(payload);
            return Task.FromResult(SendPayloadResult.FromSuccess());
        }

        /// <inheritdoc />
        public async Task<ReceivePayloadResult<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested)
            {
                return await Task.FromCanceled<ReceivePayloadResult<IPayload>>(ct);
            }

            while (!ct.IsCancellationRequested)
            {
                if (_payloadsToSend.TryDequeue(out var payload))
                {
                    return ReceivePayloadResult<IPayload>.FromSuccess(payload);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10), ct);
            }

            return await Task.FromCanceled<ReceivePayloadResult<IPayload>>(ct);
        }

        /// <inheritdoc />
        public Task<GatewayConnectionResult> DisconnectAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
