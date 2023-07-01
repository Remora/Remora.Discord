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
        var context = new MessageContext
        (
            gatewayEvent,
            gatewayEvent.GuildID
        );

        if (!context.Message.Author.TryGet(out var author))
        {
            return Result.FromSuccess();
        }

        if (author.IsBot.TryGet(out var isBot) && isBot)
        {
            return Result.FromSuccess();
        }

        if (author.IsSystem.TryGet(out var isSystem) && isSystem)
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
        if (!gatewayEvent.Content.TryGet(out var content))
        {
            return Result.FromSuccess();
        }

        var context = new MessageContext(gatewayEvent, gatewayEvent.GuildID);

        if (!context.Message.Author.TryGet(out var author))
        {
            return Result.FromSuccess();
        }

        if (author.IsBot.TryGet(out var isBot) && isBot)
        {
            return Result.FromSuccess();
        }

        if (author.IsSystem.TryGet(out var isSystem) && isSystem)
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
    /// <param name="operationContext">The command context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    protected virtual async Task<Result> ExecuteCommandAsync
    (
        string content,
        MessageContext operationContext,
        CancellationToken ct = default
    )
    {
        // Provide the created context to any services inside this scope
        _contextInjection.Context = operationContext;

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

        if (_treeNameResolver is null)
        {
            return await TryExecuteCommandAsync(operationContext, content, null, ct);
        }

        var getTreeName = await _treeNameResolver.GetTreeNameAsync(operationContext, ct);
        if (!getTreeName.IsSuccess)
        {
            return (Result)getTreeName;
        }

        var treeName = getTreeName.Entity;
        return await TryExecuteCommandAsync(operationContext, content, treeName, ct);
    }

    private async Task<Result> TryExecuteCommandAsync
    (
        MessageContext operationContext,
        string content,
        string? treeName = null,
        CancellationToken ct = default
    )
    {
        var prepareCommand = await _commandService.TryPrepareCommandAsync
        (
            content,
            _services,
            _tokenizerOptions,
            _treeSearchOptions,
            treeName,
            ct
        );

        if (!prepareCommand.IsSuccess)
        {
            var preparationError = await _eventCollector.RunPreparationErrorEvents
            (
                _services,
                operationContext,
                prepareCommand,
                ct
            );

            // check if the result has changed, since the error events might change the outcome
            if (!preparationError.IsSuccess && !Equals(preparationError.Error, prepareCommand.Error))
            {
                return preparationError;
            }

            // eat the error if it's not a developer problem
            if (prepareCommand.Error.IsUserOrEnvironmentError())
            {
                // We've done our part and notified whoever might be interested; job well done
                return Result.FromSuccess();
            }

            return (Result)prepareCommand;
        }

        var preparedCommand = prepareCommand.Entity;

        // Update the available context
        var commandContext = new TextCommandContext
        (
            operationContext.Message,
            operationContext.GuildID,
            preparedCommand
        );

        _contextInjection.Context = commandContext;

        // Run any user-provided pre-execution events
        var preExecution = await _eventCollector.RunPreExecutionEvents(_services, commandContext, ct);
        if (!preExecution.IsSuccess)
        {
            return preExecution;
        }

        var executionResult = await _commandService.TryExecuteAsync(preparedCommand, _services, ct);

        // Run any user-provided post-execution events
        return await _eventCollector.RunPostExecutionEvents
        (
            _services,
            commandContext,
            executionResult.IsSuccess ? executionResult.Entity : executionResult,
            ct
        );
    }
}
