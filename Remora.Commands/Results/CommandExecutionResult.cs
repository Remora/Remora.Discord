//
//  CommandExecutionResult.cs
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
using Remora.Results;

namespace Remora.Commands.Results
{
    /// <summary>
    /// Represents a command search result.
    /// </summary>
    [PublicAPI]
    public class CommandExecutionResult : ResultBase<CommandExecutionResult>
    {
        /// <summary>
        /// Gets the found command, if any.
        /// </summary>
        public ExecutionStatus Status { get; private set; }

        /// <summary>
        /// Gets the result that the executed command returned, if any.
        /// </summary>
        public IResult? InnerResult { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionResult"/> class.
        /// </summary>
        /// <param name="commandNode">The found command.</param>
        protected CommandExecutionResult(ExecutionStatus commandNode)
        {
            this.Status = commandNode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionResult"/> class.
        /// </summary>
        /// <param name="innerResult">The result from the command execution.</param>
        protected CommandExecutionResult(IResult innerResult)
        {
            this.Status = ExecutionStatus.Successful;
            this.InnerResult = innerResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionResult"/> class.
        /// </summary>
        /// <param name="errorReason">A more detailed error description.</param>
        /// <param name="exception">The exception that caused the error (if any).</param>
        protected CommandExecutionResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
            this.Status = !(exception is null)
                ? ExecutionStatus.Faulted
                : ExecutionStatus.Failed;
        }

        /// <summary>
        /// Creates a "not found" failed result.
        /// </summary>
        /// <returns>The result.</returns>
        public static CommandExecutionResult NotFound()
        {
            var result = new CommandExecutionResult("No matching command was found.")
            {
                Status = ExecutionStatus.NotFound
            };

            return result;
        }

        /// <summary>
        /// Creates a faulted result.
        /// </summary>
        /// <param name="exception">The exception that caused the fault.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Faulted(Exception exception)
        {
            var result = FromError(exception);
            result.Status = ExecutionStatus.Faulted;

            return result;
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="result">The failed base result.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Failed(IResult result)
        {
            var newResult = FromError(result);
            newResult.Status = ExecutionStatus.Failed;

            return newResult;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="inner">The inner result.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult FromSuccess(IResult inner)
        {
            return new CommandExecutionResult(inner);
        }
    }
}
