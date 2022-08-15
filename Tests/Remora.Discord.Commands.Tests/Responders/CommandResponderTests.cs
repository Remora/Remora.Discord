//
//  CommandResponderTests.cs
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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remora.Commands.Extensions;
using Remora.Commands.Results;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Responders;
using Remora.Discord.Commands.Services;
using Remora.Discord.Commands.Tests.Data.Events;
using Remora.Discord.Commands.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Responders;

/// <summary>
/// Tests the <see cref="CommandResponder"/> class.
/// </summary>
public class CommandResponderTests
{
    /// <summary>
    /// Tests pre-execution events.
    /// </summary>
    public class PreExecutionEvents : CommandResponderTestBase
    {
        private readonly Mock<IPreExecutionEvent> _preExecutionEventMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreExecutionEvents"/> class.
        /// </summary>
        public PreExecutionEvents()
        {
            _preExecutionEventMock = new Mock<IPreExecutionEvent>();

            _preExecutionEventMock
                .Setup(e => e.BeforeExecutionAsync(It.IsAny<ICommandContext>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Result.FromSuccess()));
        }

        /// <summary>
        /// Tests whether pre-execution events are executed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AreExecuted()
        {
            var authorMock = new Mock<IUser>();
            var eventMock = new Mock<IMessageCreate>();

            eventMock.Setup(e => e.Author).Returns(authorMock.Object);
            eventMock.Setup(e => e.Content).Returns("!successful");

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _preExecutionEventMock
                .Verify(e => e.BeforeExecutionAsync(It.IsAny<ICommandContext>(), It.IsAny<CancellationToken>()));
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCommandTree()
                    .WithCommandGroup<SimpleGroup>()
                .Finish()
                .AddScoped(_ => _preExecutionEventMock.Object);
        }
    }

    /// <summary>
    /// Tests post-execution events.
    /// </summary>
    public class PostExecutionEvents : CommandResponderTestBase
    {
        private readonly Mock<IPostExecutionEvent> _postExecutionEventMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostExecutionEvents"/> class.
        /// </summary>
        public PostExecutionEvents()
        {
            _postExecutionEventMock = new Mock<IPostExecutionEvent>();

            _postExecutionEventMock
                .Setup
                (
                    e => e.AfterExecutionAsync
                    (
                        It.IsAny<ICommandContext>(),
                        It.IsAny<IResult>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns(Task.FromResult(Result.FromSuccess()));
        }

        /// <summary>
        /// Tests whether post-execution events are executed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AreExecuted()
        {
            var authorMock = new Mock<IUser>();
            var eventMock = new Mock<IMessageCreate>();

            eventMock.Setup(e => e.Author).Returns(authorMock.Object);
            eventMock.Setup(e => e.Content).Returns("!successful");

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _postExecutionEventMock
                .Verify
                (
                    e => e.AfterExecutionAsync
                    (
                        It.IsAny<ICommandContext>(),
                        It.Is<IResult>(r => r.IsSuccess),
                        It.IsAny<CancellationToken>()
                    )
                );
        }

        /// <summary>
        /// Tests whether post-execution events are executed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AreExecutedForUnsuccessfulCommands()
        {
            var authorMock = new Mock<IUser>();
            var eventMock = new Mock<IMessageCreate>();

            eventMock.Setup(e => e.Author).Returns(authorMock.Object);
            eventMock.Setup(e => e.Content).Returns("!unsuccessful");

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _postExecutionEventMock
                .Verify
                (
                    e => e.AfterExecutionAsync
                    (
                        It.IsAny<ICommandContext>(),
                        It.Is<IResult>(r => !r.IsSuccess),
                        It.IsAny<CancellationToken>()
                    )
                );
        }

        /// <summary>
        /// Tests whether post-execution events are executed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AreExecutedForNotFoundCommands()
        {
            var authorMock = new Mock<IUser>();
            var eventMock = new Mock<IMessageCreate>();

            eventMock.Setup(e => e.Author).Returns(authorMock.Object);
            eventMock.Setup(e => e.Content).Returns("!notfound");

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _postExecutionEventMock
                .Verify
                (
                    e => e.AfterExecutionAsync
                    (
                        It.IsAny<ICommandContext>(),
                        It.Is<IResult>(r => r.Error is CommandNotFoundError),
                        It.IsAny<CancellationToken>()
                    )
                );
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCommandTree()
                    .WithCommandGroup<SimpleGroup>()
                .Finish()
                .AddScoped(_ => _postExecutionEventMock.Object);
        }
    }
}
