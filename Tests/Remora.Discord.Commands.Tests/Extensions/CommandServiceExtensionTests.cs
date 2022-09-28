//
//  CommandServiceExtensionTests.cs
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

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Commands.Results;
using Remora.Commands.Services;
using Remora.Commands.Trees;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Commands.Tests.Data.Contexts;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Tests;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="CommandServiceExtensions"/> class.
/// </summary>
public class CommandServiceExtensionTests
{
    /// <summary>
    /// Tests the <see cref="CommandTreeExtensions.TryPrepareAndExecute"/> method.
    /// </summary>
    public class TryPrepareAndExecuteCommand
    {
        /// <summary>
        /// Tests that the returned result is unsuccessful if the command cannot be preparred (unknown/bad parameters).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ReturnsPreperationErrorIfPreperationFails()
        {
            var services = new ServiceCollection()
                               .AddDiscordGateway(_ => "dummy")
                               .AddDiscordCommands()
                               .AddCommandTree()
                               .WithCommandGroup<GroupWithNodeContext>()
                               .Finish()
                               .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var nodeInjection = services.GetRequiredService<CommandNodeInjectionService>();

            var result = await commandService.TryPrepareAndExecuteCommandAsync
            (
                commandString: "node success",
                services: services,
                treeName: "invalid_tree"
            );

            Assert.Null(nodeInjection.Node.Value);

            ResultAssert.Unsuccessful(result);
            Assert.IsType<TreeNotFoundError>(result.Error);
        }
    }

    /// <summary>
    /// Tests that the returned result is successful if the command is executed successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReturnsSuccessWhenCommandIsSuccessful()
    {
        var services = new ServiceCollection()
                           .AddDiscordGateway(_ => "dummy")
                           .AddDiscordCommands()
                           .AddCommandTree()
                           .WithCommandGroup<GroupWithNodeContext>()
                           .Finish()
                           .BuildServiceProvider();

        var commandService = services.GetRequiredService<CommandService>();
        var nodeInjection = services.GetRequiredService<CommandNodeInjectionService>();

        var result = await commandService.TryPrepareAndExecuteCommandAsync
        (
            commandString: "node success",
            services: services
        );

        Assert.Equal(nodeInjection.Node.Value?.Key, "success");
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests that the returned result is unsuccessful if the command is executed unsuccessfully, and that the returned error is
    /// not wrapped in an unexpected manner.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReturnsCommandErrorWhenCommandFails()
    {
        var services = new ServiceCollection()
                           .AddDiscordGateway(_ => "dummy")
                           .AddDiscordCommands()
                           .AddCommandTree()
                           .WithCommandGroup<GroupWithNodeContext>()
                           .Finish()
                           .BuildServiceProvider();

        var commandService = services.GetRequiredService<CommandService>();
        var nodeInjection = services.GetRequiredService<CommandNodeInjectionService>();

        var result = await commandService.TryPrepareAndExecuteCommandAsync
        (
            commandString: "node failure",
            services: services
        );

        Assert.Equal(nodeInjection.Node.Value?.Key, "failure");

        ResultAssert.Unsuccessful((Result)result.Entity);
        Assert.IsType<InvalidOperationError>(result.Entity.Error);
    }

    /// <summary>
    /// Tests that the returned result is unsuccessful if the command is executed unsuccessfully, and that the returned error is
    /// not wrapped in an unexpected manner.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReturnsCommandErrorWhenCommandThrows()
    {
        var services = new ServiceCollection()
                           .AddDiscordGateway(_ => "dummy")
                           .AddDiscordCommands()
                           .AddCommandTree()
                           .WithCommandGroup<GroupWithNodeContext>()
                           .Finish()
                           .BuildServiceProvider();

        var commandService = services.GetRequiredService<CommandService>();
        var nodeInjection = services.GetRequiredService<CommandNodeInjectionService>();

        var result = await commandService.TryPrepareAndExecuteCommandAsync
        (
            commandString: "node throw",
            services: services
        );

        Assert.Equal(nodeInjection.Node.Value?.Key, "throw");

        ResultAssert.Unsuccessful((Result)result.Entity);
        Assert.IsType<ExceptionError>(result.Entity.Error);
    }
}
