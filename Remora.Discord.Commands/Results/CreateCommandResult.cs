//
//  CreateCommandResult.cs
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
using Remora.Discord.Rest.Results;
using Remora.Results;

namespace Remora.Discord.Commands.Results
{
    /// <summary>
    /// Represents an attempt to create a Slash command from a Remora.Commands command.
    /// </summary>
    public class CreateCommandResult : ResultBase<CreateCommandResult>
    {
        /// <summary>
        /// Gets the command that failed to convert.
        /// </summary>
        public CommandNode? Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandResult"/> class.
        /// </summary>
        private CreateCommandResult()
        {
        }

        /// <inheritdoc cref="CreateRestEntityResult{TEntity}"/>
        [UsedImplicitly]
        private CreateCommandResult
        (
            string? errorReason,
            CommandNode? command = null
        )
            : this(errorReason, (Exception?)null)
        {
            this.Command = command;
        }

        /// <inheritdoc cref="CreateRestEntityResult{TEntity}"/>
        [UsedImplicitly]
        private CreateCommandResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorReason">A more detailed error reason.</param>
        /// <param name="command">The command node that caused the fault.</param>
        /// <returns>A failed result.</returns>
        public static CreateCommandResult FromError(string errorReason, CommandNode command)
        {
            return new(errorReason, command);
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A successful result.</returns>
        public static CreateCommandResult FromSuccess()
        {
            return new();
        }
    }
}
