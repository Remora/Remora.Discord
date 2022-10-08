//
//  InteractionDataExtensionsTests.cs
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

using OneOf;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;
using Xunit;

namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="ApplicationCommandDataExtensions"/> class.
/// </summary>
public class InteractionDataExtensionsTests
{
    /// <summary>
    /// Tests the <see cref="ApplicationCommandDataExtensions.UnpackInteraction"/> method.
    /// </summary>
    public class UnpackInteraction
    {
        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackRootLevelParameterlessCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "a",
                ApplicationCommandType.ChatInput
            );

            command.UnpackInteraction(out var commandPath, out var parameters);
            Assert.Equal(new[] { "a" }, commandPath);
            Assert.Empty(parameters);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackRootLevelSingleParameterCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "a",
                ApplicationCommandType.ChatInput,
                Options: new[]
                {
                    new ApplicationCommandInteractionDataOption
                    (
                        "b",
                        ApplicationCommandOptionType.Integer,
                        new Optional<OneOf<string, long, bool, Snowflake, double>>(1)
                    )
                }
            );

            command.UnpackInteraction(out var commandPath, out var parameters);
            Assert.Equal(new[] { "a" }, commandPath);

            Assert.Single(parameters);
            Assert.True(parameters.ContainsKey("b"));

            Assert.Single(parameters["b"]);
            Assert.Equal("1", parameters["b"][0]);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackRootLevelMultipleParameterCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "a",
                ApplicationCommandType.ChatInput,
                Options: new[]
                {
                    new ApplicationCommandInteractionDataOption
                    (
                        "b",
                        ApplicationCommandOptionType.Integer,
                        new Optional<OneOf<string, long, bool, Snowflake, double>>(1)
                    ),
                    new ApplicationCommandInteractionDataOption
                    (
                        "c",
                        ApplicationCommandOptionType.Integer,
                        new Optional<OneOf<string, long, bool, Snowflake, double>>(2)
                    )
                }
            );

            command.UnpackInteraction(out var commandPath, out var parameters);
            Assert.Equal(new[] { "a" }, commandPath);

            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.ContainsKey("b"));
            Assert.True(parameters.ContainsKey("c"));

            Assert.Single(parameters["b"]);
            Assert.Equal("1", parameters["b"][0]);

            Assert.Single(parameters["c"]);
            Assert.Equal("2", parameters["c"][0]);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackNestedParameterlessCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "a",
                ApplicationCommandType.ChatInput,
                Options: new[]
                {
                    new ApplicationCommandInteractionDataOption
                    (
                        "b",
                        default
                    )
                }
            );

            command.UnpackInteraction(out var commandPath, out var parameters);
            Assert.Equal(new[] { "a", "b" }, commandPath);
            Assert.Empty(parameters);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackNestedSingleParameterCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "a",
                ApplicationCommandType.ChatInput,
                Options: new[]
                {
                    new ApplicationCommandInteractionDataOption
                    (
                        "b",
                        ApplicationCommandOptionType.SubCommand,
                        default,
                        new[]
                        {
                            new ApplicationCommandInteractionDataOption
                            (
                                "c",
                                ApplicationCommandOptionType.Integer,
                                new Optional<OneOf<string, long, bool, Snowflake, double>>(1)
                            )
                        }
                    )
                }
            );

            command.UnpackInteraction(out var commandPath, out var parameters);
            Assert.Equal(new[] { "a", "b" }, commandPath);

            Assert.Single(parameters);
            Assert.True(parameters.ContainsKey("c"));

            Assert.Single(parameters["c"]);
            Assert.Equal("1", parameters["c"][0]);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackNestedMultipleParameterCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "a",
                ApplicationCommandType.ChatInput,
                Options: new[]
                {
                    new ApplicationCommandInteractionDataOption
                    (
                        "b",
                        ApplicationCommandOptionType.SubCommand,
                        default,
                        new[]
                        {
                            new ApplicationCommandInteractionDataOption
                            (
                                "c",
                                ApplicationCommandOptionType.Integer,
                                new Optional<OneOf<string, long, bool, Snowflake, double>>(1)
                            ),
                            new ApplicationCommandInteractionDataOption
                            (
                                "d",
                                ApplicationCommandOptionType.Integer,
                                new Optional<OneOf<string, long, bool, Snowflake, double>>(2)
                            )
                        }
                    )
                }
            );

            command.UnpackInteraction(out var commandPath, out var parameters);
            Assert.Equal(new[] { "a", "b" }, commandPath);

            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.ContainsKey("c"));
            Assert.True(parameters.ContainsKey("d"));

            Assert.Single(parameters["c"]);
            Assert.Equal("1", parameters["c"][0]);

            Assert.Single(parameters["d"]);
            Assert.Equal("2", parameters["d"][0]);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackUserCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "user",
                ApplicationCommandType.User,
                TargetID: DiscordSnowflake.New(1)
            );

            command.UnpackInteraction(out var commandPath, out var parameters);

            Assert.Equal(new[] { "user" }, commandPath);
            Assert.Equal(1, parameters.Count);
            Assert.True(parameters.ContainsKey("user"));
            Assert.Equal("1", parameters["user"][0]);
        }

        /// <summary>
        /// Tests whether the method performs as expected in a single case.
        /// </summary>
        [Fact]
        public void CanUnpackMessageCommand()
        {
            var command = new ApplicationCommandData
            (
                DiscordSnowflake.New(0),
                "message",
                ApplicationCommandType.Message,
                TargetID: DiscordSnowflake.New(1)
            );

            command.UnpackInteraction(out var commandPath, out var parameters);

            Assert.Equal(new[] { "message" }, commandPath);
            Assert.Equal(1, parameters.Count);
            Assert.True(parameters.ContainsKey("message"));
            Assert.Equal("1", parameters["message"][0]);
        }
    }
}
