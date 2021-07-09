//
//  DiscordHttpClientTestBase.cs
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
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Remora.Discord.Rest.Tests.TestBases
{
    /// <summary>
    /// Tests the <see cref="DiscordHttpClient"/> class.
    /// </summary>
    public abstract class DiscordHttpClientTestBase
    {
        /// <summary>
        /// Creates a <see cref="DiscordHttpClient"/> with related mocking rules.
        /// </summary>
        /// <param name="builder">The mock builder.</param>
        /// <returns>The created client.</returns>
        protected DiscordHttpClient CreateClient(Action<MockHttpMessageHandler> builder)
        {
            var serviceCollection = new ServiceCollection();
            var clientBuilder = serviceCollection.AddHttpClient<DiscordHttpClient>("Discord");
            clientBuilder.ConfigurePrimaryHttpMessageHandler
            (
                _ =>
                {
                    var mockHandler = new MockHttpMessageHandler();
                    builder(mockHandler);

                    return mockHandler;
                }
            );

            return serviceCollection.BuildServiceProvider().GetRequiredService<DiscordHttpClient>();
        }
    }
}
