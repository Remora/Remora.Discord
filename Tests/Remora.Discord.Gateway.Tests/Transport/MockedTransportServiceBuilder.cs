//
//  MockedTransportServiceBuilder.cs
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
using System.Threading;
using JetBrains.Annotations;
using Remora.Discord.Gateway.Transport;
using Xunit.Abstractions;

namespace Remora.Discord.Gateway.Tests.Transport;

/// <summary>
/// Builds a mocked transport service.
/// </summary>
[PublicAPI]
public class MockedTransportServiceBuilder
{
    private readonly ITestOutputHelper _testOutput;
    private readonly MockedTransportServiceOptions _serviceOptions = new();
    private readonly List<MockedTransportSequence> _sequences = new();

    private readonly List<MockedTransportSequence> _continuousSequences = new();

    private CancellationTokenSource _finisher = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MockedTransportServiceBuilder"/> class.
    /// </summary>
    /// <param name="testOutput">The test output helper.</param>
    public MockedTransportServiceBuilder(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    /// <summary>
    /// Sets a timeout for the service. If no sequences have advanced within this timeout, the service will
    /// terminate and throw.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The builder, with the timeout.</returns>
    public MockedTransportServiceBuilder WithTimeout(TimeSpan timeout)
    {
        _serviceOptions.Timeout = timeout;
        return this;
    }

    /// <summary>
    /// Sets whether the service should ignore unexpected receivals.
    /// </summary>
    /// <param name="ignoreUnexpected">true if unexpected receivals should be ignored; otherwise, false.</param>
    /// <returns>The builder, with the option configured.</returns>
    public MockedTransportServiceBuilder IgnoreUnexpected(bool ignoreUnexpected = true)
    {
        _serviceOptions.IgnoreUnexpected = ignoreUnexpected;
        return this;
    }

    /// <summary>
    /// Adds a continuous expected action to the builder.
    /// </summary>
    /// <param name="continuousBuilder">The builder.</param>
    /// <returns>The builder, with the action.</returns>
    public MockedTransportServiceBuilder Continuously
    (
        Action<MockedTransportSequenceBuilder> continuousBuilder
    )
    {
        var sequenceBuilderInstance = new MockedTransportSequenceBuilder();
        continuousBuilder(sequenceBuilderInstance);

        _continuousSequences.Add(sequenceBuilderInstance.Build());
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
        var sequenceBuilderInstance = new MockedTransportSequenceBuilder();
        sequenceBuilder(sequenceBuilderInstance);

        _sequences.Add(sequenceBuilderInstance.Build());
        return this;
    }

    /// <summary>
    /// Finishes the service, signalling the cancellation token source that it should stop.
    /// </summary>
    /// <param name="tokenSource">The token source.</param>
    /// <returns>The sequence builder, with the finalizer.</returns>
    public MockedTransportServiceBuilder Finish(CancellationTokenSource tokenSource)
    {
        _finisher = tokenSource;
        return this;
    }

    /// <summary>
    /// Builds the transport service.
    /// </summary>
    /// <returns>The transport service.</returns>
    public IPayloadTransportService Build() => new MockedTransportService
    (
        _testOutput,
        _sequences,
        _continuousSequences,
        _serviceOptions,
        _finisher
    );
}
