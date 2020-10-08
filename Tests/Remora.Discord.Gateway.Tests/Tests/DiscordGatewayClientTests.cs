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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Tests.Transport;
using Remora.Discord.Gateway.Transport;
using Xunit;

namespace Remora.Discord.Gateway.Tests.Tests
{
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
                .Sequence
                (
                    s => s
                        .ExpectConnection(new Uri("wss://gateway.discord.gg/?v=8&encoding=json"))
                        .Send(new Hello(1000))
                        .Expect<Identify>(i => i.Token == Constants.MockToken)
                        .Send
                        (
                            new Ready
                            (
                                8,
                                Constants.BotUser,
                                new List<IUnavailableGuild>(),
                                "mock-session",
                                default
                            )
                        )
                        .Finish(tokenSource)
                )
                .Continuously
                (
                    c => c
                        .Expect<IHeartbeat>()
                        .Send(new HeartbeatAcknowledge()),
                    TimeSpan.FromSeconds(1)
                )
                .Build();

            var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

            var services = new ServiceCollection()
                .AddDiscordGateway(() => Constants.MockToken)
                .Replace(transportMockDescriptor)
                .BuildServiceProvider();

            var client = services.GetRequiredService<DiscordGatewayClient>();
            var runResult = await client.RunAsync(tokenSource.Token);

            Assert.True(runResult.IsSuccess);
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
                .Sequence
                (
                    s => s
                        .ExpectConnection(new Uri("wss://gateway.discord.gg/?v=8&encoding=json"))
                        .Send(new Hello(1000))
                        .Expect<Identify>(i => i.Token == Constants.MockToken)
                        .Send
                        (
                            new Ready
                            (
                                8,
                                Constants.BotUser,
                                new List<IUnavailableGuild>(),
                                Constants.MockSessionID,
                                default
                            )
                        )
                        .Send(new Reconnect())
                        .ExpectDisconnect()
                        .ExpectConnection(new Uri("wss://gateway.discord.gg/?v=8&encoding=json"))
                        .Send(new Hello(1000))
                        .Expect<Resume>(r => r.SessionID == Constants.MockSessionID)
                        .Send(new Resumed())
                        .Finish(tokenSource)
                )
                .Continuously
                (
                    c => c
                        .Expect<IHeartbeat>()
                        .Send(new HeartbeatAcknowledge()),
                    TimeSpan.FromSeconds(1)
                )
                .Build();

            var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

            var services = new ServiceCollection()
                .AddDiscordGateway(() => Constants.MockToken)
                .Replace(transportMockDescriptor)
                .BuildServiceProvider();

            var client = services.GetRequiredService<DiscordGatewayClient>();
            var runResult = await client.RunAsync(tokenSource.Token);

            Assert.True(runResult.IsSuccess);
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
                .Sequence
                (
                    s => s
                        .ExpectConnection(new Uri("wss://gateway.discord.gg/?v=8&encoding=json"))
                        .Send(new Hello(1000))
                        .Expect<Identify>(i => i.Token == Constants.MockToken)
                        .Send
                        (
                            new Ready
                            (
                                8,
                                Constants.BotUser,
                                new List<IUnavailableGuild>(),
                                Constants.MockSessionID,
                                default
                            )
                        )
                        .Send(new Reconnect())
                        .ExpectDisconnect()
                        .ExpectConnection(new Uri("wss://gateway.discord.gg/?v=8&encoding=json"))
                        .Send(new Hello(1000))
                        .Expect<Resume>(r => r.SessionID == Constants.MockSessionID)
                        .Send(new InvalidSession(false))
                        .Expect<Identify>(i => i.Token == Constants.MockToken)
                        .Send
                        (
                            new Ready
                            (
                                8,
                                Constants.BotUser,
                                new List<IUnavailableGuild>(),
                                Constants.MockSessionID,
                                default
                            )
                        )
                        .Finish(tokenSource)
                )
                .Continuously
                (
                    c => c
                        .Expect<IHeartbeat>()
                        .Send(new HeartbeatAcknowledge()),
                    TimeSpan.FromSeconds(1)
                )
                .Build();

            var transportMockDescriptor = ServiceDescriptor.Singleton(typeof(IPayloadTransportService), transportMock);

            var services = new ServiceCollection()
                .AddDiscordGateway(() => Constants.MockToken)
                .Replace(transportMockDescriptor)
                .BuildServiceProvider();

            var client = services.GetRequiredService<DiscordGatewayClient>();
            var runResult = await client.RunAsync(tokenSource.Token);

            Assert.True(runResult.IsSuccess);
        }
    }
}
