//
//  LiveDiscordLiveGatewayClientTests.cs
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
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Events;
using Remora.Discord.API.Objects.Messages;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Tests.TestBases;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Gateway.Tests
{
    /// <summary>
    /// Contains live tests for the Discord gateway.
    /// </summary>
    public class LiveDiscordLiveGatewayClientTests : LiveGatewayClientTestBase
    {
        /// <summary>
        /// Tests whether the client can maintain a connection for a number of seconds.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CanMaintainConnection()
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var connectionResult = await this.GatewayClient.RunAsync(tokenSource.Token);

            ResultAssert.Successful(connectionResult);
        }

        /// <summary>
        /// Tests whether the client can respond to a ping-pong request.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanRespond()
        {
            var responder = ActivatorUtilities.CreateInstance<PingPongResponder>(this.Services);

            this.GatewayClient.SubscribeResponder(responder);

            var connectionResult = await this.GatewayClient.RunAsync(responder.TokenSource.Token);

            ResultAssert.Successful(connectionResult);
            Assert.True(responder.DidRespond);
        }

        private class PingPongResponder : IResponder<IMessageCreate>
        {
            private readonly IDiscordRestChannelAPI _channelAPI;

            public CancellationTokenSource TokenSource { get; set; }
                = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            public bool DidRespond { get; set; }

            public PingPongResponder(IDiscordRestChannelAPI channelAPI)
            {
                _channelAPI = channelAPI;
            }

            public async Task<EventResponseResult> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
            {
                if (gatewayEvent.Content != "!ping")
                {
                    return EventResponseResult.FromSuccess();
                }

                var respondResult = await _channelAPI.CreateMessageAsync
                (
                    gatewayEvent.ChannelID,
                    embed: new Embed(description: "Pong!", colour: Color.Purple),
                    ct: ct
                );
                if (!respondResult.IsSuccess)
                {
                    return EventResponseResult.FromError(respondResult);
                }

                this.DidRespond = true;
                this.TokenSource.Cancel();

                return EventResponseResult.FromSuccess();
            }
        }
    }
}
