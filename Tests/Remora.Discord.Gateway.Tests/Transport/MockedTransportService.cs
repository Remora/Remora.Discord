//
//  MockedTransportService.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Gateway.Tests.Transport.Events;
using Remora.Discord.Gateway.Transport;
using Remora.Results;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Remora.Discord.Gateway.Tests.Transport;

/// <summary>
/// Represents a mocked transport service.
/// </summary>
public class MockedTransportService : IPayloadTransportService
{
    private readonly ITestOutputHelper _testOutput;
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
    /// <param name="testOutput">The test output helper.</param>
    /// <param name="sequences">The sequences in use.</param>
    /// <param name="continuousSequences">The continuous sequences in use.</param>
    /// <param name="serviceOptions">The service options.</param>
    /// <param name="finisher">The token source to cancel when all sequences are finished.</param>
    public MockedTransportService
    (
        ITestOutputHelper testOutput,
        IReadOnlyList<MockedTransportSequence> sequences,
        IReadOnlyList<MockedTransportSequence> continuousSequences,
        MockedTransportServiceOptions serviceOptions,
        CancellationTokenSource finisher
    )
    {
        _testOutput = testOutput;
        _sequences = sequences;
        _continuousSequences = continuousSequences;
        _serviceOptions = serviceOptions;
        _finisher = finisher;

        _lastAdvance = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc />
    public async Task<Result> ConnectAsync(Uri endpoint, CancellationToken ct = default)
    {
        try
        {
            await _semaphore.WaitAsync(ct);
            var sequenceAdvanced = false;

            foreach (var sequence in _sequences.Except(_finishedSequences))
            {
                if (sequence.Current is not ConnectEvent c)
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
                        sequenceAdvanced = true;

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
                        throw new ArgumentOutOfRangeException(nameof(endpoint));
                    }
                }
            }

            if (!sequenceAdvanced)
            {
                foreach (var continuousSequence in _continuousSequences)
                {
                    if (continuousSequence.Current is not ConnectEvent c)
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
                            throw new ArgumentOutOfRangeException(nameof(endpoint));
                        }
                    }
                }
            }

            var remainingSequences = _sequences.Except(_finishedSequences).ToList();
            if (remainingSequences.Count == 0)
            {
                _finisher.Cancel();
            }

            if (!sequenceAdvanced)
            {
                CheckTimeout();
            }

            this.IsConnected = true;
            return Result.FromSuccess();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<Result> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
    {
        try
        {
            await _semaphore.WaitAsync(ct);
            var sequenceAdvanced = false;
            var hadExpectedEvent = false;

            foreach (var sequence in _sequences.Except(_finishedSequences))
            {
                if (sequence.Current is not ReceiveEvent r)
                {
                    continue;
                }

                switch (r.Matches(payload, true))
                {
                    case EventMatch.Pass:
                    {
                        if (!sequence.MoveNext())
                        {
                            _finishedSequences.Add(sequence);
                        }

                        _lastAdvance = DateTimeOffset.UtcNow;
                        sequenceAdvanced = true;
                        hadExpectedEvent = true;

                        break;
                    }
                    case EventMatch.Fail:
                    {
                        throw new TrueException("An event in a sequence failed.", null);
                    }
                    case EventMatch.Ignore:
                    {
                        hadExpectedEvent = false;
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(payload));
                    }
                }
            }

            if (!sequenceAdvanced)
            {
                foreach (var continuousSequence in _continuousSequences)
                {
                    if (continuousSequence.Current is not ReceiveEvent r)
                    {
                        // TODO: not sure about this one... more testing required.
                        hadExpectedEvent = true;
                        continue;
                    }

                    switch (r.Matches(payload, true))
                    {
                        case EventMatch.Pass:
                        {
                            if (!continuousSequence.MoveNext())
                            {
                                continuousSequence.Reset();
                            }

                            hadExpectedEvent = true;

                            break;
                        }
                        case EventMatch.Fail:
                        {
                            throw new TrueException("An event in a continuous sequence failed.", null);
                        }
                        case EventMatch.Ignore:
                        {
                            hadExpectedEvent = false;
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException(nameof(payload));
                        }
                    }
                }
            }

            if (!hadExpectedEvent && !_serviceOptions.IgnoreUnexpected)
            {
                throw new IsTypeException("[sequence]", payload.GetType().ToString());
            }

            var remainingSequences = _sequences.Except(_finishedSequences).ToList();
            if (remainingSequences.Count == 0)
            {
                _finisher.Cancel();
            }

            if (!sequenceAdvanced)
            {
                CheckTimeout();
            }

            return Result.FromSuccess();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<Result<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await _semaphore.WaitAsync(ct);
                var sequenceAdvanced = false;

                IPayload? payload = null;

                foreach (var sequence in _sequences.Except(_finishedSequences))
                {
                    switch (sequence.Current)
                    {
                        case SendEvent s:
                        {
                            payload = s.CreatePayload();
                            if (!sequence.MoveNext())
                            {
                                _finishedSequences.Add(sequence);
                            }

                            _lastAdvance = DateTimeOffset.UtcNow;
                            sequenceAdvanced = true;
                            break;
                        }
                        case SendExceptionEvent se:
                        {
                            if (!sequence.MoveNext())
                            {
                                _finishedSequences.Add(sequence);
                            }

                            this.IsConnected = false;
                            throw se.CreateException();
                        }
                    }

                    if (payload is not null)
                    {
                        break;
                    }
                }

                if (!sequenceAdvanced)
                {
                    foreach (var continuousSequence in _continuousSequences)
                    {
                        if (continuousSequence.Current is not SendEvent s)
                        {
                            continue;
                        }

                        payload = s.CreatePayload();
                        if (!continuousSequence.MoveNext())
                        {
                            continuousSequence.Reset();
                        }

                        break;
                    }
                }

                var remainingSequences = _sequences.Except(_finishedSequences).ToList();
                if (remainingSequences.Count == 0)
                {
                    _finisher.Cancel();
                }

                if (!sequenceAdvanced)
                {
                    CheckTimeout();
                }

                if (payload is not null)
                {
                    return Result<IPayload>.FromSuccess(payload);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10), ct);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return await Task.FromCanceled<Result<IPayload>>(ct);
    }

    /// <inheritdoc />
    public async Task<Result> DisconnectAsync
    (
        bool reconnectionIntended,
        CancellationToken ct = default
    )
    {
        try
        {
            await _semaphore.WaitAsync(ct);
            var sequenceAdvanced = false;

            foreach (var sequence in _sequences.Except(_finishedSequences))
            {
                if (sequence.Current is not DisconnectEvent d)
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
                        sequenceAdvanced = true;

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
                        throw new ArgumentOutOfRangeException(nameof(d));
                    }
                }
            }

            if (!sequenceAdvanced)
            {
                foreach (var continuousSequence in _continuousSequences)
                {
                    if (continuousSequence.Current is not DisconnectEvent d)
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
                            throw new ArgumentOutOfRangeException(nameof(d));
                        }
                    }
                }
            }

            var remainingSequences = _sequences.Except(_finishedSequences).ToList();
            if (remainingSequences.Count == 0)
            {
                _finisher.Cancel();
            }

            if (!sequenceAdvanced)
            {
                CheckTimeout();
            }

            this.IsConnected = false;
            return Result.FromSuccess();
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
            // timeout += TimeSpan.FromMinutes(10);
        }

        if (DateTimeOffset.UtcNow - _lastAdvance <= timeout)
        {
            return;
        }

        var pendingEvents = _sequences.Select(s => s.Current.ToString()).Humanize();
        _testOutput.WriteLine($"Timed out waiting for {pendingEvents}");

        throw new TestTimeoutException((int)_serviceOptions.Timeout.TotalMilliseconds);
    }
}
