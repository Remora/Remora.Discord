//
//  InteractivityResponder.cs
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
using Humanizer;
using Microsoft.Extensions.Options;
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
using Remora.Discord.Commands.Responders;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Interactivity;

/// <summary>
/// Handles dispatch of interaction events to interested entities.
/// </summary>
internal sealed class InteractivityResponder : IResponder<IInteractionCreate>
{
    private readonly ContextInjectionService _contextInjection;
    private readonly IDiscordRestInteractionAPI _interactionAPI;
    private readonly IServiceProvider _services;
    private readonly InteractionResponderOptions _interactionOptions;
    private readonly InteractivityResponderOptions _options;
    private readonly ExecutionEventCollectorService _eventCollector;
    private readonly CommandService _commandService;

    private readonly TokenizerOptions _tokenizerOptions;
    private readonly TreeSearchOptions _treeSearchOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractivityResponder"/> class.
    /// </summary>
    /// <param name="commandService">The command service.</param>
    /// <param name="interactionOptions">The interaction responder options.</param>
    /// <param name="options">The responder options.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    /// <param name="services">The available services.</param>
    /// <param name="contextInjection">The context injection service.</param>
    /// <param name="tokenizerOptions">The tokenizer options.</param>
    /// <param name="treeSearchOptions">The tree search options.</param>
    /// <param name="eventCollector">The execution event collector.</param>
    public InteractivityResponder
    (
        CommandService commandService,
        IOptions<InteractionResponderOptions> interactionOptions,
        IOptions<InteractivityResponderOptions> options,
        IDiscordRestInteractionAPI interactionAPI,
        IServiceProvider services,
        ContextInjectionService contextInjection,
        IOptions<TokenizerOptions> tokenizerOptions,
        IOptions<TreeSearchOptions> treeSearchOptions,
        ExecutionEventCollectorService eventCollector
    )
    {
        _services = services;
        _contextInjection = contextInjection;
        _eventCollector = eventCollector;
        _interactionAPI = interactionAPI;
        _commandService = commandService;

        _interactionOptions = interactionOptions.Value;
        _options = options.Value;

        _tokenizerOptions = tokenizerOptions.Value;
        _treeSearchOptions = treeSearchOptions.Value;
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = default)
    {
        if (gatewayEvent.Type is not (InteractionType.MessageComponent or InteractionType.ModalSubmit))
        {
            return Result.FromSuccess();
        }

        if (!gatewayEvent.Data.TryGet(out var data))
        {
            return new InvalidOperationError("Component or modal interaction without data received. Bug?");
        }

        var context = new InteractionContext(gatewayEvent);
        _contextInjection.Context = context;

        return data.TryPickT1(out var componentData, out var remainder)
            ? await HandleComponentInteractionAsync(context, componentData, ct)
            : remainder.TryPickT1(out var modalSubmitData, out _)
                ? await HandleModalInteractionAsync(context, modalSubmitData, ct)
                : Result.FromSuccess();
    }

    private async Task<Result> HandleComponentInteractionAsync
    (
        IInteractionContext context,
        IMessageComponentData data,
        CancellationToken ct = default
    )
    {
        if (!data.CustomID.StartsWith(Constants.InteractionTree))
        {
            // Not a component we handle
            return Result.FromSuccess();
        }

        var isSelectMenu = data.ComponentType is ComponentType.StringSelect
            or ComponentType.UserSelect
            or ComponentType.RoleSelect
            or ComponentType.MentionableSelect
            or ComponentType.ChannelSelect;

        if (isSelectMenu && !data.Values.HasValue)
        {
            return new InvalidOperationError("The interaction did not contain any selected values.");
        }

        var commandPath = data.CustomID[Constants.InteractionTree.Length..][2..]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        commandPath = ExtractState(commandPath, out var state);

        var buildParameters = data.ComponentType switch
        {
            ComponentType.Button => new Dictionary<string, IReadOnlyList<string>>(),
            ComponentType.StringSelect => BuildParametersFromStringData(data),
            ComponentType.UserSelect
                or ComponentType.RoleSelect
                or ComponentType.MentionableSelect
                or ComponentType.ChannelSelect
                => BuildParametersFromResolvedData(data),
            _ => new InvalidOperationError("An unsupported component type was encountered.")
        };

        if (!buildParameters.IsSuccess)
        {
            return (Result)buildParameters;
        }

        var parameters = buildParameters.Entity;

        if (state != null)
        {
            parameters.Add("state", new[] { state });
        }

        return await TryExecuteCommandAsync(context, commandPath, parameters, ct);
    }

    private static Result<Dictionary<string, IReadOnlyList<string>>> BuildParametersFromStringData
    (
        IMessageComponentData data
    )
    {
        // In the case that the developer defined options are similar to Snowflakes, they'll
        // be incorrectly parsed. Hence, we have to handle either case for string selects.
        var values = data.Values.Value.TryPickT1(out var stringValues, out var snowflakeValues)
            ? stringValues
            : snowflakeValues.Select(x => x.ToString()).ToList();

        return new Dictionary<string, IReadOnlyList<string>>
        {
            { "values", values }
        };
    }

