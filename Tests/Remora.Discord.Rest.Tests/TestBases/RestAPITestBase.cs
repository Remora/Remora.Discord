//
//  RestAPITestBase.cs
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
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Unstable.Extensions;
using RichardSzalay.MockHttp;

namespace Remora.Discord.Rest.Tests.TestBases;

/// <summary>
/// Serves as a base class for REST API tests.
/// </summary>
/// <typeparam name="TAPI">The API type.</typeparam>
public abstract class RestAPITestBase<TAPI> where TAPI : notnull
{
    /// <summary>
    /// Creates a configured, mocked API instance.
    /// </summary>
    /// <param name="builder">The mock builder.</param>
    /// <returns>The API instance.</returns>
    protected TAPI CreateAPI(Action<MockHttpMessageHandler> builder)
    {
        var services = CreateConfiguredAPIServices(builder);
        return services.GetRequiredService<TAPI>();
    }

    /// <summary>
    /// Creates a configured service provider with the given HTTP mock settings.
    /// </summary>
    /// <param name="builder">The HTTP mock builder.</param>
    /// <returns>The configured services.</returns>
    protected IServiceProvider CreateConfiguredAPIServices(Action<MockHttpMessageHandler> builder)
    {
        var serviceContainer = new ServiceCollection()
            .AddDiscordRest
            (
                _ => ("TEST_TOKEN", DiscordTokenType.Bot),
                b => b.ConfigurePrimaryHttpMessageHandler
                (
                    _ =>
                    {
                        var mockHandler = new MockHttpMessageHandler();
                        builder(mockHandler);

                        return mockHandler;
                    }
                )
            )
            .AddExperimentalDiscordApi()
            .BuildServiceProvider(true);

        return serviceContainer;
    }
}
