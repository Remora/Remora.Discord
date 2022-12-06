//
//  ServiceCollectionExtensionsTests.cs
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

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Commands.Services;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Extensions.Extensions;
using Remora.Discord.Extensions.Tests.Samples;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Services;
using Xunit;

namespace Remora.Discord.Extensions.Tests;

/// <summary>
/// Tests for <see cref="Remora.Discord.Extensions.Extensions.ServiceCollectionExtensions"/>.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    /// <summary>
    /// Tests that the method successfully registers all commands from the assembly.
    /// </summary>
    [Fact]
    public void AddsCommandGroupsFromAssemblySuccessfully()
    {
        var services = new ServiceCollection();

        services
            .AddCommands()
            .AddCommandGroupsFromAssembly(typeof(ServiceCollectionExtensionsTests).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        var commandTree = serviceProvider.GetRequiredService<CommandTreeAccessor>();

        var treeExists = commandTree.TryGetNamedTree(null, out var tree);

        Assert.True(treeExists);
        Assert.NotEmpty(tree!.Root.Children);
    }

    /// <summary>
    /// Tests that the method successfully registers all responders from the assembly.
    /// </summary>
    [Fact]
    public void AddsRespondersFromAssemblySuccessfully()
    {
        var services = new ServiceCollection();

        services
            .AddDiscordGateway(_ => "ooga")
            .AddRespondersFromAssembly(typeof(ServiceCollectionExtensionsTests).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        var responderRepo = serviceProvider.GetRequiredService<IResponderTypeRepository>();

        var messageCreateResponders = responderRepo.GetResponderTypes<IMessageCreate>().ToArray();
        var messageDeleteResponders = responderRepo.GetResponderTypes<IMessageDelete>().ToArray();

        var messageCreateType = Assert.Single(messageCreateResponders);
        Assert.Equal(typeof(TestResponder), messageCreateType);

        var messageDeleteType = Assert.Single(messageDeleteResponders);
        Assert.Equal(typeof(TestResponder), messageDeleteType);
    }
}
