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
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Extensions;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Unstable.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

#pragma warning disable SA1402

namespace Remora.Discord.Rest.Tests.TestBases;

/// <summary>
/// Serves as a base class for REST API tests.
/// </summary>
/// <typeparam name="TAPI">The API type.</typeparam>
[Collection("REST API tests")]
public abstract class RestAPITestBase<TAPI> where TAPI : notnull
{
    private readonly RestAPITestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestAPITestBase{TAPI}"/> class.
    /// </summary>
    /// <param name="fixture">The test fixture.</param>
    protected RestAPITestBase(RestAPITestFixture fixture)
    {
        _fixture = fixture;
    }

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
            .AddSingleton(_fixture)
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
            .AddSingleton<IOptionsFactory<JsonSerializerOptions>, OptionsFactory<JsonSerializerOptions>>()
            .Decorate<IOptionsFactory<JsonSerializerOptions>, CachedOptionsFactory>()
            .BuildServiceProvider(true);

        return serviceContainer;
    }

    private class CachedOptionsFactory : IOptionsFactory<JsonSerializerOptions>
    {
        private readonly IOptionsFactory<JsonSerializerOptions> _actual;
        private readonly RestAPITestFixture _fixture;

        public CachedOptionsFactory(IOptionsFactory<JsonSerializerOptions> actual, RestAPITestFixture fixture)
        {
            _fixture = fixture;
            _actual = actual;
        }

        public JsonSerializerOptions Create(string name)
        {
            return name is "Discord"
                ? _fixture.Options
                : _actual.Create(name);
        }
    }
}

/// <summary>
/// Acts as a test fixture for REST API tests.
/// </summary>
public class RestAPITestFixture
{
    /// <summary>
    /// Gets a set of JSON serializer options.
    /// </summary>
    public JsonSerializerOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RestAPITestFixture"/> class.
    /// </summary>
    public RestAPITestFixture()
    {
        var services = new ServiceCollection()
            .ConfigureDiscordJsonConverters()
            .AddExperimentalDiscordApi()
            .BuildServiceProvider(true);

        this.Options = services.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>()
            .Get("Discord");
    }
}

/// <summary>
/// Defines a test collection for JSON-backed type tests.
/// </summary>
[CollectionDefinition("REST API tests")]
public class RestAPITestCollection : ICollectionFixture<RestAPITestFixture>;
