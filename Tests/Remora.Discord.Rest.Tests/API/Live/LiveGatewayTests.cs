//
//  LiveGatewayTests.cs
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

using System.Threading.Tasks;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Live
{
    /// <summary>
    /// Contains tests against the live Discord gateway.
    /// </summary>
    public class LiveGatewayTests : LiveTestBase<IDiscordRestGatewayAPI>
    {
        /// <summary>
        /// Tests whether <see cref="IDiscordRestGatewayAPI.GetGatewayAsync"/> can fetch a gateway object.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CanFetchGateway()
        {
            var result = await this.API.GetGatewayAsync();
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether <see cref="IDiscordRestGatewayAPI.GetGatewayAsync"/> can fetch a gateway object.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CanFetchBotGateway()
        {
            var result = await this.API.GetGatewayBotAsync();
            ResultAssert.Successful(result);
        }
    }
}
