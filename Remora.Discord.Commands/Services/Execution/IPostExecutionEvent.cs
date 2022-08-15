//
//  IPostExecutionEvent.cs
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

using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Represents the public interface of a service that can perform a post-execution event.
/// </summary>
public interface IPostExecutionEvent
{
    /// <summary>
    /// Runs after a command has been executed, successfully or otherwise.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="commandResult">The result returned by the command.</param>
    /// <param name="ct">The cancellation token of the current operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result> AfterExecutionAsync
    (
        ICommandContext context,
        IResult commandResult,
        CancellationToken ct = default
    );
}
