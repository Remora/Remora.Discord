//
//  CommandTreeBuilderTests.cs
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
using Remora.Commands.Tests.Data.DummyModules;
using Remora.Commands.Tests.Data.Modules;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Xunit;

namespace Remora.Commands.Tests.Trees
{
    /// <summary>
    /// Tests the <see cref="CommandTreeBuilder"/> class.
    /// </summary>
    public class CommandTreeBuilderTests
    {
        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesNamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<NamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Single(root.Children);
            Assert.IsType<GroupNode>(root.Children[0]);

            var groupNode = (GroupNode)root.Children[0];

            Assert.Equal("a", groupNode.Key);

            Assert.Equal(3, groupNode.Children.Count);
            Assert.IsType<CommandNode>(groupNode.Children[0]);
            Assert.IsType<CommandNode>(groupNode.Children[1]);
            Assert.IsType<CommandNode>(groupNode.Children[2]);

            var command1 = (CommandNode)groupNode.Children[0];
            var command2 = (CommandNode)groupNode.Children[1];
            var command3 = (CommandNode)groupNode.Children[2];

            Assert.Equal("b", command1.Key);
            Assert.Equal("c", command2.Key);
            Assert.Equal("d", command3.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithCommandsWithNestedNamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesNamedGroupWithCommandsWithNestedNamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<NamedGroupWithCommandsWithNestedNamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Single(root.Children);
            Assert.IsType<GroupNode>(root.Children[0]);

            var groupNode = (GroupNode)root.Children[0];

            Assert.Equal("a", groupNode.Key);

            Assert.Equal(2, groupNode.Children.Count);
            Assert.Contains(groupNode.Children, n => n is GroupNode);
            Assert.Contains(groupNode.Children, n => n is CommandNode);

            var command1 = (CommandNode)groupNode.Children.First(n => n is CommandNode);
            Assert.Equal("b", command1.Key);

            var nestedGroupNode = (GroupNode)groupNode.Children.First(n => n is GroupNode);
            Assert.Equal("c", nestedGroupNode.Key);

            Assert.Single(nestedGroupNode.Children);
            var command2 = (CommandNode)nestedGroupNode.Children[0];

            Assert.Equal("d", command2.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithCommandsWithNestedUnnamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesNamedGroupWithCommandsWithNestedUnnamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<NamedGroupWithCommandsWithNestedUnnamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Single(root.Children);
            Assert.IsType<GroupNode>(root.Children[0]);

            var groupNode = (GroupNode)root.Children[0];

            Assert.Equal("a", groupNode.Key);

            Assert.Equal(3, groupNode.Children.Count);
            Assert.IsType<CommandNode>(groupNode.Children[0]);
            Assert.IsType<CommandNode>(groupNode.Children[1]);
            Assert.IsType<CommandNode>(groupNode.Children[2]);

            var command1 = (CommandNode)groupNode.Children[0];
            var command2 = (CommandNode)groupNode.Children[1];
            var command3 = (CommandNode)groupNode.Children[2];

            Assert.Equal("b", command1.Key);
            Assert.Equal("c", command2.Key);
            Assert.Equal("d", command3.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithNestedNamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesNamedGroupWithNestedNamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<NamedGroupWithNestedNamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Single(root.Children);
            Assert.IsType<GroupNode>(root.Children[0]);

            var groupNode = (GroupNode)root.Children[0];

            Assert.Equal("a", groupNode.Key);

            Assert.Single(groupNode.Children);
            Assert.IsType<GroupNode>(groupNode.Children[0]);

            var nestedGroupNode = (GroupNode)groupNode.Children[0];
            Assert.Equal("b", nestedGroupNode.Key);

            var command1 = (CommandNode)nestedGroupNode.Children[0];
            Assert.Equal("c", command1.Key);

            var command2 = (CommandNode)nestedGroupNode.Children[1];
            Assert.Equal("d", command2.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithNestedUnnamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesNamedGroupWithNestedUnnamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<NamedGroupWithNestedUnnamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Single(root.Children);
            Assert.IsType<GroupNode>(root.Children[0]);

            var groupNode = (GroupNode)root.Children[0];

            Assert.Equal("a", groupNode.Key);

            Assert.Equal(3, groupNode.Children.Count);
            Assert.IsType<CommandNode>(groupNode.Children[0]);
            Assert.IsType<CommandNode>(groupNode.Children[1]);
            Assert.IsType<CommandNode>(groupNode.Children[2]);

            var command1 = (CommandNode)groupNode.Children[0];
            var command2 = (CommandNode)groupNode.Children[1];
            var command3 = (CommandNode)groupNode.Children[2];

            Assert.Equal("b", command1.Key);
            Assert.Equal("c", command2.Key);
            Assert.Equal("d", command3.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesUnnamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<UnnamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Equal(4, root.Children.Count);
            Assert.IsType<CommandNode>(root.Children[0]);
            Assert.IsType<CommandNode>(root.Children[1]);
            Assert.IsType<CommandNode>(root.Children[2]);
            Assert.IsType<CommandNode>(root.Children[3]);

            var command1 = (CommandNode)root.Children[0];
            var command2 = (CommandNode)root.Children[1];
            var command3 = (CommandNode)root.Children[2];
            var command4 = (CommandNode)root.Children[3];

            Assert.Equal("a", command1.Key);
            Assert.Equal("b", command2.Key);
            Assert.Equal("c", command3.Key);
            Assert.Equal("d", command4.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithCommandsWithNestedNamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesUnnamedGroupWithCommandsWithNestedNamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<UnnamedGroupWithCommandsWithNestedNamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Equal(3, root.Children.Count);

            var command1 = (CommandNode)root.Children[0];
            Assert.Equal("a", command1.Key);

            var command2 = (CommandNode)root.Children[1];
            Assert.Equal("b", command2.Key);

            var groupNode = (GroupNode)root.Children[2];

            Assert.Single(groupNode.Children);
            var command3 = (CommandNode)groupNode.Children[0];

            Assert.Equal("d", command3.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithCommandsWithNestedUnnamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesUnnamedGroupWithCommandsWithNestedUnnamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<UnnamedGroupWithCommandsWithNestedUnnamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Equal(4, root.Children.Count);
            Assert.IsType<CommandNode>(root.Children[0]);
            Assert.IsType<CommandNode>(root.Children[1]);
            Assert.IsType<CommandNode>(root.Children[2]);
            Assert.IsType<CommandNode>(root.Children[3]);

            var command1 = (CommandNode)root.Children[0];
            var command2 = (CommandNode)root.Children[1];
            var command3 = (CommandNode)root.Children[2];
            var command4 = (CommandNode)root.Children[3];

            Assert.Equal("a", command1.Key);
            Assert.Equal("b", command2.Key);
            Assert.Equal("c", command3.Key);
            Assert.Equal("d", command4.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithNestedNamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesUnnamedGroupWithNestedNamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<UnnamedGroupWithNestedNamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Single(root.Children);
            Assert.IsType<GroupNode>(root.Children[0]);

            var groupNode = (GroupNode)root.Children[0];

            Assert.Equal("a", groupNode.Key);

            Assert.Equal(3, groupNode.Children.Count);
            Assert.IsType<CommandNode>(groupNode.Children[0]);
            Assert.IsType<CommandNode>(groupNode.Children[1]);
            Assert.IsType<CommandNode>(groupNode.Children[2]);

            var command1 = (CommandNode)groupNode.Children[0];
            var command2 = (CommandNode)groupNode.Children[1];
            var command3 = (CommandNode)groupNode.Children[2];

            Assert.Equal("b", command1.Key);
            Assert.Equal("c", command2.Key);
            Assert.Equal("d", command3.Key);
        }

        /// <summary>
        /// Tests whether a <see cref="NamedGroupWithNestedUnnamedGroupWithCommands"/> can be correctly parsed into a tree.
        /// </summary>
        [Fact]
        public void ParsesUnnamedGroupWithNestedUnnamedGroupWithCommandsCorrectly()
        {
            var builder = new CommandTreeBuilder();
            builder.RegisterModule<UnnamedGroupWithNestedUnnamedGroupWithCommands>();

            var tree = builder.Build();
            var root = tree.Root;

            Assert.Equal(4, root.Children.Count);
            Assert.IsType<CommandNode>(root.Children[0]);
            Assert.IsType<CommandNode>(root.Children[1]);
            Assert.IsType<CommandNode>(root.Children[2]);
            Assert.IsType<CommandNode>(root.Children[3]);

            var command1 = (CommandNode)root.Children[0];
            var command2 = (CommandNode)root.Children[1];
            var command3 = (CommandNode)root.Children[2];
            var command4 = (CommandNode)root.Children[3];

            Assert.Equal("a", command1.Key);
            Assert.Equal("b", command2.Key);
            Assert.Equal("c", command3.Key);
            Assert.Equal("d", command4.Key);
        }
    }
}
