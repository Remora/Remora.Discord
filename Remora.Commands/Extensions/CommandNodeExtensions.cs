//
//  CommandNodeExtensions.cs
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

using Remora.Commands.Tokenization;
using Remora.Commands.Trees.Nodes;

namespace Remora.Commands.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="CommandNode"/> class.
    /// </summary>
    public static class CommandNodeExtensions
    {
        /// <summary>
        /// Determines whether the given tokenizer matches the signature of the given command node.
        /// </summary>
        /// <param name="commandNode">The command node to check against.</param>
        /// <param name="tokenizer">The tokenizer with the offered tokens.</param>
        /// <returns>true if the signature matches; otherwise, false.</returns>
        public static bool SignatureMatches(this CommandNode commandNode, TokenizingEnumerator tokenizer)
        {
            // TODO: Check signature
            return true;
        }
    }
}
