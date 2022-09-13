//
//  CommandServiceExtensions.cs
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Services;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees;
using Remora.Discord.Commands.Services;
using Remora.Results;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// A class of extensions for <see cref="CommandService"/>.
/// </summary>
public static class CommandServiceExtensions
{
    /// <summary>
    /// Prepares and executes a command based on the given input, setting the retrieved node if possible.
    /// </summary>
    /// <param name="this">The instance of <see cref="CommandService"/>.</param>
    /// <param name="commandString">The command string.</param>
    /// <param name="services">The services available to the invocation.</param>
    /// <param name="tokenizerOptions">The tokenizer options.</param>
    /// <param name="searchOptions">A set of search options.</param>
    /// <param name="treeName">The name of the tree to search for the command in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>An execution result which may or may not have succeeded.</returns>
    public static async Task<Result<IResult>> TryPrepareAndExecuteCommandAsync
    (
        this CommandService @this,
        string commandString,
        IServiceProvider services,
        TokenizerOptions? tokenizerOptions = null,
        TreeSearchOptions? searchOptions = null,
        string? treeName = null,
        CancellationToken ct = default
    )
    {
        var nodeInjection = services.GetRequiredService<CommandNodeInjectionService>();
        nodeInjection.Node = null;

        var prepareCommand = await @this.TryPrepareCommandAsync
        (
            commandString,
            services,
            tokenizerOptions,
            searchOptions,
            treeName,
            ct
        );

        if (!prepareCommand.IsSuccess)
        {
            return Result<IResult>.FromError(prepareCommand);
        }

        var command = prepareCommand.Entity;

        nodeInjection.Node = command.Command.Node;

        var executeCommand = await @this.TryExecuteAsync(command, services, ct);

        return executeCommand;
    }
}
