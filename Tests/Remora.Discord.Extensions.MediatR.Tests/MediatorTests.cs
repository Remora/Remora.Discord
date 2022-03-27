//
//  MediatorTests.cs
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
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.MediatR.Requests;
using Remora.Discord.Extensions.MediatR.Responders;
using Remora.Discord.Extensions.MediatR.Tests.Responders;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Services;
using Remora.Discord.Gateway.Tests.Transport;
using Remora.Discord.Gateway.Transport;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

using GatewayConstants = Remora.Discord.Gateway.Tests.Constants;

namespace Remora.Discord.Extensions.MediatR.Tests
{
#pragma warning disable SA1600, CS1591
    public class MediatorTests
    {
        [Fact]
        public async Task HandlesEvent()
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
                                Assert.Equal(GatewayConstants.MockToken, i?.Token);
                                return true;
                            }
                        )
                        .Send
                        (
                            new Ready
                            (
                                10,
                                GatewayConstants.BotUser,
                                new List<IUnavailableGuild>(),
                                GatewayConstants.MockSessionID,
                                default,
                                new PartialApplication()
                            )
                        )
                        .Send
                        (
                            new MessageCreate
                            (
                                DiscordSnowflake.New(0),
                                DiscordSnowflake.New(1),
                                DiscordSnowflake.New(2),
                                GatewayConstants.BotUser,
                                default,
                                "Hello world!",
                                DateTimeOffset.UtcNow,
                                null,
                                false,
                                false,
                                new List<IUserMention>(),
                                new List<Snowflake>(),
                                default,
                                new List<IAttachment>(),
                                new List<IEmbed>(),
                                new List<IReaction>(),
                                "brr",
                                false,
                                default,
                                MessageType.Default
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

            var handler = new MessageCreateHandler();

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<IGatewayEventRequest<IMessageCreate>>(), It.IsAny<CancellationToken>()))
                .Callback<IGatewayEventRequest<IMessageCreate>, CancellationToken>((request, ct) =>
                    handler.Handle(request, ct));

            var services = new ServiceCollection()
                .AddDiscordGateway(_ => GatewayConstants.MockToken)
                .Replace(transportMockDescriptor)
                .Replace(CreateMockedGatewayAPI())
                .AddSingleton<IResponderTypeRepository, ResponderService>()
                .AddSingleton(typeof(IMediator), mediator)
                .AddResponder<MediatorEventResponder>()
                .BuildServiceProvider(true);

            var client = services.GetRequiredService<DiscordGatewayClient>();
            var runResult = await client.RunAsync(tokenSource.Token);

            ResultAssert.Successful(runResult);
        }

        private static ServiceDescriptor CreateMockedGatewayAPI()
        {
            var gatewayAPIMock = new Mock<IDiscordRestGatewayAPI>();
            gatewayAPIMock
                .Setup(a => a.GetGatewayBotAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IGatewayEndpoint>.FromSuccess(new GatewayEndpoint("wss://gateway.discord.gg/")));

            var gatewayApi = gatewayAPIMock.Object;
            var gatewayAPIMockDescriptor = ServiceDescriptor.Singleton(typeof(IDiscordRestGatewayAPI), gatewayApi);
            return gatewayAPIMockDescriptor;
        }
    }
}
