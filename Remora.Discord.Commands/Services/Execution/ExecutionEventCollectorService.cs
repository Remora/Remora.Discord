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
using System.Collections.Generic;
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
    /// Runs all collected command preparation error events.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="operationContext">The operation context.</param>
    /// <param name="preparationResult">The result of the command preparation.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result> RunPreparationErrorEvents
    (
        IServiceProvider services,
        IOperationContext operationContext,
        IResult preparationResult,
        CancellationToken ct
    )
    {
        var events = services.GetServices<IPreparationErrorEvent>();
        return await RunEvents
        (
            events.Select
            (
                e => new Func<CancellationToken, Task<Result>>
                (
                    token => e.PreparationFailed
                    (
                        operationContext,
                        preparationResult,
                        token
                    )
                )
            ),
            ct
        );
    }

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
        var events = services.GetServices<IPreExecutionEvent>();
        return await RunEvents
        (
            events.Select
            (
                e => new Func<CancellationToken, Task<Result>>
                (
                    token => e.BeforeExecutionAsync
                    (
                        commandContext,
                        token
                    )
                )
            ),
            ct
        );
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
        var events = services.GetServices<IPostExecutionEvent>();
        return await RunEvents
        (
            events.Select
            (
                e => new Func<CancellationToken, Task<Result>>
                (
                    token => e.AfterExecutionAsync
                    (
                        commandContext,
                        commandResult,
                        token
                    )
                )
            ),
            ct
        );
    }

    private async Task<Result> RunEvents
    (
        IEnumerable<Func<CancellationToken, Task<Result>>> events,
        CancellationToken ct
    )
    {
        var errors = new List<Result>();

        foreach (var eventToExecute in events)
        {
            try
            {
                var result = await eventToExecute(ct);
                if (!result.IsSuccess)
                {
                    errors.Add(result);
                }
            }
            catch (Exception e)
            {
                errors.Add(Result.FromError(new ExceptionError(e)));
            }
        }

        if (errors.Count > 0)
        {
            return errors.Count == 1
                ? errors[0]
                : new AggregateError(errors.Cast<IResult>().ToList());
        }

        return Result.FromSuccess();
    }
}
