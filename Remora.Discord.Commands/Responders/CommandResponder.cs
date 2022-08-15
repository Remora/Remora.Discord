//
//  CommandResponder.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Commands;
using Remora.Commands.Services;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Commands.Responders;

/// <summary>
/// Responds to commands.
/// </summary>
[PublicAPI]
public class CommandResponder : IResponder<IMessageCreate>, IResponder<IMessageUpdate>
{
    private readonly CommandService _commandService;
    private readonly ICommandResponderOptions _options;
    private readonly ExecutionEventCollectorService _eventCollector;
    private readonly IServiceProvider _services;
    private readonly ContextInjectionService _contextInjection;

    private readonly TokenizerOptions _tokenizerOptions;
    private readonly TreeSearchOptions _treeSearchOptions;

    private readonly ITreeNameResolver? _treeNameResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandResponder"/> class.
    /// </summary>
    /// <param name="commandService">The command service.</param>
    /// <param name="options">The command responder options.</param>
    /// <param name="eventCollector">The event collector.</param>
    /// <param name="services">The available services.</param>
    /// <param name="contextInjection">The injection service.</param>
    /// <param name="tokenizerOptions">The tokenizer options.</param>
    /// <param name="treeSearchOptions">The tree search options.</param>
    /// <param name="treeNameResolver">The tree name resolver, if one is available.</param>
    public CommandResponder
    (
        CommandService commandService,
        IOptions<CommandResponderOptions> options,
        ExecutionEventCollectorService eventCollector,
        IServiceProvider services,
        ContextInjectionService contextInjection,
        IOptions<TokenizerOptions> tokenizerOptions,
        IOptions<TreeSearchOptions> treeSearchOptions,
        ITreeNameResolver? treeNameResolver = null
    )
    {
        _commandService = commandService;
        _services = services;
        _contextInjection = contextInjection;
        _eventCollector = eventCollector;
        _options = options.Value;

        _tokenizerOptions = tokenizerOptions.Value;
        _treeSearchOptions = treeSearchOptions.Value;

        _treeNameResolver = treeNameResolver;
    }

    /// <inheritdoc/>
    public virtual async Task<Result> RespondAsync
    (
        IMessageCreate gatewayEvent,
        CancellationToken ct = default
    )
    {
        var createContext = gatewayEvent.CreateContext();
        if (!createContext.IsSuccess)
        {
            return (Result)createContext;
        }

        var context = createContext.Entity;

        var author = context.User;
        if (author.IsBot.IsDefined(out var isBot) && isBot)
        {
            return Result.FromSuccess();
        }

        if (author.IsSystem.IsDefined(out var isSystem) && isSystem)
        {
            return Result.FromSuccess();
        }

        return await ExecuteCommandAsync(gatewayEvent.Content, context, ct);
    }

    /// <inheritdoc/>
    public virtual async Task<Result> RespondAsync
    (
        IMessageUpdate gatewayEvent,
        CancellationToken ct = default
    )
    {
        if (!gatewayEvent.Content.IsDefined(out var content))
        {
            return Result.FromSuccess();
        }

        var createContext = gatewayEvent.CreateContext();
        if (!createContext.IsSuccess)
        {
            return (Result)createContext;
        }

        var context = createContext.Entity;

        var author = context.User;
        if (author.IsBot.IsDefined(out var isBot) && isBot)
        {
            return Result.FromSuccess();
        }

        if (author.IsSystem.IsDefined(out var isSystem) && isSystem)
        {
            return Result.FromSuccess();
        }

        if (gatewayEvent.EditedTimestamp.IsDefined(out var edited))
        {
            // Check if the edit happened in the last three seconds; if so, we'll assume this isn't some other
            // change made to the message object
            var interval = DateTimeOffset.UtcNow - edited;
            if (interval >= TimeSpan.FromSeconds(3))
            {
                return Result.FromSuccess();
            }
        }
        else
        {
            // No edit means no visible change to the command
            return Result.FromSuccess();
        }

        return await ExecuteCommandAsync(content, context, ct);
    }

    /// <summary>
    /// Executes the actual command.
    /// </summary>
    /// <param name="content">The contents of the message.</param>
    /// <param name="commandContext">The command context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    protected virtual async Task<Result> ExecuteCommandAsync
    (
        string content,
        ICommandContext commandContext,
        CancellationToken ct = default
    )
    {
        // Provide the created context to any services inside this scope
        _contextInjection.Context = commandContext;

        var prefixMatcher = _services.GetRequiredService<ICommandPrefixMatcher>();
        var checkPrefix = await prefixMatcher.MatchesPrefixAsync(content, ct);
        if (!checkPrefix.IsDefined(out var check))
        {
            return (Result)checkPrefix;
        }

        if (!check.Matches)
        {
            return Result.FromSuccess();
        }

        // Strip off the prefix
        content = content[check.ContentStartIndex..];

        // Run any user-provided pre execution events
        var preExecution = await _eventCollector.RunPreExecutionEvents(_services, commandContext, ct);
        if (!preExecution.IsSuccess)
        {
            return preExecution;
        }

        string? treeName = null;
        var allowDefaultTree = false;
        if (_treeNameResolver is not null)
        {
            var getTreeName = await _treeNameResolver.GetTreeNameAsync(commandContext, ct);
            if (!getTreeName.IsSuccess)
            {
                return (Result)getTreeName;
            }

            (treeName, allowDefaultTree) = getTreeName.Entity;
        }

        // Run the actual command
        var executeResult = await _commandService.TryExecuteAsync
        (
            content,
            _services,
            _tokenizerOptions,
            _treeSearchOptions,
            treeName,
            ct
        );

        var tryDefaultTree = allowDefaultTree && (treeName is not null || treeName != Constants.DefaultTreeName);
        if (executeResult.IsSuccess || !tryDefaultTree)
        {
            return await _eventCollector.RunPostExecutionEvents
            (
                _services,
                commandContext,
                executeResult.IsSuccess ? executeResult.Entity : executeResult,
                ct
            );
        }

        var oldResult = executeResult;
        executeResult = await _commandService.TryExecuteAsync
        (
            content,
            _services,
            _tokenizerOptions,
            _treeSearchOptions,
            ct: ct
        );

        if (!executeResult.IsSuccess)
        {
            executeResult = new AggregateError(oldResult, executeResult);
        }

        // Run any user-provided post execution events, passing along either the result of the command itself, or if
        // execution failed, the reason why
        return await _eventCollector.RunPostExecutionEvents
        (
            _services,
            commandContext,
            executeResult.IsSuccess ? executeResult.Entity : executeResult,
            ct
        );
    }
}
