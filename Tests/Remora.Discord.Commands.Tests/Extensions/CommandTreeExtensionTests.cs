//
//  CommandTreeExtensionTests.cs
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
using System.Linq;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Tests.Data.DiscordLimits;
using Remora.Discord.Commands.Tests.Data.Exclusion;
using Remora.Discord.Commands.Tests.Data.InternalLimits;
using Remora.Discord.Commands.Tests.Data.Valid;
using Remora.Discord.Commands.Tests.Data.Valid.Basics;
using Remora.Discord.Tests;
using Xunit;
using static Remora.Discord.API.Abstractions.Objects.ApplicationCommandOptionType;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="CommandTreeExtensions"/> class.
/// </summary>
public class CommandTreeExtensionTests
{
    /// <summary>
    /// Tests the <see cref="CommandTreeExtensions.CreateApplicationCommands(CommandTree)"/> method.
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

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMultipleNamedGroupsWithTheSameNameHaveADefaultPermissionAttribute()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<AtMostOneDefaultPermissionAttributeAllowed.Named.GroupOne>();
                builder.RegisterModule<AtMostOneDefaultPermissionAttributeAllowed.Named.GroupTwo>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMultipleNamedGroupsWithTheSameNameHaveADefaultDMPermissionAttribute()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<AtMostOneDMPermissionAttributeAllowed.Named.GroupOne>();
                builder.RegisterModule<AtMostOneDMPermissionAttributeAllowed.Named.GroupTwo>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();

                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfContextMenuHasDescription()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ContextMenusWithDescriptionsAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfContextMenuIsNested()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<NestedContextMenusAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ThrowsIfContextMenuHasInvalidParameters()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ContextMenusWithInvalidParametersAreNotSupported>();

                var tree = builder.Build();

