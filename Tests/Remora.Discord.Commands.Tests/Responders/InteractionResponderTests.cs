//
//  InteractionResponderTests.cs
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OneOf;
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Responders;
using Remora.Discord.Commands.Services;
using Remora.Discord.Commands.Tests.Data.Ephemeral;
using Remora.Discord.Commands.Tests.Data.Events;
using Remora.Discord.Commands.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Responders;

/// <summary>
/// Tests the <see cref="InteractionResponder"/> class.
/// </summary>
public class InteractionResponderTests
{
    /// <summary>
    /// Tests pre-execution events.
    /// </summary>
    public class PreExecutionEvents : InteractionResponderTestBase
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
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("successful");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _preExecutionEventMock.Verify
            (
                e => e.BeforeExecutionAsync(It.IsAny<ICommandContext>(), It.IsAny<CancellationToken>())
            );
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .Configure<InteractionResponderOptions>(o => o.SuppressAutomaticResponses = true)
                .AddCommandTree()
                    .WithCommandGroup<SimpleGroup>()
                .Finish()
                .AddScoped(_ => _preExecutionEventMock.Object);
        }
    }

    /// <summary>
    /// Tests post-execution events.
    /// </summary>
    public class PostExecutionEvents : InteractionResponderTestBase
    {
        private readonly Mock<IPostExecutionEvent> _postExecutionEventMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostExecutionEvents"/> class.
        /// </summary>
        public PostExecutionEvents()
        {
            _postExecutionEventMock = new Mock<IPostExecutionEvent>();

            _postExecutionEventMock.Setup
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
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("successful");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _postExecutionEventMock.Verify
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
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("unsuccessful");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _postExecutionEventMock.Verify
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
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("notfound");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            _postExecutionEventMock.Verify
            (
                e => e.AfterExecutionAsync
                (
                    It.IsAny<ICommandContext>(),
                    It.Is<IResult>(r => !r.IsSuccess),
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

    /// <summary>
    /// Tests that proper interaction responses are dispatched for ephemeral commands.
    /// </summary>
    public class EphemeralCommands : InteractionResponderTestBase
    {
        /// <summary>
        /// Tests whether ephemeral commands are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task EphemeralHandledCorrectly()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("a");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi.Verify
            (
                e => e.CreateInteractionResponseAsync
                (
                    It.IsAny<Snowflake>(),
                    It.IsAny<string>(),
                    It.Is<IInteractionResponse>
                    (
                        r => r.Data.HasValue &&
                             r.Data.Value.AsT0.Flags.HasValue &&
                             (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) != 0
                    ),
                    It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        /// <summary>
        /// Tests whether non-ephemeral commands are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task NonEphemeralHandledCorrectly()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("b");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi.Verify
            (
                e => e.CreateInteractionResponseAsync
                (
                    It.IsAny<Snowflake>(),
                    It.IsAny<string>(),
                    It.Is<IInteractionResponse>
                    (
                        r => !r.Data.HasValue ||
                             !r.Data.Value.AsT0.Flags.HasValue ||
                             (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) == 0
                    ),
                    It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        /// <summary>
        /// Tests whether commands that explicitly opt out of ephemeral responses are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task DisabledEphemeralHandledCorrectly()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("c");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi
                .Verify
                (
                    e => e.CreateInteractionResponseAsync
                    (
                        It.IsAny<Snowflake>(),
                        It.IsAny<string>(),
                        It.Is<IInteractionResponse>
                        (
                            r => !r.Data.HasValue ||
                                 !r.Data.Value.AsT0.Flags.HasValue ||
                                 (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) == 0
                        ),
                        It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                        It.IsAny<CancellationToken>()
                    )
                );
        }

        /// <summary>
        /// Tests whether commands that explicitly opt out of ephemeral responses are handled correctly, if
        /// ephemeral responses are globally enabled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task DisabledEphemeralHandledCorrectlyIfGloballyEphemeral()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("c");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi
                .Verify
                (
                    e => e.CreateInteractionResponseAsync
                    (
                        It.IsAny<Snowflake>(),
                        It.IsAny<string>(),
                        It.Is<IInteractionResponse>
                        (
                            r => !r.Data.HasValue ||
                                 !r.Data.Value.AsT0.Flags.HasValue ||
                                 (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) == 0
                        ),
                        It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                        It.IsAny<CancellationToken>()
                    )
                );
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCommandTree()
                .WithCommandGroup<EphemeralCommand>();
        }
    }

    /// <summary>
    /// Tests that proper interaction responses are dispatched for globally-enabled ephemeral commands.
    /// </summary>
    public class GlobalEphemeralCommands : InteractionResponderTestBase
    {
        /// <summary>
        /// Tests whether ephemeral commands are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task EphemeralHandledCorrectly()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("a");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi.Verify
            (
                e => e.CreateInteractionResponseAsync
                (
                    It.IsAny<Snowflake>(),
                    It.IsAny<string>(),
                    It.Is<IInteractionResponse>
                    (
                        r => r.Data.HasValue &&
                             r.Data.Value.AsT0.Flags.HasValue &&
                             (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) != 0
                    ),
                    It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        /// <summary>
        /// Tests whether non-ephemeral commands are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task NonEphemeralHandledCorrectly()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("b");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi.Verify
            (
                e => e.CreateInteractionResponseAsync
                (
                    It.IsAny<Snowflake>(),
                    It.IsAny<string>(),
                    It.Is<IInteractionResponse>
                    (
                        r => !r.Data.HasValue ||
                             !r.Data.Value.AsT0.Flags.HasValue ||
                             (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) != 0
                    ),
                    It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        /// <summary>
        /// Tests whether commands that explicitly opt out of ephemeral responses are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task DisabledEphemeralHandledCorrectly()
        {
            var userMock = new Mock<IUser>();
            var dataMock = new Mock<IApplicationCommandData>();

            dataMock.Setup(d => d.Name).Returns("c");

            var eventMock = new Mock<IInteractionCreate>();

            eventMock.Setup(e => e.Type).Returns(InteractionType.ApplicationCommand);
            eventMock.Setup(e => e.ChannelID).Returns(DiscordSnowflake.New(0));
            eventMock.Setup(e => e.User).Returns(new Optional<IUser>(userMock.Object));
            eventMock.Setup(e => e.Data).Returns(OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(dataMock.Object));

            var result = await this.Responder.RespondAsync(eventMock.Object);
            ResultAssert.Successful(result);

            this.MockInteractionApi
                .Verify
                (
                    e => e.CreateInteractionResponseAsync
                    (
                        It.IsAny<Snowflake>(),
                        It.IsAny<string>(),
                        It.Is<IInteractionResponse>
                        (
                            r => !r.Data.HasValue ||
                                 !r.Data.Value.AsT0.Flags.HasValue ||
                                 (r.Data.Value.AsT0.Flags.Value & MessageFlags.Ephemeral) == 0
                        ),
                        It.IsAny<Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>>>(),
                        It.IsAny<CancellationToken>()
                    )
                );
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCommandTree()
                    .WithCommandGroup<EphemeralCommand>()
                .Finish()
                .Configure<InteractionResponderOptions>(o => o.UseEphemeralResponses = true );
        }
    }
}