    private static Result<Dictionary<string, IReadOnlyList<string>>> BuildParametersFromResolvedData
    (
        IMessageComponentData data
    )
    {
        var parameters = new Dictionary<string, IReadOnlyList<string>>();

        var values = new HashSet<Snowflake>();
        if (data.Values.Value.TryPickT0(out var snowflakes, out _))
        {
            foreach (var snowflake in snowflakes)
            {
                values.Add(snowflake);
            }
        }

        if (!data.Resolved.TryGet(out var resolved))
        {
            return parameters;
        }

        if (resolved.Users.TryGet(out var users))
        {
            parameters.Add
            (
                "users",
                users.Keys.Where(x => values.Contains(x))
                    .Select(x => x.ToString())
                    .ToList()
            );
        }

        if (resolved.Roles.TryGet(out var roles))
        {
            parameters.Add
            (
                "roles",
                roles.Keys.Where(x => values.Contains(x))
                    .Select(x => x.ToString())
                    .ToList()
            );
        }

        if (resolved.Channels.TryGet(out var channels))
        {
            parameters.Add
            (
                "channels",
                channels.Keys.Where(x => values.Contains(x))
                    .Select(x => x.ToString())
                    .ToList()
            );
        }

        return parameters;
    }

    private async Task<Result> HandleModalInteractionAsync
    (
        IInteractionContext context,
        IModalSubmitData data,
        CancellationToken ct = default
    )
    {
        if (!data.CustomID.StartsWith(Constants.InteractionTree))
        {
            // Not a component we handle
            return Result.FromSuccess();
        }

        var commandPath = data.CustomID[Constants.InteractionTree.Length..][2..]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        commandPath = ExtractState(commandPath, out var state);

        var parameters = ExtractParameters(data.Components);

        if (state != null)
        {
            parameters.Add("state", new[] { state });
        }

        return await TryExecuteCommandAsync
        (
            context,
            commandPath,
            parameters,
            ct
        );
    }

    private static Dictionary<string, IReadOnlyList<string>> ExtractParameters
    (
        IEnumerable<IPartialMessageComponent> components
    )
    {
        var parameters = new Dictionary<string, IReadOnlyList<string>>();
        foreach (var component in components)
        {
            if (component is IPartialActionRowComponent actionRow)
            {
                if (!actionRow.Components.TryGet(out var rowComponents))
                {
                    continue;
                }

                var nestedComponents = ExtractParameters(rowComponents);
                foreach (var nestedComponent in nestedComponents)
                {
                    parameters.Add(nestedComponent.Key, nestedComponent.Value);
                }

                continue;
            }

            switch (component)
            {
                case IPartialTextInputComponent textInput:
                {
                    if (!textInput.CustomID.TryGet(out var id))
                    {
                        continue;
                    }

                    if (!textInput.Value.TryGet(out var value))
                    {
                        continue;
                    }

                    parameters.Add(id.Replace('-', '_').Camelize(), new[] { value });
                    break;
                }
                case IPartialStringSelectComponent selectMenu:
                {
                    if (!selectMenu.CustomID.TryGet(out var id))
                    {
                        continue;
                    }

                    if (!selectMenu.Options.TryGet(out var options))
                    {
                        continue;
                    }

                    var values = options.Where(op => op.Value.HasValue).Select(op => op.Value.Value).ToList();

                    parameters.Add(id.Replace('-', '_').Camelize(), values);
                    break;
                }
            }
        }

        return parameters;
    }

    private async Task<Result> TryExecuteCommandAsync
    (
        IInteractionContext operationContext,
        IReadOnlyList<string> commandPath,
        IReadOnlyDictionary<string, IReadOnlyList<string>> parameters,
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
            treeName: Constants.InteractionTree,
            ct: ct
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

            if (!preparationError.IsSuccess)
            {
                return preparationError;
            }

            if (prepareCommand.Error.IsUserOrEnvironmentError())
            {
                // We've done our part and notified whoever might be interested; job well done
                return Result.FromSuccess();
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
            suppressResponseAttribute?.Suppress ??
            (_options.SuppressAutomaticResponses || commandContext.HasRespondedToInteraction)
        );

        if (shouldSendResponse)
        {
            var response = new InteractionResponse(InteractionCallbackType.DeferredUpdateMessage);

            var ephemeralAttribute = preparedCommand.Command.Node
                .FindCustomAttributeOnLocalTree<EphemeralAttribute>();

            var sendEphemeral = (ephemeralAttribute is null && _interactionOptions.UseEphemeralResponses) ||
                                ephemeralAttribute?.IsEphemeral == true;

            if (sendEphemeral)
            {
                response = response with
                {
                    Data = new(new InteractionMessageCallbackData(Flags: MessageFlags.Ephemeral))
                };
            }

            var createResponse = await _interactionAPI.CreateInteractionResponseAsync
            (
                commandContext.Interaction.ID,
                commandContext.Interaction.Token,
                response,
                ct: ct
            );

            if (!createResponse.IsSuccess)
            {
                return createResponse;
            }

            operationContext.HasRespondedToInteraction = true;
            operationContext.IsOriginalEphemeral = sendEphemeral;

            commandContext.HasRespondedToInteraction = true;
            commandContext.IsOriginalEphemeral = sendEphemeral;
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

    private static string[] ExtractState(string[] commandPath, out string? state)
    {
        if (commandPath.Length <= 0 || !commandPath[0].StartsWith(Constants.StatePrefix))
        {
            state = null;
            return commandPath;
        }

        state = commandPath[0][Constants.StatePrefix.Length..];
        return commandPath[1..];
    }
}
