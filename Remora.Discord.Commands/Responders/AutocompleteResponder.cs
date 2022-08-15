//
//  AutocompleteResponder.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Autocomplete;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Commands.Responders;

/// <summary>
/// Responds to autocompletion interactions, routing the request to the appropriate provider.
/// </summary>
[PublicAPI]
public class AutocompleteResponder : IResponder<IInteractionCreate>
{
    private readonly SlashService _slashService;
    private readonly IServiceProvider _services;
    private readonly IDiscordRestInteractionAPI _interactionAPI;
    private readonly ILogger<AutocompleteResponder> _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocompleteResponder"/> class.
    /// </summary>
    /// <param name="slashService">The slash command service.</param>
    /// <param name="services">The available services.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    /// <param name="log">The logging instance.</param>
    public AutocompleteResponder
    (
        SlashService slashService,
        IServiceProvider services,
        IDiscordRestInteractionAPI interactionAPI,
        ILogger<AutocompleteResponder> log)
    {
        _slashService = slashService;
        _log = log;
        _services = services;
        _interactionAPI = interactionAPI;
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = default)
    {
        if (gatewayEvent.Type is not InteractionType.ApplicationCommandAutocomplete)
        {
            return Result.FromSuccess();
        }

        if (!gatewayEvent.Data.IsDefined(out var data) || !data.TryPickT0(out var autocompleteData, out _))
        {
            return Result.FromSuccess();
        }

        var createContext = gatewayEvent.CreateContext();
        if (!createContext.IsSuccess)
        {
            return (Result)createContext;
        }

        var context = createContext.Entity;

        if (!autocompleteData.Options.IsDefined(out var options))
        {
            return new InvalidOperationError("Autocomplete interaction without options received. Bug?");
        }

        // Check for a global command
        if (!_slashService.CommandMap.TryGetValue((default, autocompleteData.ID), out var value))
        {
            // Check for a guild command
            if (!_slashService.CommandMap.TryGetValue((gatewayEvent.GuildID, autocompleteData.ID), out value))
            {
                return new InvalidOperationError
                (
                    "Corresponding command node or submap not found when responding to an autocomplete request. Desync?"
                );
            }
        }

        autocompleteData.UnpackInteraction(out var path, out _);
        var commandNode = value.Match
        (
            map => map[string.Join("::", path)],
            node => node
        );

        if (!TryFindFocusedParameter(autocompleteData, out var focusedParameter))
        {
            return new InvalidOperationError("Autocomplete interaction without focused option received. Desync?");
        }

        var realParameter = commandNode.CommandMethod.GetParameters().First
        (
            p => p.Name is not null && p.Name.Equals(focusedParameter.Name, StringComparison.OrdinalIgnoreCase)
        );

        var contextInjector = _services.GetRequiredService<ContextInjectionService>();
        contextInjector.Context = context;

        // Time to look up our provider
        IAutocompleteProvider? autocompleteProvider;

        // First, check if the parameter wants a specific provider
        var providerAttribute = realParameter.GetCustomAttribute<AutocompleteProviderAttribute>();
        if (providerAttribute is not null)
        {
            // look up the requested provider
            autocompleteProvider = _services.GetServices<IAutocompleteProvider>().FirstOrDefault
            (
                p => p.Identity == providerAttribute.ProviderIdentity
            );

            if (autocompleteProvider is null)
            {
                _log.LogWarning
                (
                    "No autocomplete provider could be found for a parameter that had autocompletion enabled " +
                    "(requested a provider with the identity \"{Identity}\")",
                    providerAttribute.ProviderIdentity
                );
            }
        }
        else
        {
            // see if there's an autocomplete provider registered for the parameter type
            var parameterType = realParameter.ParameterType;
            var autocompleteProviderType = typeof(IAutocompleteProvider<>).MakeGenericType(parameterType);
            autocompleteProvider = (IAutocompleteProvider?)_services.GetService(autocompleteProviderType);

            if (autocompleteProvider is null)
            {
                _log.LogWarning
                (
                    "No autocomplete provider could be found for a parameter that had autocompletion enabled " +
                    "(requested a provider for the type \"{Type}\")",
                    parameterType.Name
                );
            }
        }

        var userInput = focusedParameter.Value.IsDefined(out var inputValue)
            ? inputValue.Value.ToString() ?? string.Empty
            : string.Empty;

        var suggestions = autocompleteProvider is null
            ? Array.Empty<IApplicationCommandOptionChoice>()
            : await autocompleteProvider.GetSuggestionsAsync(options, userInput, ct);

        return await _interactionAPI.CreateInteractionResponseAsync
        (
            gatewayEvent.ID,
            gatewayEvent.Token,
            new InteractionResponse
            (
                InteractionCallbackType.ApplicationCommandAutocompleteResult,
                new(new InteractionAutocompleteCallbackData(suggestions.Take(25).ToList()))
            ),
            ct: ct
        );
    }

    private static bool TryFindFocusedParameter
    (
        IApplicationCommandData data,
        [NotNullWhen(true)] out IApplicationCommandInteractionDataOption? focusedParameter
    )
    {
        focusedParameter = null;

        if (!data.Options.IsDefined(out var options))
        {
            return false;
        }

        while (true)
        {
            if (options.Count is 0)
            {
                return false;
            }

            focusedParameter = options.FirstOrDefault(o => o.IsFocused.HasValue && o.IsFocused.Value);
            if (focusedParameter is not null)
            {
                // Found it
                return true;
            }

            // More than one path through this; no go
            if (options.Count > 1)
            {
                return false;
            }

            if (!options[0].Options.IsDefined(out options))
            {
                return false;
            }
        }
    }
}