                Assert.Throws<InvalidOperationException>(() => tree.CreateApplicationCommands());
            }

            /// <summary>
            /// Tests whether method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfChannelTypesAttributeAppliedOnNonChannelParameter()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ChannelTypesAttributeOnlyOnChannelParameter>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfChannelTypesAttributeHasZeroValues()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ChannelTypesAttributeRequiresAtLeastOneValue>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMinLengthConstraintIsInvalid()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<InvalidLengthConstraints.InvalidMinLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMaxLengthConstraintIsInvalid()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<InvalidLengthConstraints.InvalidMaxLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMinLengthConstraintIsInvalidButMaxIsValid()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<InvalidLengthConstraints.InvalidMinAndValidMaxLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMaxLengthConstraintIsInvalidButMinIsValid()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<InvalidLengthConstraints.ValidMinAndInvalidMaxLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMinLengthConstraintIsAppliedToAnIncompatibleType()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<InvalidLengthConstraints.MinConstraintOnIncompatibleParameterType>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMaxLengthConstraintIsAppliedToAnIncompatibleType()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<InvalidLengthConstraints.MaxConstraintOnIncompatibleParameterType>();

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
                Assert.Equal(2, commands.Count);

                var topLevelCommand = commands.FirstOrDefault(c => c.Name == "top-level-command");
                Assert.NotNull(topLevelCommand);

                var topLevelGroup = commands.FirstOrDefault(c => c.Name == "top-level-group");
                Assert.NotNull(topLevelGroup);

                Assert.True(topLevelGroup.Options.HasValue);
                Assert.Equal(2, topLevelGroup.Options.Value.Count);

                var firstNestedCommand = topLevelGroup.Options.Value.FirstOrDefault(c => c.Type == SubCommand);
                Assert.NotNull(firstNestedCommand);

                var nestedGroup = topLevelGroup.Options.Value.FirstOrDefault(c => c.Type == SubCommandGroup);
                Assert.NotNull(nestedGroup);

                Assert.True(nestedGroup.Options.HasValue);
                Assert.Single(nestedGroup.Options.Value);

                var secondNestedCommand = nestedGroup.Options.Value.FirstOrDefault(c => c.Type == SubCommand);
                Assert.NotNull(secondNestedCommand);
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

                var requiredCommand = commands.First(c => c.Name == "required");
                var requiredParameter = requiredCommand.Options.Value[0];
                Assert.True(requiredParameter.IsRequired.HasValue);
                Assert.True(requiredParameter.IsRequired.Value);

                var optionalCommand = commands.First(c => c.Name == "optional");
                var optionalParameter = optionalCommand.Options.Value[0];
                if (optionalParameter.IsRequired.HasValue)
                {
                    Assert.False(optionalParameter.IsRequired.Value);
                }
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesUnnamedGroupWithDefaultPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<UnnamedGroupWithDefaultPermission>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                var command = commands.SingleOrDefault();
                Assert.Equal(8, command!.DefaultMemberPermissions?.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesNamedGroupWithDefaultPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<NamedGroupWithDefaultPermission>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                var command = commands.SingleOrDefault();
                Assert.Equal(8, command!.DefaultMemberPermissions?.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesUngroupedTopLevelCommandsWithDefaultPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<MultipleCommandsWithDefaultPermission.GroupOne>();
                builder.RegisterModule<MultipleCommandsWithDefaultPermission.GroupTwo>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                Assert.Equal(2, commands.Count);
                var a = commands[0];
                var b = commands[1];

                Assert.Equal(8, a.DefaultMemberPermissions?.Value);
                Assert.Equal(4, b.DefaultMemberPermissions?.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesUngroupedTopLevelCommandsWithDefaultDMPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<MultipleCommandsWithDMPermission.GroupOne>();
                builder.RegisterModule<MultipleCommandsWithDMPermission.GroupTwo>();
                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesMultipartGroupWithDefaultDMPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<MultipartNamedGroupWithDMPermission>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesContextMenuCommandsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithContextMenus>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                Assert.Equal(4, commands.Count);

                var user = commands[0];
                var message = commands[1];
                var userParameter = commands[2];
                var messageParameter = commands[3];

                Assert.Equal(ApplicationCommandType.User, user.Type.Value);
                Assert.Equal(ApplicationCommandType.Message, message.Type.Value);
                Assert.Equal(ApplicationCommandType.User, userParameter.Type.Value);
                Assert.Equal(ApplicationCommandType.Message, messageParameter.Type.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesCombinedContextMenuCommandsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithContextMenuAndCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                Assert.Equal(2, commands.Count);

                var normal = commands[0];
                var message = commands[1];

                Assert.Equal(ApplicationCommandType.ChatInput, normal.Type.Value);
                Assert.Equal(ApplicationCommandType.Message, message.Type.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesMinLengthConstrainedParametersCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<StringParameterWithLengthConstraints.MinLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                var command = commands.Single();
                var parameter = command.Options.Value.Single();

                Assert.Equal(0u, parameter.MinLength.Value);
                Assert.False(parameter.MaxLength.HasValue);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesMaxLengthConstrainedParametersCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<StringParameterWithLengthConstraints.MaxLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                var command = commands.Single();
                var parameter = command.Options.Value.Single();

                Assert.Equal(1u, parameter.MaxLength.Value);
                Assert.False(parameter.MinLength.HasValue);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesMinAndMaxLengthConstrainedParametersCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<StringParameterWithLengthConstraints.MinAndMaxLengthConstraint>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                var command = commands.Single();
                var parameter = command.Options.Value.Single();

                Assert.Equal(0u, parameter.MinLength.Value);
                Assert.Equal(1u, parameter.MaxLength.Value);
            }
        }

        /// <summary>
        /// Tests various cases related to types and type hints.
        /// </summary>
        public class Types
        {
            /// <summary>
            /// Gets complex test data.
            /// </summary>
            public static IEnumerable<object?[]> ComplexTestData => new[]
            {
                new object?[]
                {
                    typeof(TypedCommands),
                    "enum-value",
                    ApplicationCommandOptionType.String,
                    null,
                    new Action<IApplicationCommandOptionChoice>[]
                    {
                        a =>
                        {
                            Assert.Equal(nameof(DummyEnum.A), a.Name);
                            Assert.True(a.Value.IsT0);
                            Assert.Equal(nameof(DummyEnum.A), a.Value.AsT0);
                        },
                        b =>
                        {
                            Assert.Equal(nameof(DummyEnum.B), b.Name);
                            Assert.True(b.Value.IsT0);
                            Assert.Equal(nameof(DummyEnum.B), b.Value.AsT0);
                        }
                    }
                },
                new object?[]
                {
                    typeof(NullableTypedCommands),
                    "nullable-enum-value",
                    ApplicationCommandOptionType.String,
                    null,
                    new Action<IApplicationCommandOptionChoice>[]
                    {
                        a =>
                        {
                            Assert.Equal(nameof(DummyEnum.A), a.Name);
                            Assert.True(a.Value.IsT0);
                            Assert.Equal(nameof(DummyEnum.A), a.Value.AsT0);
                        },
                        b =>
                        {
                            Assert.Equal(nameof(DummyEnum.B), b.Name);
                            Assert.True(b.Value.IsT0);
                            Assert.Equal(nameof(DummyEnum.B), b.Value.AsT0);
                        }
                    }
                },
                new object?[]
                {
                    typeof(OptionalTypedCommands),
                    "optional-enum-value",
                    ApplicationCommandOptionType.String,
                    null,
                    new Action<IApplicationCommandOptionChoice>[]
                    {
                        a =>
                        {
                            Assert.Equal(nameof(DummyEnum.A), a.Name);
                            Assert.True(a.Value.IsT0);
                            Assert.Equal(nameof(DummyEnum.A), a.Value.AsT0);
                        },
                        b =>
                        {
                            Assert.Equal(nameof(DummyEnum.B), b.Name);
                            Assert.True(b.Value.IsT0);
                            Assert.Equal(nameof(DummyEnum.B), b.Value.AsT0);
                        }
                    }
                },
                new object[]
                {
                    typeof(SpecialTypedCommands),
                    "typed-channel-value",
                    ApplicationCommandOptionType.Channel,
                    new Action<ChannelType>[] { c => Assert.Equal(ChannelType.GuildText, c) }
                },
                new object[]
                {
                    typeof(SpecialTypedCommands),
                    "hinted-string-value",
                    ApplicationCommandOptionType.Role
                },
                new object[]
                {
                    typeof(SpecialTypedCommands),
                    "hinted-snowflake-value",
                    ApplicationCommandOptionType.Channel
                }
            };

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            /// <param name="moduleType">The module to register.</param>
            /// <param name="commandName">The name of the command to search for.</param>
            /// <param name="type">The type of the actual command.</param>
            /// <param name="channelTypeAsserter">
            /// The channel types the parameter should be constrained to, if any.
            /// </param>
            /// <param name="choiceAsserter">The choices that should have been created, if any.</param>
            [Theory]
            [InlineData(typeof(TypedCommands), "sbyte-value", Integer)]
            [InlineData(typeof(TypedCommands), "byte-value", Integer)]
            [InlineData(typeof(TypedCommands), "short-value", Integer)]
            [InlineData(typeof(TypedCommands), "ushort-value", Integer)]
            [InlineData(typeof(TypedCommands), "int-value", Integer)]
            [InlineData(typeof(TypedCommands), "uint-value", Integer)]
            [InlineData(typeof(TypedCommands), "long-value", Integer)]
            [InlineData(typeof(TypedCommands), "ulong-value", Integer)]
            [InlineData(typeof(TypedCommands), "float-value", Number)]
            [InlineData(typeof(TypedCommands), "double-value", Number)]
            [InlineData(typeof(TypedCommands), "decimal-value", Number)]
            [InlineData(typeof(TypedCommands), "string-value", ApplicationCommandOptionType.String)]
            [InlineData(typeof(TypedCommands), "bool-value", ApplicationCommandOptionType.Boolean)]
            [InlineData(typeof(TypedCommands), "role-value", ApplicationCommandOptionType.Role)]
            [InlineData(typeof(TypedCommands), "user-value", ApplicationCommandOptionType.User)]
            [InlineData(typeof(TypedCommands), "member-value", ApplicationCommandOptionType.User)]
            [InlineData(typeof(TypedCommands), "channel-value", ApplicationCommandOptionType.Channel)]
            [InlineData(typeof(TypedCommands), "snowflake-value", ApplicationCommandOptionType.String)]
            [InlineData(typeof(NullableTypedCommands), "nullable-sbyte-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-byte-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-short-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-ushort-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-int-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-uint-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-long-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-ulong-value", Integer)]
            [InlineData(typeof(NullableTypedCommands), "nullable-float-value", Number)]
            [InlineData(typeof(NullableTypedCommands), "nullable-double-value", Number)]
            [InlineData(typeof(NullableTypedCommands), "nullable-decimal-value", Number)]
            [InlineData(typeof(NullableTypedCommands), "nullable-string-value", ApplicationCommandOptionType.String)]
            [InlineData(typeof(NullableTypedCommands), "nullable-bool-value", ApplicationCommandOptionType.Boolean)]
            [InlineData(typeof(NullableTypedCommands), "nullable-role-value", ApplicationCommandOptionType.Role)]
            [InlineData(typeof(NullableTypedCommands), "nullable-user-value", ApplicationCommandOptionType.User)]
            [InlineData(typeof(NullableTypedCommands), "nullable-member-value", ApplicationCommandOptionType.User)]
            [InlineData(typeof(NullableTypedCommands), "nullable-channel-value", ApplicationCommandOptionType.Channel)]
            [InlineData(typeof(NullableTypedCommands), "nullable-snowflake-value", ApplicationCommandOptionType.String)]
            [InlineData(typeof(OptionalTypedCommands), "optional-sbyte-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-byte-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-short-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-ushort-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-int-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-uint-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-long-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-ulong-value", Integer)]
            [InlineData(typeof(OptionalTypedCommands), "optional-float-value", Number)]
            [InlineData(typeof(OptionalTypedCommands), "optional-double-value", Number)]
            [InlineData(typeof(OptionalTypedCommands), "optional-decimal-value", Number)]
            [InlineData(typeof(OptionalTypedCommands), "optional-string-value", ApplicationCommandOptionType.String)]
            [InlineData(typeof(OptionalTypedCommands), "optional-bool-value", ApplicationCommandOptionType.Boolean)]
            [InlineData(typeof(OptionalTypedCommands), "optional-role-value", ApplicationCommandOptionType.Role)]
            [InlineData(typeof(OptionalTypedCommands), "optional-user-value", ApplicationCommandOptionType.User)]
            [InlineData(typeof(OptionalTypedCommands), "optional-member-value", ApplicationCommandOptionType.User)]
            [InlineData(typeof(OptionalTypedCommands), "optional-channel-value", ApplicationCommandOptionType.Channel)]
            [InlineData(typeof(OptionalTypedCommands), "optional-snowflake-value", ApplicationCommandOptionType.String)]
            [MemberData(nameof(ComplexTestData))]
            public void CreatesTypedOptionsCorrectly
            (
                Type moduleType,
                string commandName,
                ApplicationCommandOptionType type,
                Action<ChannelType>[]? channelTypeAsserter = null,
                Action<IApplicationCommandOptionChoice>[]? choiceAsserter = null
            )
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule(moduleType);

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                var command = commands.FirstOrDefault(c => c.Name == commandName);
                Assert.NotNull(command);

                var parameter = command.Options.Value[0];
                Assert.Equal(type, parameter.Type);

                if (choiceAsserter is not null)
                {
                    var choices = parameter.Choices.Value;
                    Assert.Collection(choices, choiceAsserter);
                }
                else
                {
                    Assert.False(parameter.Choices.HasValue);
                }

                if (channelTypeAsserter is not null)
                {
                    var channelTypes = parameter.ChannelTypes.Value;
                    Assert.Collection(channelTypes, channelTypeAsserter);
                }
                else
                {
                    Assert.False(parameter.ChannelTypes.HasValue);
                }
            }
        }

        /// <summary>
        /// Tests various cases involving enumerations.
        /// </summary>
        public class Enums
        {
            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesDescriptionOverriddenEnumOptionsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithEnumParameterWithDescriptionOverrides>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                void AssertExistsWithType(string commandName, ApplicationCommandOptionType type)
                {
                    var command = commands.FirstOrDefault(c => c.Name == commandName);
                    Assert.NotNull(command);

                    var parameter = command.Options.Value[0];
                    Assert.Equal(type, parameter.Type);
                }

                AssertExistsWithType("description-enum", ApplicationCommandOptionType.String);
                var enumCommand = commands.First(c => c.Name == "description-enum");

                var enumParameter = enumCommand.Options.Value[0];
                Assert.True(enumParameter.Choices.HasValue);

                var enumChoices = enumParameter.Choices.Value;
                Assert.Collection
                (
                    enumChoices,
                    choice =>
                    {
                        Assert.Equal("A longer description", choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DescriptionEnum.A), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal(nameof(DescriptionEnum.B), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DescriptionEnum.B), choice.Value.AsT0);
                    }
                );
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesDisplayOverriddenEnumOptionsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithEnumParameterWithDisplayOverrides>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                void AssertExistsWithType(string commandName, ApplicationCommandOptionType type)
                {
                    var command = commands.FirstOrDefault(c => c.Name == commandName);
                    Assert.NotNull(command);

                    var parameter = command.Options.Value[0];
                    Assert.Equal(type, parameter.Type);
                }

                AssertExistsWithType("display-enum", ApplicationCommandOptionType.String);
                var enumCommand = commands.First(c => c.Name == "display-enum");

                var enumParameter = enumCommand.Options.Value[0];
                Assert.True(enumParameter.Choices.HasValue);

                var enumChoices = enumParameter.Choices.Value;
                Assert.Collection
                (
                    enumChoices,
                    choice =>
                    {
                        Assert.Equal("A description", choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DisplayEnum.A), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal("A name", choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DisplayEnum.B), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal("A preferred description", choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DisplayEnum.C), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal(nameof(DisplayEnum.D), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DisplayEnum.D), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal(nameof(DisplayEnum.E), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DisplayEnum.E), choice.Value.AsT0);
                    }
                );
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesExclusionOverriddenEnumOptionsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithEnumParameterWithExclusionOverrides>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                void AssertExistsWithType(string commandName, ApplicationCommandOptionType type)
                {
                    var command = commands.FirstOrDefault(c => c.Name == commandName);
                    Assert.NotNull(command);

                    var parameter = command.Options.Value[0];
                    Assert.Equal(type, parameter.Type);
                }

                AssertExistsWithType("excluded-enum", ApplicationCommandOptionType.String);
                var enumCommand = commands.First(c => c.Name == "excluded-enum");

                var enumParameter = enumCommand.Options.Value[0];
                Assert.True(enumParameter.Choices.HasValue);

                var enumChoices = enumParameter.Choices.Value;
                Assert.Collection
                (
                    enumChoices,
                    choice =>
                    {
                        Assert.Equal(nameof(ExcludedEnum.A), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(ExcludedEnum.A), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal(nameof(ExcludedEnum.B), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(ExcludedEnum.B), choice.Value.AsT0);
                    }
                );
            }
        }

        /// <summary>
        /// Tests various cases where commands are filtered out.
        /// </summary>
        public class Filtering
        {
            /// <summary>
            /// Tests whether a single command can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeCommand()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                var group = commands.Single();

                Assert.Equal("a", group.Name);

                Assert.Collection
                (
                    group.Options.Value,
                    c =>
                    {
                        Assert.Equal(SubCommand, c.Type);
                        Assert.Equal("b", c.Name);
                    }
                );
            }

            /// <summary>
            /// Tests whether a single nested command can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeNestedCommand()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedNestedCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                var group = commands.Single();

                Assert.Equal("a", group.Name);

                var nestedGroup = group.Options.Value.Single();

                Assert.Equal(SubCommandGroup, nestedGroup.Type);
                Assert.Equal("b", nestedGroup.Name);

                Assert.Collection
                (
                    nestedGroup.Options.Value,
                    c =>
                    {
                        Assert.Equal(SubCommand, c.Type);
                        Assert.Equal("d", c.Name);
                    }
                );
            }

            /// <summary>
            /// Tests whether a complete group can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeGroup()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedGroup>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                Assert.Empty(commands);
            }

            /// <summary>
            /// Tests whether a complete nested group can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeNestedGroup()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedNestedGroup>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                var group = commands.Single();

                Assert.Equal("a", group.Name);

                Assert.Collection
                (
                    group.Options.Value,
                    c =>
                    {
                        Assert.Equal(SubCommand, c.Type);
                        Assert.Equal("d", c.Name);
                    }
                );
            }

            /// <summary>
            /// Tests whether groups that are empty after exclusion filtering are optimized out.
            /// </summary>
            [Fact]
            public void EmptyGroupsAreOptimizedOut()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupThatIsEmptyAfterExclusion>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                Assert.Empty(commands);
            }

            /// <summary>
            /// Tests whether nested groups that are empty after exclusion filtering are optimized out.
            /// </summary>
            [Fact]
            public void NestedEmptyGroupsAreOptimizedOut()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<NestedGroupThatIsEmptyAfterExclusion>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                Assert.Empty(commands);
            }
        }
    }

    /// <summary>
    /// Tests the <see cref="CommandTreeExtensions.MapDiscordCommands"/> method.
    /// </summary>
    public class MapDiscordCommands
    {
        /// <summary>
        /// Tests whether the method can successfully map a single top-level command.
        /// </summary>
        [Fact]
        public void CanMapSingleTopLevelCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<UnnamedGroupWithSingleCommand>();

            var tree = treeBuilder.Build();

            var commandNode = tree.Root.Children[0];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    commandNode.Key,
                    string.Empty,
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, node) = map.Single();

            Assert.Equal((default, commandID), id);
            Assert.Same(commandNode, node.AsT1);
        }

        /// <summary>
        /// Tests whether the method can successfully map multiple top-level commands.
        /// </summary>
        [Fact]
        public void CanMapMultipleTopLevelCommands()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<UnnamedGroupWithMultipleCommands>();

            var tree = treeBuilder.Build();

            var commandNodeA = tree.Root.Children[0];
            var commandNodeB = tree.Root.Children[1];

            var commandAID = DiscordSnowflake.New(1);
            var commandBID = DiscordSnowflake.New(2);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandAID,
                    default,
                    default,
                    default,
                    commandNodeA.Key,
                    string.Empty,
                    default,
                    default
                ),
                new ApplicationCommand
                (
                    commandBID,
                    default,
                    default,
                    default,
                    commandNodeB.Key,
                    string.Empty,
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Equal(2, map.Count);
            var (nodeAID, nodeA) = map.ToList()[0];
            var (nodeBID, nodeB) = map.ToList()[1];

            Assert.Equal((default, commandAID), nodeAID);
            Assert.Same(commandNodeA, nodeA.AsT1);

            Assert.Equal((default, commandBID), nodeBID);
            Assert.Same(commandNodeB, nodeB.AsT1);
        }

        /// <summary>
        /// Tests whether the method can successfully map a single nested command.
        /// </summary>
        [Fact]
        public void CanMapSingleNestedCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<NamedGroupWithSingleCommand>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children[0];
            var commandNode = groupNode.Children[0];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommand, commandNode.Key, string.Empty)
                    },
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, value) = map.Single();

            Assert.Equal((default, commandID), id);

            var subMap = value.AsT0;
            Assert.Single(subMap);

            var (path, mappedNode) = subMap.Single();
            Assert.Equal("a::b", path);
            Assert.Same(commandNode, mappedNode);
        }

        /// <summary>
        /// Tests whether the method can successfully map a single nested command.
        /// </summary>
        [Fact]
        public void CanMapMultipleNestedCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<NamedGroupWithMultipleCommands>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children[0];
            var commandNodeB = groupNode.Children[0];
            var commandNodeC = groupNode.Children[1];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommand, commandNodeB.Key, string.Empty),
                        new ApplicationCommandOption(SubCommand, commandNodeC.Key, string.Empty)
                    },
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, value) = map.Single();

            Assert.Equal((default, commandID), id);

            var subMap = value.AsT0;
            Assert.Equal(2, subMap.Count);

            var (pathB, mappedNodeB) = subMap.ToList()[0];
            var (pathC, mappedNodeC) = subMap.ToList()[1];

            Assert.Equal("a::b", pathB);
            Assert.Equal("a::c", pathC);

            Assert.Same(commandNodeB, mappedNodeB);
            Assert.Same(commandNodeC, mappedNodeC);
        }

        /// <summary>
        /// Tests whether the method can successfully map a single deeply nested command.
        /// </summary>
        [Fact]
        public void CanMapDeeplyNestedCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<DeeplyNestedCommand>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children[0];
            var subGroupNode = (GroupNode)groupNode.Children[0];
            var commandNode = subGroupNode.Children[0];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommandGroup, subGroupNode.Key, string.Empty, Options: new[]
                        {
                            new ApplicationCommandOption(SubCommand, commandNode.Key, string.Empty)
                        })
                    },
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, value) = map.Single();

            Assert.Equal((default, commandID), id);

            var subMap = value.AsT0;
            Assert.Single(subMap);

            var (path, mappedNode) = subMap.Single();
            Assert.Equal("a::b::c", path);
            Assert.Same(commandNode, mappedNode);
        }

        /// <summary>
        /// Tests whether the method can successfully map a complex tree, which is a combination of all previous
        /// tests.
        /// </summary>
        [Fact]
        public void CanMapComplexTree()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<ComplexGroup>();
            treeBuilder.RegisterModule<ComplexGroupUnnamedPart>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children.Single(c => c.Key is "a");
            var subGroupNode = (GroupNode)groupNode.Children.Single(c => c.Key is "b");
            var commandNodeC = subGroupNode.Children.Single(c => c.Key is "c");
            var commandNodeD = subGroupNode.Children.Single(c => c.Key is "d");
            var commandNodeE = groupNode.Children.Single(c => c.Key is "e");
            var commandNodeF = groupNode.Children.Single(c => c.Key is "f");
            var commandNodeG = tree.Root.Children.Single(c => c.Key is "g");

            var groupID = DiscordSnowflake.New(1);
            var commandGroupID = DiscordSnowflake.New(2);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    groupID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommandGroup, subGroupNode.Key, string.Empty, Options: new[]
                        {
                            new ApplicationCommandOption(SubCommand, commandNodeC.Key, string.Empty),
                            new ApplicationCommandOption(SubCommand, commandNodeD.Key, string.Empty)
                        }),
                        new ApplicationCommandOption(SubCommand, commandNodeE.Key, string.Empty),
                        new ApplicationCommandOption(SubCommand, commandNodeF.Key, string.Empty)
                    },
                    default
                ),
                new ApplicationCommand
                (
                    commandGroupID,
                    default,
                    default,
                    default,
                    commandNodeG.Key,
                    string.Empty,
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            var (mappedGroupID, groupMap) = map.ToList()[0];

            Assert.Equal(mappedGroupID.CommandID, groupID);
            var (pathC, mappedCommandNodeC) = groupMap.AsT0.ToList()[0];
            var (pathD, mappedCommandNodeD) = groupMap.AsT0.ToList()[1];
            var (pathE, mappedCommandNodeE) = groupMap.AsT0.ToList()[2];
            var (pathF, mappedCommandNodeF) = groupMap.AsT0.ToList()[3];

            Assert.Equal("a::b::c", pathC);
            Assert.Equal("a::b::d", pathD);
            Assert.Equal("a::e", pathE);
            Assert.Equal("a::f", pathF);

            Assert.Same(mappedCommandNodeC, commandNodeC);
            Assert.Same(mappedCommandNodeD, commandNodeD);
            Assert.Same(mappedCommandNodeE, commandNodeE);
            Assert.Same(mappedCommandNodeF, commandNodeF);

            var (mappedCommandID, mappedCommandNode) = map.ToList()[1];

            Assert.Equal(mappedCommandID.CommandID, commandGroupID);
            Assert.Same(commandNodeG, mappedCommandNode.AsT1);
        }
    }
}
