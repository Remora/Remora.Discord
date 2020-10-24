//
//  CommandServiceTests.cs
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
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Commands.Services;
using Remora.Commands.Tests.Data.Modules;
using Xunit;

namespace Remora.Commands.Tests.Services
{
    /// <summary>
    /// Tests the <see cref="CommandService"/> class.
    /// </summary>
    public class CommandServiceTests
    {
        /// <summary>
        /// Tests whether the command service can execute a parameterless command.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteParameterlessCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test parameterless", default);

            Assert.True(executionResult.IsSuccess);
        }

        /// <summary>
        /// Tests whether the command service can execute a command with a single positional parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteSinglePositionalCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test single-positional booga", default);

            Assert.True(executionResult.IsSuccess);
        }

        /// <summary>
        /// Tests whether the command service can execute a command with a single optional positional parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteSingleOptionalPositionalCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test single-optional-positional", default);

            Assert.True(executionResult.IsSuccess);
        }

        /// <summary>
        /// Tests whether the command service can execute a command with a single positional parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteSingleNamedCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test single-named --value booga", default);

            Assert.True(executionResult.IsSuccess);
        }

        /// <summary>
        /// Tests whether the command service can execute a command with a single positional parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteSingleOptionalNamedCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test single-optional-named", default);

            Assert.True(executionResult.IsSuccess);
        }

        /// <summary>
        /// Tests whether the command service can execute a command with a single positional boolean parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteSinglePositionalBoolCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test single-positional-bool true", default);

            Assert.True(executionResult.IsSuccess);
        }

        /// <summary>
        /// Tests whether the command service can execute a command with a single positional boolean parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanExecuteSingleNamedBoolCommand()
        {
            var services = new ServiceCollection()
                .AddCommands()
                .AddCommandModule<TestCommandModule>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<CommandService>();
            var executionResult = await commandService.TryExecuteAsync("test single-named-bool --enable", default);

            Assert.True(executionResult.IsSuccess);

            executionResult = await commandService.TryExecuteAsync("test single-named-bool", default);

            Assert.True(executionResult.IsSuccess);
        }
    }
}
