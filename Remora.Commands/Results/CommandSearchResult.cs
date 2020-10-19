//
//  CommandSearchResult.cs
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
using JetBrains.Annotations;
using Remora.Commands.Trees.Nodes;
using Remora.Results;

namespace Remora.Commands.Results
{
    /// <summary>
    /// Represents a command search result.
    /// </summary>
    [PublicAPI]
    public class CommandSearchResult : ResultBase<CommandSearchResult>
    {
        private CommandNode? _commandNode;

        /// <summary>
        /// Gets the found command, if any.
        /// </summary>
        public CommandNode Command
        {
            get
            {
                if (_commandNode is null || !this.IsSuccess)
                {
                    throw new InvalidOperationException("The result did not contain a value value.");
                }

                return _commandNode;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSearchResult"/> class.
        /// </summary>
        /// <param name="commandNode">The found command.</param>
        protected CommandSearchResult(CommandNode commandNode)
        {
            _commandNode = commandNode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSearchResult"/> class.
        /// </summary>
        /// <param name="errorReason">A more detailed error description.</param>
        /// <param name="exception">The exception that caused the error (if any).</param>
        protected CommandSearchResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }
    }
}
