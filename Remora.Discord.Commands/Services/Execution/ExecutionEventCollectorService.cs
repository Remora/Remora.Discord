//
//  ExecutionEventCollectorService.cs
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

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Collects execution event services for simpler conjoined execution.
/// </summary>
[PublicAPI]
public class ExecutionEventCollectorService
{
    /// <summary>
    /// Runs all collected pre-execution events.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="commandContext">The command context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result> RunPreExecutionEvents
    (
        IServiceProvider services,
        ICommandContext commandContext,
        CancellationToken ct
    )
    {
        var results = await Task.WhenAll
        (
            services
                .GetServices<IPreExecutionEvent>()
                .Select(e => e.BeforeExecutionAsync(commandContext, ct))
        );

        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Runs all collected post-execution events.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="commandContext">The command context.</param>
    /// <param name="commandResult">The result of the executed command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result> RunPostExecutionEvents
    (
        IServiceProvider services,
        ICommandContext commandContext,
        IResult commandResult,
        CancellationToken ct
    )
    {
        var results = await Task.WhenAll
        (
            services
                .GetServices<IPostExecutionEvent>()
                .Select(e => e.AfterExecutionAsync(commandContext, commandResult, ct))
        );

        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.FromSuccess();
    }
}
