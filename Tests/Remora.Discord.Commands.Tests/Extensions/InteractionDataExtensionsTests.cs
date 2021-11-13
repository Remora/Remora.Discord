//
//  InteractionDataExtensionsTests.cs
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

using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Core;
using Remora.Rest.Core;
using Xunit;

namespace Remora.Discord.Commands.Tests.Extensions
{
    /// <summary>
    /// Tests the <see cref="InteractionDataExtensions"/> class.
    /// </summary>
    public class InteractionDataExtensionsTests
    {
        /// <summary>
        /// Tests the <see cref="InteractionDataExtensions.UnpackInteraction"/> method.
        /// </summary>
        public class UnpackInteraction
        {
            /// <summary>
            /// Tests whether the method performs as expected in a single case.
            /// </summary>
            [Fact]
            public void CanUnpackRootLevelParameterlessCommand()
            {
                var command = new InteractionData
                (
                    new Snowflake(0),
                    "a",
                    default
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
                var command = new InteractionData
                (
                    new Snowflake(0),
                    "a",
                    default,
                    new[]
                    {
                        new ApplicationCommandInteractionDataOption
                        (
                            "b",
                            ApplicationCommandOptionType.Integer,
                            new Optional<OneOf<IApplicationCommandInteractionDataOption, string, long, bool, Snowflake>>(1)
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
                var command = new InteractionData
                (
                    new Snowflake(0),
                    "a",
                    default,
                    new[]
                    {
                        new ApplicationCommandInteractionDataOption
                        (
                            "b",
                            ApplicationCommandOptionType.Integer,
                            new Optional<OneOf<IApplicationCommandInteractionDataOption, string, long, bool, Snowflake>>(1)
                        ),
                        new ApplicationCommandInteractionDataOption
                        (
                            "c",
                            ApplicationCommandOptionType.Integer,
                            new Optional<OneOf<IApplicationCommandInteractionDataOption, string, long, bool, Snowflake>>(2)
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
                var command = new InteractionData
                (
                    new Snowflake(0),
                    "a",
                    default,
                    new[]
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
                var command = new InteractionData
                (
                    new Snowflake(0),
                    "a",
                    default,
                    new[]
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
                                    new Optional<OneOf<IApplicationCommandInteractionDataOption, string, long, bool, Snowflake>>(1)
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
                var command = new InteractionData
                (
                    new Snowflake(0),
                    "a",
                    default,
                    new[]
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
                                    new Optional<OneOf<IApplicationCommandInteractionDataOption, string, long, bool, Snowflake>>(1)
                                ),
                                new ApplicationCommandInteractionDataOption
                                (
                                    "d",
                                    ApplicationCommandOptionType.Integer,
                                    new Optional<OneOf<IApplicationCommandInteractionDataOption, string, long, bool, Snowflake>>(2)
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
        }
    }
}
