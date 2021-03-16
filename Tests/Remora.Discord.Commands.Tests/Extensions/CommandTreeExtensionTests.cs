//
//  CommandTreeExtensionTests.cs
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

using System.Linq;
using Remora.Commands.Trees;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Tests.Data.DiscordLimits;
using Remora.Discord.Commands.Tests.Data.InternalLimits;
using Remora.Discord.Commands.Tests.Data.Valid;
using Remora.Discord.Tests;
using Xunit;
using static Remora.Discord.API.Abstractions.Objects.ApplicationCommandOptionType;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
namespace Remora.Discord.Commands.Tests.Extensions
{
    /// <summary>
    /// Tests the <see cref="CommandTreeExtensions"/> class.
    /// </summary>
    public class CommandTreeExtensionTests
    {
        /// <summary>
        /// Tests the <see cref="CommandTreeExtensions.CreateApplicationCommands"/> method.
        /// </summary>
        public class CreateApplicationCommands
        {
            /// <summary>
            /// Tests various failing cases.
            /// </summary>
            public class Failures
            {
                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfGroupsAreTooDeeplyNested()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooDeeplyNested>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfACommandHasTooManyParameters()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooManyCommandParameters>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfThereAreTooManyRootLevelCommands()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooManyCommands>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfAGroupHasTooManyCommands()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooManyGroupCommands>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfACommandContainsACollectionParameter()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<CollectionsAreNotSupported>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfACommandContainsASwitchParameter()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<SwitchesAreNotSupported>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfThereAreOverloadsAtTheRootLevel()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<OverloadsAreNotSupportedInRoot>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfThereAreOverloadsInAGroup()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<OverloadsAreNotSupportedInGroups>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfACommandIsTooLong()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooLongCommand>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfACommandDescriptionIsTooLong()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooLongCommandDescription>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsUnsuccessfulIfAParameterDescriptionIsTooLong()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooLongParameterDescription>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Unsuccessful(result);
                }
            }

            /// <summary>
            /// Tests various successful cases.
            /// </summary>
            public class Successes
            {
                /// <summary>
                /// Tests whether the method responds appropriately to a successful case.
                /// </summary>
                [Fact]
                public void ReturnsSuccessForValidTree()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<ValidCommandGroup>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    ResultAssert.Successful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a successful case.
                /// </summary>
                [Fact]
                public void CreatesValidTreeCorrectly()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<ValidCommandGroup>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    var commands = result.Entity;

                    ResultAssert.Successful(result);

                    Assert.NotNull(commands);
                    Assert.Equal(2, commands!.Count);

                    var topLevelCommand = commands.FirstOrDefault(c => c.Type == SubCommand);
                    Assert.NotNull(topLevelCommand);

                    var topLevelGroup = commands.FirstOrDefault(c => c.Type == SubCommandGroup);
                    Assert.NotNull(topLevelGroup);

                    Assert.True(topLevelGroup!.Options.HasValue);
                    Assert.Equal(2, topLevelGroup.Options.Value!.Count);

                    var firstNestedCommand = topLevelGroup.Options.Value!.FirstOrDefault(c => c.Type == SubCommand);
                    Assert.NotNull(firstNestedCommand);

                    var nestedGroup = topLevelGroup.Options.Value!.FirstOrDefault(c => c.Type == SubCommandGroup);
                    Assert.NotNull(nestedGroup);

                    Assert.True(nestedGroup!.Options.HasValue);
                    Assert.Single(nestedGroup.Options.Value!);

                    var secondNestedCommand = nestedGroup.Options.Value!.FirstOrDefault(c => c.Type == SubCommand);
                    Assert.NotNull(secondNestedCommand);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a successful case.
                /// </summary>
                [Fact]
                public void CreatesTypedOptionsCorrectly()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TypedCommands>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    var commands = result.Entity;

                    ResultAssert.Successful(result);
                    Assert.NotNull(commands);

                    void AssertExistsWithType(string commandName, ApplicationCommandOptionType type)
                    {
                        var command = commands!.FirstOrDefault(c => c.Name == commandName);
                        Assert.NotNull(command);

                        var parameter = command!.Options.Value![0];
                        Assert.Equal(type, parameter.Type);
                    }

                    AssertExistsWithType("sbyte-value", Integer);
                    AssertExistsWithType("byte-value", Integer);
                    AssertExistsWithType("short-value", Integer);
                    AssertExistsWithType("ushort-value", Integer);
                    AssertExistsWithType("int-value", Integer);
                    AssertExistsWithType("uint-value", Integer);
                    AssertExistsWithType("long-value", Integer);
                    AssertExistsWithType("ulong-value", Integer);

                    AssertExistsWithType("float-value", String);
                    AssertExistsWithType("double-value", String);
                    AssertExistsWithType("decimal-value", String);

                    AssertExistsWithType("string-value", String);

                    AssertExistsWithType("bool-value", Boolean);

                    AssertExistsWithType("role-value", Role);
                    AssertExistsWithType("user-value", User);
                    AssertExistsWithType("channel-value", Channel);
                    AssertExistsWithType("member-value", User);

                    AssertExistsWithType("enum-value", String);
                    var enumCommand = commands!.First(c => c.Name == "enum-value");
                    var enumParameter = enumCommand.Options.Value![0];
                    Assert.True(enumParameter.Choices.HasValue);

                    var enumChoices = enumParameter.Choices.Value!;
                    Assert.Equal(2, enumChoices.Count);
                    Assert.Collection
                    (
                        enumChoices,
                        choice =>
                        {
                            Assert.Equal(nameof(TestEnum.Value1), choice.Name);
                            Assert.True(choice.Value.IsT0);
                            Assert.Equal(nameof(TestEnum.Value1), choice.Value.AsT0);
                        },
                        choice =>
                        {
                            Assert.Equal(nameof(TestEnum.Value2), choice.Name);
                            Assert.True(choice.Value.IsT0);
                            Assert.Equal(nameof(TestEnum.Value2), choice.Value.AsT0);
                        }
                    );
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a successful case.
                /// </summary>
                [Fact]
                public void CreatesRequiredOptionsCorrectly()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<CommandsWithRequiredOrOptionalParameters>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands();
                    var commands = result.Entity;

                    ResultAssert.Successful(result);
                    Assert.NotNull(commands);

                    var requiredCommand = commands!.First(c => c.Name == "required");
                    var requiredParameter = requiredCommand.Options.Value![0];
                    Assert.True(requiredParameter.IsRequired.HasValue);
                    Assert.True(requiredParameter.IsRequired.Value);

                    var optionalCommand = commands!.First(c => c.Name == "optional");
                    var optionalParameter = optionalCommand.Options.Value![0];
                    if (optionalParameter.IsRequired.HasValue)
                    {
                        Assert.False(optionalParameter.IsRequired.Value);
                    }
                }
            }
        }
    }
}
