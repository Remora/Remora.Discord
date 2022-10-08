//
//  CommandNodeExtensionsTests.cs
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
using System.Linq;
using Remora.Commands.Groups;
using Remora.Commands.Trees;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Tests.Data.Ephemeral;
using Xunit;

namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="CommandNodeExtensions"/> class.
/// </summary>
public static class CommandNodeExtensionsTests
{
    /// <summary>
    /// Tests the <see cref="CommandNodeExtensions.FindCustomAttributeOnLocalTree{T}(Remora.Commands.Trees.Nodes.CommandNode, bool)"/> method.
    /// </summary>
    public class FindCustomAttributeOnLocalTreeTests
    {
        /// <summary>
        /// Tests whether the method finds a custom attribute that decorates the command.
        /// </summary>
        [Fact]
        public void FindsAttributeOnCommand()
        {
            var tree = BuildCommandTree<EphemeralCommand>();
            var node = tree.Search("a").Single();

            Assert.NotNull(node.Node.FindCustomAttributeOnLocalTree<EphemeralAttribute>());
        }

        /// <summary>
        /// Tests whether the method finds a custom attribute that decorates the group, but not the command.
        /// </summary>
        [Fact]
        public void FindsAttributeOnGroup()
        {
            var tree = BuildCommandTree<EphemeralGroup>();
            var node = tree.Search
            (
                new[] { "a", "b" },
                new Dictionary<string, IReadOnlyList<string>>()
            ).Single();

            Assert.NotNull(node.Node.FindCustomAttributeOnLocalTree<EphemeralAttribute>());
        }

        /// <summary>
        /// Tests whether the method ignores ancestors of a command that have the given attribute, when specified.
        /// </summary>
        [Fact]
        public void IgnoresAttributeOnAncestorWhenSpecified()
        {
            var tree = BuildCommandTree<EphemeralGroup>();
            var node = tree.Search
            (
                new[] { "a", "b" },
                new Dictionary<string, IReadOnlyList<string>>()
            ).Single();

            Assert.Null(node.Node.FindCustomAttributeOnLocalTree<EphemeralAttribute>(false));
        }

        private static CommandTree BuildCommandTree<TModule>() where TModule : CommandGroup
        {
            CommandTreeBuilder builder = new();
            builder.RegisterModule<TModule>();

            return builder.Build();
        }
    }
}
