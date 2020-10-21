//
//  CommandTree.cs
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

using System;
using Remora.Commands.Extensions;
using Remora.Commands.Results;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees.Nodes;

namespace Remora.Commands.Trees
{
    /// <summary>
    /// Represents a tree view of the available commands.
    /// </summary>
    public class CommandTree
    {
        /// <summary>
        /// Gets the root of the command tree.
        /// </summary>
        public RootNode Root { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTree"/> class.
        /// </summary>
        /// <param name="root">The root of the command tree.</param>
        public CommandTree(RootNode root)
        {
            this.Root = root;
        }

        /// <summary>
        /// Searches the command tree for a command that matches the given command string.
        /// </summary>
        /// <param name="commandString">The raw command string.</param>
        /// <returns>A search result which may or may not have succeeded.</returns>
        public CommandSearchResult Search(ReadOnlySpan<char> commandString)
        {
            var tokenizer = new TokenizingEnumerator(commandString);

            IParentNode currentLevel = this.Root;
            while (true)
            {
                if (!tokenizer.MoveNext())
                {
                    return CommandSearchResult.FromError("No matching command found.");
                }

                if (tokenizer.Current.Type != TokenType.Value)
                {
                    return CommandSearchResult.FromError("No matching command found.");
                }

                var currentToken = tokenizer.Current;

                IChildNode? matchingChild = null;
                foreach (var child in currentLevel.Children)
                {
                    if (child.Key != currentToken.Value)
                    {
                        continue;
                    }

                    matchingChild = child;
                }

                switch (matchingChild)
                {
                    case CommandNode commandNode when commandNode.SignatureMatches(tokenizer):
                    {
                        return commandNode;
                    }
                    case GroupNode groupNode:
                    {
                        currentLevel = groupNode;
                        continue;
                    }
                    case null:
                    {
                        return CommandSearchResult.FromError("No matching command found.");
                    }
                    default:
                    {
                        throw new InvalidOperationException
                        (
                            "Unknown node type encountered; tree is invalid and the search cannot continue."
                        );
                    }
                }
            }
        }
    }
}
