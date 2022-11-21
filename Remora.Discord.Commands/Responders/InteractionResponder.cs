//
//  InteractionResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Commands;
using Remora.Commands.Services;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Commands.Responders;

/// <summary>
/// Responds to interactions.
/// </summary>
[PublicAPI]
public class InteractionResponder : IResponder<IInteractionCreate>
{
    private readonly CommandService _commandService;
    private readonly InteractionResponderOptions _options;
    private readonly IDiscordRestInteractionAPI _interactionAPI;
    private readonly ExecutionEventCollectorService _eventCollector;
    private readonly IServiceProvider _services;
    private readonly ContextInjectionService _contextInjection;

    private readonly TokenizerOptions _tokenizerOptions;
    private readonly TreeSearchOptions _treeSearchOptions;

    private readonly ITreeNameResolver? _treeNameResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionResponder"/> class.
    /// </summary>
    /// <param name="commandService">The command service.</param>
    /// <param name="options">The options.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    /// <param name="eventCollector">The event collector.</param>
    /// <param name="services">The available services.</param>
    /// <param name="contextInjection">The context injection service.</param>
    /// <param name="tokenizerOptions">The tokenizer options.</param>
    /// <param name="treeSearchOptions">The tree search options.</param>
    /// <param name="treeNameResolver">The tree name resolver, if available.</param>
    public InteractionResponder
    (
        CommandService commandService,
        IOptions<InteractionResponderOptions> options,
        IDiscordRestInteractionAPI interactionAPI,
        ExecutionEventCollectorService eventCollector,
        IServiceProvider services,
        ContextInjectionService contextInjection,
        IOptions<TokenizerOptions> tokenizerOptions,
        IOptions<TreeSearchOptions> treeSearchOptions,
        ITreeNameResolver? treeNameResolver = null
    )
    {
        _commandService = commandService;
        _options = options.Value;
        _eventCollector = eventCollector;
        _services = services;
        _contextInjection = contextInjection;
        _interactionAPI = interactionAPI;

        _tokenizerOptions = tokenizerOptions.Value;
        _treeSearchOptions = treeSearchOptions.Value;

        _treeNameResolver = treeNameResolver;
    }

    /// <inheritdoc />
    public virtual async Task<Result> RespondAsync
    (
        IInteractionCreate gatewayEvent,
        CancellationToken ct = default
    )
    {
        if (gatewayEvent.Type != InteractionType.ApplicationCommand)
        {
            return Result.FromSuccess();
        }

        if (!gatewayEvent.Data.IsDefined(out var data) || !data.TryPickT0(out var commandData, out _))
        {
            return Result.FromSuccess();
        }

        // Provide the created context to any services inside this scope
        var operationContext = new InteractionContext(gatewayEvent);
        _contextInjection.Context = operationContext;

        commandData.UnpackInteraction(out var commandPath, out var parameters);

        string? treeName = null;
        var allowDefaultTree = false;
        if (_treeNameResolver is not null)
        {
            var getTreeName = await _treeNameResolver.GetTreeNameAsync(operationContext, ct);
            if (!getTreeName.IsSuccess)
            {
                return (Result)getTreeName;
            }

            (treeName, allowDefaultTree) = getTreeName.Entity;
        }

        var executeResult = await TryExecuteCommandAsync(operationContext, commandPath, parameters, treeName, ct);

        var tryDefaultTree = allowDefaultTree && (treeName is not null || treeName != Constants.DefaultTreeName);
        if (executeResult.IsSuccess || !tryDefaultTree)
        {
            return executeResult;
        }

        return await TryExecuteCommandAsync(operationContext, commandPath, parameters, null, ct);
    }

    private async Task<Result> TryExecuteCommandAsync
    (
        InteractionContext operationContext,
        IReadOnlyList<string> commandPath,
        IReadOnlyDictionary<string, IReadOnlyList<string>> parameters,
        string? treeName = null,
        CancellationToken ct = default
    )
    {
        var prepareCommand = await _commandService.TryPrepareCommandAsync
        (
            commandPath,
            parameters,
            _services,
            searchOptions: _treeSearchOptions,
            tokenizerOptions: _tokenizerOptions,
            treeName: treeName,
            ct: ct
        );

        if (!prepareCommand.IsSuccess)
        {
            var preparationError = await _eventCollector.RunPreparationErrorEvents(_services, operationContext, ct);
            if (!preparationError.IsSuccess)
            {
                return preparationError;
            }

            return (Result)prepareCommand;
        }

        var preparedCommand = prepareCommand.Entity;

        // Update the available context
        var commandContext = new InteractionCommandContext
        (
            operationContext.Interaction,
            preparedCommand
        )
        {
            HasRespondedToInteraction = operationContext.HasRespondedToInteraction
        };

        _contextInjection.Context = commandContext;

        var suppressResponseAttribute = preparedCommand.Command.Node
            .FindCustomAttributeOnLocalTree<SuppressInteractionResponseAttribute>();

        var shouldSendResponse =
        !(
            suppressResponseAttribute?.Suppress ?? _options.SuppressAutomaticResponses ||
            commandContext.HasRespondedToInteraction
        );

        if (shouldSendResponse)
        {
            // Signal Discord that we'll be handling this one asynchronously
            var response = new InteractionResponse(InteractionCallbackType.DeferredChannelMessageWithSource);

            var ephemeralAttribute = preparedCommand.Command.Node
                .FindCustomAttributeOnLocalTree<EphemeralAttribute>();

            var sendEphemeral = (ephemeralAttribute is null && _options.UseEphemeralResponses) ||
                                ephemeralAttribute?.IsEphemeral == true;

            if (sendEphemeral)
            {
                response = response with
                {
                    Data = new(new InteractionMessageCallbackData(Flags: MessageFlags.Ephemeral))
                };
            }

            var interactionResponse = await _interactionAPI.CreateInteractionResponseAsync
            (
                commandContext.Interaction.ID,
                commandContext.Interaction.Token,
                response,
                ct: ct
            );

            if (!interactionResponse.IsSuccess)
            {
                return interactionResponse;
            }

            operationContext.HasRespondedToInteraction = true;
            commandContext.HasRespondedToInteraction = true;
        }

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
