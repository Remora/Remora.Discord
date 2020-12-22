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
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Tests.Data.DiscordLimits;
using Remora.Discord.Commands.Tests.Data.InternalLimits;
using Remora.Discord.Tests;
using Xunit;

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

                    var result = tree.CreateApplicationCommands(out _);
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

                    var result = tree.CreateApplicationCommands(out _);
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

                    var result = tree.CreateApplicationCommands(out _);
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

                    var result = tree.CreateApplicationCommands(out _);
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

                    var result = tree.CreateApplicationCommands(out _);
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

                    var result = tree.CreateApplicationCommands(out _);
                    ResultAssert.Unsuccessful(result);
                }
            }

            /// <summary>
            /// Tests various edge cases.
            /// </summary>
            public class EdgeCases
            {
                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void ReturnsSuccessfulIfAnEnumHasTooManyChoices()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooManyChoiceValues>();

                    var tree = builder.Build();

                    var result = tree.CreateApplicationCommands(out _);
                    ResultAssert.Successful(result);
                }

                /// <summary>
                /// Tests whether the method responds appropriately to a failure case.
                /// </summary>
                [Fact]
                public void DoesNotCreateChoicesIfAnEnumHasTooManyChoices()
                {
                    var builder = new CommandTreeBuilder();
                    builder.RegisterModule<TooManyChoiceValues>();

                    var tree = builder.Build();
                    _ = tree.CreateApplicationCommands(out var commands);

                    var command = commands!.Single();

                    if (command.Choices.HasValue)
                    {
                        Assert.Empty(command.Choices.Value!);
                    }
                }
            }

            /// <summary>
            /// Tests various successful cases.
            /// </summary>
            public class Successes
            {
            }
        }
    }
}
