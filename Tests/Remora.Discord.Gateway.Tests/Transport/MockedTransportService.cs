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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Tests.Transport.Events;
using Remora.Discord.Gateway.Transport;
using Xunit.Sdk;

namespace Remora.Discord.Gateway.Tests.Transport
{
    /// <summary>
    /// Represents a mocked transport service.
    /// </summary>
    public class MockedTransportService : IPayloadTransportService
    {
        private readonly IReadOnlyList<MockedTransportSequence> _sequences;
        private readonly IReadOnlyList<MockedTransportSequence> _continuousSequences;
        private readonly MockedTransportServiceOptions _serviceOptions;
        private readonly CancellationTokenSource _finisher;

        private readonly List<MockedTransportSequence> _finishedSequences = new();

        private readonly SemaphoreSlim _semaphore = new(1);

        private DateTimeOffset _lastAdvance;

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockedTransportService"/> class.
        /// </summary>
        /// <param name="sequences">The sequences in use.</param>
        /// <param name="continuousSequences">The continuous sequences in use.</param>
        /// <param name="serviceOptions">The service options.</param>
        /// <param name="finisher">The token source to cancel when all sequences are finished.</param>
        public MockedTransportService
        (
            IReadOnlyList<MockedTransportSequence> sequences,
            IReadOnlyList<MockedTransportSequence> continuousSequences,
            MockedTransportServiceOptions serviceOptions,
            CancellationTokenSource finisher
        )
        {
            _sequences = sequences;
            _continuousSequences = continuousSequences;
            _serviceOptions = serviceOptions;
            _finisher = finisher;

            _lastAdvance = DateTimeOffset.UtcNow;
        }

        /// <inheritdoc />
        public async Task<GatewayConnectionResult> ConnectAsync(Uri endpoint, CancellationToken ct = default)
        {
            try
            {
                await _semaphore.WaitAsync(ct);

                foreach (var sequence in _sequences.Except(_finishedSequences))
                {
                    if (!(sequence.Current is ConnectEvent c))
                    {
                        continue;
                    }

                    switch (c.Matches(endpoint))
                    {
                        case EventMatch.Pass:
                        {
                            if (!sequence.MoveNext())
                            {
                                _finishedSequences.Add(sequence);
                            }

                            _lastAdvance = DateTimeOffset.UtcNow;

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                foreach (var continuousSequence in _continuousSequences)
                {
                    if (!(continuousSequence.Current is ConnectEvent c))
                    {
                        continue;
                    }

                    switch (c.Matches(endpoint))
                    {
                        case EventMatch.Pass:
                        {
                            if (!continuousSequence.MoveNext())
                            {
                                continuousSequence.Reset();
                            }

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a continuous sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                var remainingSequences = _sequences.Except(_finishedSequences).ToList();
                if (remainingSequences.Count == 0)
                {
                    _finisher.Cancel();
                }

                CheckTimeout();

                this.IsConnected = true;
                return GatewayConnectionResult.FromSuccess();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task<SendPayloadResult> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
        {
            try
            {
                await _semaphore.WaitAsync(ct);

                foreach (var sequence in _sequences.Except(_finishedSequences))
                {
                    if (!(sequence.Current is ReceiveEvent r))
                    {
                        continue;
                    }

                    switch (r.Matches(payload, _serviceOptions.IgnoreUnexpected))
                    {
                        case EventMatch.Pass:
                        {
                            if (!sequence.MoveNext())
                            {
                                _finishedSequences.Add(sequence);
                            }

                            _lastAdvance = DateTimeOffset.UtcNow;

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                foreach (var continuousSequence in _continuousSequences)
                {
                    if (!(continuousSequence.Current is ReceiveEvent r))
                    {
                        continue;
                    }

                    switch (r.Matches(payload, _serviceOptions.IgnoreUnexpected))
                    {
                        case EventMatch.Pass:
                        {
                            if (!continuousSequence.MoveNext())
                            {
                                continuousSequence.Reset();
                            }

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a continuous sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                var remainingSequences = _sequences.Except(_finishedSequences).ToList();
                if (remainingSequences.Count == 0)
                {
                    _finisher.Cancel();
                }

                CheckTimeout();

                return SendPayloadResult.FromSuccess();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task<ReceivePayloadResult<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _semaphore.WaitAsync(ct);

                    foreach (var sequence in _sequences.Except(_finishedSequences))
                    {
                        if (!(sequence.Current is SendEvent s))
                        {
                            continue;
                        }

                        var payload = s.CreatePayload();
                        if (!sequence.MoveNext())
                        {
                            _finishedSequences.Add(sequence);
                        }

                        _lastAdvance = DateTimeOffset.UtcNow;

                        return ReceivePayloadResult<IPayload>.FromSuccess(payload);
                    }

                    foreach (var continuousSequence in _continuousSequences)
                    {
                        if (!(continuousSequence.Current is SendEvent s))
                        {
                            continue;
                        }

                        var payload = s.CreatePayload();
                        if (!continuousSequence.MoveNext())
                        {
                            continuousSequence.Reset();
                        }

                        return ReceivePayloadResult<IPayload>.FromSuccess(payload);
                    }

                    var remainingSequences = _sequences.Except(_finishedSequences).ToList();
                    if (remainingSequences.Count == 0)
                    {
                        _finisher.Cancel();
                    }

                    CheckTimeout();

                    await Task.Delay(TimeSpan.FromMilliseconds(10), ct);
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return await Task.FromCanceled<ReceivePayloadResult<IPayload>>(ct);
        }

        /// <inheritdoc />
        public async Task<GatewayConnectionResult> DisconnectAsync
        (
            bool reconnectionIntended,
            CancellationToken ct = default
        )
        {
            try
            {
                await _semaphore.WaitAsync(ct);

                foreach (var sequence in _sequences.Except(_finishedSequences))
                {
                    if (!(sequence.Current is DisconnectEvent d))
                    {
                        continue;
                    }

                    switch (d.Matches())
                    {
                        case EventMatch.Pass:
                        {
                            if (!sequence.MoveNext())
                            {
                                _finishedSequences.Add(sequence);
                            }

                            _lastAdvance = DateTimeOffset.UtcNow;

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                foreach (var continuousSequence in _continuousSequences)
                {
                    if (!(continuousSequence.Current is DisconnectEvent d))
                    {
                        continue;
                    }

                    switch (d.Matches())
                    {
                        case EventMatch.Pass:
                        {
                            if (!continuousSequence.MoveNext())
                            {
                                continuousSequence.Reset();
                            }

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a continuous sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                var remainingSequences = _sequences.Except(_finishedSequences).ToList();
                if (remainingSequences.Count == 0)
                {
                    _finisher.Cancel();
                }

                CheckTimeout();

                this.IsConnected = false;
                return GatewayConnectionResult.FromSuccess();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Checks that the service has advanced in time.
        /// </summary>
        private void CheckTimeout()
        {
            if (_finisher.IsCancellationRequested)
            {
                // We're fine; the sequence is finished (and probably just took some time to process)
                return;
            }

            var timeout = _serviceOptions.Timeout;
            if (Debugger.IsAttached)
            {
                // Extend the timeout
                //timeout += TimeSpan.FromMinutes(10);
            }

            if (DateTimeOffset.UtcNow - _lastAdvance > timeout)
            {
                throw new TestTimeoutException((int)_serviceOptions.Timeout.TotalMilliseconds);
            }
        }
    }
}
