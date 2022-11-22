//
//  ContextTests.cs
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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remora.Commands.Extensions;
using Remora.Commands.Services;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Commands.Tests.Data.Contexts;
using Remora.Discord.Rest;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Commands.Tests;

/// <summary>
/// Tests injection of various command contexts.
/// </summary>
public class ContextTests
{
    private readonly IServiceProvider _services;
    private readonly CommandService _commands;
    private readonly ContextInjectionService _contextInjection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextTests"/> class.
    /// </summary>
    public ContextTests()
    {
        _services = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCommands()
            .AddCommandTree()
                .WithCommandGroup<GroupWithContext>()
                .WithCommandGroup<GroupWithInteractionContext>()
                .WithCommandGroup<GroupWithMessageContext>()
                .WithCommandGroup<GroupWithInteractionCommandContext>()
                .WithCommandGroup<GroupWithTextCommandContext>()
            .Finish()
            .BuildServiceProvider(true)
            .CreateScope().ServiceProvider;

        _commands = _services.GetRequiredService<CommandService>();
        _contextInjection = _services.GetRequiredService<ContextInjectionService>();
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="ICommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanExecuteCommandFromGroupThatWantsInterfaceContext()
    {
        _contextInjection.Context = Mock.Of<ICommandContext>();

        var result = await _commands.TryExecuteAsync("interface command", _services);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="MessageContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanExecuteCommandFromGroupThatWantsMessageContext()
    {
        _contextInjection.Context = Mock.Of<IMessageContext>();

        var result = await _commands.TryExecuteAsync("message command", _services);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="InteractionContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanExecuteCommandFromGroupThatWantsInteractionContext()
    {
        _contextInjection.Context = Mock.Of<IInteractionContext>();

        var result = await _commands.TryExecuteAsync("interaction command", _services);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="TextCommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanExecuteCommandFromGroupThatWantsTextCommandContext()
    {
        _contextInjection.Context = Mock.Of<ITextCommandContext>();

        var result = await _commands.TryExecuteAsync("text-command command", _services);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="InteractionCommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanExecuteCommandFromGroupThatWantsInteractionCommandContext()
    {
        _contextInjection.Context = Mock.Of<IInteractionCommandContext>();

        var result = await _commands.TryExecuteAsync("interaction-command command", _services);
        ResultAssert.Successful(result);
    }
}
