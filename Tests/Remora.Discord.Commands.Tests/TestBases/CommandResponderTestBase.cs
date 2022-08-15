//
//  CommandResponderTestBase.cs
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
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Responders;
using Remora.Discord.Rest;
using Remora.Discord.Rest.Extensions;

namespace Remora.Discord.Commands.Tests.TestBases;

/// <summary>
/// Tests the command responder.
/// </summary>
public abstract class CommandResponderTestBase : IDisposable
{
    private readonly IServiceScope _scope;

    /// <summary>
    /// Gets the responder under test.
    /// </summary>
    protected CommandResponder Responder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandResponderTestBase"/> class.
    /// </summary>
    protected CommandResponderTestBase()
    {
        var serviceCollection = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCommands();

        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureServices(serviceCollection);

        var services = serviceCollection.BuildServiceProvider(true);

        _scope = services.CreateScope();
        this.Responder = _scope.ServiceProvider.GetRequiredService<CommandResponder>();
    }

    /// <summary>
    /// Configures additional required services.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    protected virtual void ConfigureServices(IServiceCollection serviceCollection)
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _scope.Dispose();
    }
}
