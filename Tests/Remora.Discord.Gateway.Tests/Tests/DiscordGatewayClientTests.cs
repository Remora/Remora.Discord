//
//  DiscordGatewayClientTests.cs
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
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Services;
using Remora.Discord.Gateway.Tests.Transport;
using Remora.Discord.Gateway.Transport;
using Remora.Discord.Tests;
using Remora.Results;
using Xunit;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
namespace Remora.Discord.Gateway.Tests.Tests;

/// <summary>
/// Tests the <see cref="DiscordGatewayClient"/> class.
/// </summary>
public class DiscordGatewayClientTests
{
    /// <summary>
    /// Tests whether the client can send the correct sequence of events in order to connect normally.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanConnectAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            new List<IUnavailableGuild>(),
                            "mock-session",
                            default,
                            new PartialApplication()
                        )
                    )
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the client can directly resume a session, rather than connecting anew.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanDirectlyResumeAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Resume>
                    (
                        r =>
                        {
                            Assert.Equal(Constants.MockToken, r?.Token);
                            Assert.Equal(Constants.MockSessionID, r?.SessionID);
                            Assert.Equal(Constants.MockLastSequenceNumber, r?.SequenceNumber);
                            return true;
                        }
                    )
                    .Send<Resumed>()
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(Constants.MockSessionID, Constants.MockLastSequenceNumber, tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the client can reconnect and resume properly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanReconnectAndResumeAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            new List<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
                    .Send<Reconnect>()
                    .ExpectDisconnect()
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Resume>
                    (
                        r =>
                        {
                            Assert.Equal(Constants.MockSessionID, r?.SessionID);
                            return true;
                        }
                    )
                    .Send<Resumed>()
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the client can reconnect and create a new session properly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanReconnectAndCreateNewSessionAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            new List<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
                    .Send<Reconnect>()
                    .ExpectDisconnect()
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Resume>
                    (
                        r =>
                        {
                            Assert.Equal(Constants.MockSessionID, r?.SessionID);
                            return true;
                        }
                    )
                    .Send(new InvalidSession(false))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            new List<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the client can retry session creation if the attempt is invalidated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanRetrySessionCreationIfInvalidatedAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new InvalidSession(false)
                    )
                    .ExpectDisconnect()
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            new List<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the gateway can successfully reconnect and re-establish a connection, after a network exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanReconnectAfterExceptionAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            Array.Empty<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
                    .SendException(() => new WebSocketException())
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            Array.Empty<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
                    .SendException(() => new HttpRequestException())
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            Array.Empty<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the gateway can successfully reconnect and resume a connection,
    /// after a network exception that allows resumption.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanResumeAfterExceptionAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            Array.Empty<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
                    .SendResultError(() => new GatewayWebSocketError(WebSocketCloseStatus.ProtocolError))
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Resume>
                    (
                        r =>
                        {
                            Assert.Equal(Constants.MockToken, r?.Token);
                            Assert.Equal(Constants.MockSessionID, r?.SessionID);
                            return true;
                        }
                    )
                    .Send<Resumed>()
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    /// <summary>
    /// Tests whether the gateway can successfully reconnect and resume a connection,
    /// after being asked to reconnect and a subsequent gateway error occurs (presumably
    /// because the gateway we are being asked to transition off is closing).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanResumeAfterReconnectAndExceptionAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var transportMock = new MockedTransportServiceBuilder()
            .IgnoreUnexpected()
            .Sequence
            (
                s => s
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Identify>
                    (
                        i =>
                        {
                            Assert.Equal(Constants.MockToken, i?.Token);
                            return true;
                        }
                    )
                    .Send
                    (
                        new Ready
                        (
                            8,
                            Constants.BotUser,
                            Array.Empty<IUnavailableGuild>(),
                            Constants.MockSessionID,
                            default,
                            new PartialApplication()
                        )
                    )
                    .Send<Reconnect>()
                    .SendResultError(() => new GatewayWebSocketError(WebSocketCloseStatus.ProtocolError))
                    .ExpectConnection(new Uri($"wss://gateway.discord.gg/?v={(int)DiscordAPIVersion.V10}&encoding=json"))
                    .Send(new Hello(TimeSpan.FromMilliseconds(200)))
                    .Expect<Resume>
                    (
                        r =>
                        {
                            Assert.Equal(Constants.MockToken, r?.Token);
                            Assert.Equal(Constants.MockSessionID, r?.SessionID);
                            return true;
                        }
                    )
                    .Send<Resumed>()
            )
            .Continuously
            (
                c => c
                    .Expect<IHeartbeat>()
                    .Send<HeartbeatAcknowledge>()
            )
            .Finish(tokenSource)
            .Build();

        var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

        var services = new ServiceCollection()
            .AddDiscordGateway(_ => Constants.MockToken)
            .Replace(transportMockDescriptor)
            .Replace(CreateMockedGatewayAPI())
            .AddSingleton<IResponderTypeRepository, ResponderService>()
            .BuildServiceProvider(true);

        var client = services.GetRequiredService<DiscordGatewayClient>();
        var runResult = await client.RunAsync(tokenSource.Token);

        ResultAssert.Successful(runResult);
    }

    private static ServiceDescriptor CreateMockedGatewayAPI()
    {
        var gatewayAPIMock = new Mock<IDiscordRestGatewayAPI>();
        gatewayAPIMock.Setup
        (
            a => a.GetGatewayBotAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync
        (
            Result<IGatewayEndpoint>.FromSuccess
            (
                new GatewayEndpoint
                (
                    "wss://gateway.discord.gg/"
                )
            )
        );

        var gatewayApi = gatewayAPIMock.Object;
        var gatewayAPIMockDescriptor = ServiceDescriptor.Singleton(typeof(IDiscordRestGatewayAPI), gatewayApi);
        return gatewayAPIMockDescriptor;
    }
}
