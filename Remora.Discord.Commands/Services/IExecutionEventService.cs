//
//  IExecutionEventService.cs
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

using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Commands.Services
{
    /// <summary>
    /// Defines the public API for a service that performs actions related to command execution.
    /// </summary>
    public interface IExecutionEventService
    {
        /// <summary>
        /// Runs before the attempted execution of a command.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="ct">The cancellation token of the current operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Result> BeforeExecutionAsync(ICommandContext context, CancellationToken ct = default);

        /// <summary>
        /// Runs after a command has been successfully executed.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="executionResult">The result of the execution.</param>
        /// <param name="ct">The cancellation token of the current operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Result> AfterExecutionAsync
        (
            ICommandContext context,
            IResult executionResult,
            CancellationToken ct = default
        );
    }
}
