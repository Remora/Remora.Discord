//
//  InteractionResponderTestBase.cs
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Responders;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.TestBases;

/// <summary>
/// Tests the command responder.
/// </summary>
public abstract class InteractionResponderTestBase : IDisposable
{
    private readonly IServiceScope _scope;

    /// <summary>
    /// Gets the mocked <see cref="IDiscordRestInteractionAPI"/>.
    /// </summary>
    protected Mock<IDiscordRestInteractionAPI> MockInteractionApi { get; }

    /// <summary>
    /// Gets the responder under test.
    /// </summary>
    protected InteractionResponder Responder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionResponderTestBase"/> class.
    /// </summary>
    protected InteractionResponderTestBase()
    {
        this.MockInteractionApi = new Mock<IDiscordRestInteractionAPI>();
        this.MockInteractionApi.Setup
            (
                i => i.CreateInteractionResponseAsync
                (
                    It.IsAny<Snowflake>(),
                    It.IsAny<string>(),
                    It.IsAny<IInteractionResponse>(),
                    It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(Result.FromSuccess()));

        var serviceCollection = new ServiceCollection()
            .AddSingleton(this.MockInteractionApi.Object)
            .AddDiscordCommands(true);

        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureServices(serviceCollection);

        var services = serviceCollection.BuildServiceProvider(true);

        _scope = services.CreateScope();
        this.Responder = _scope.ServiceProvider.GetRequiredService<InteractionResponder>();
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
