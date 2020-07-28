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
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.Gateway.Tests.TestBases;
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

            Assert.True
            (
                connectionResult.IsSuccess,
                connectionResult.IsSuccess ? string.Empty : connectionResult.ErrorReason
            );
        }
    }
}
