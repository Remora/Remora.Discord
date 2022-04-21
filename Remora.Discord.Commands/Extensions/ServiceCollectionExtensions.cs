//
//  ServiceCollectionExtensions.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Remora.Commands.Extensions;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees;
using Remora.Discord.Commands.Autocomplete;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Commands.Feedback.Themes;
using Remora.Discord.Commands.Parsers;
using Remora.Discord.Commands.Responders;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Extensions;
using Remora.Extensions.Options.Immutable;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all services required for Discord-integrated commands.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="enableSlash">Whether to enable slash commands.</param>
    /// <param name="useDefaultCommandResponder">Whether to add a default command responder.</param>
    /// <param name="useDefaultInteractionResponder">Whether to add a default interaction responder.</param>
    /// <returns>The service collection, with slash commands.</returns>
    public static IServiceCollection AddDiscordCommands
    (
        this IServiceCollection serviceCollection,
        bool enableSlash = false,
        bool useDefaultCommandResponder = true,
        bool useDefaultInteractionResponder = true
    )
    {
        // Add the helpers used for context injection.
        serviceCollection
            .TryAddScoped<ContextInjectionService>();

        // Set up context injection
        serviceCollection
            .TryAddTransient
            (
                s =>
                {
                    var injectionService = s.GetRequiredService<ContextInjectionService>();
                    return injectionService.Context ?? throw new InvalidOperationException
                    (
                        "No context has been set for this scope."
                    );
                }
            );

        serviceCollection
            .TryAddTransient
            (
                s =>
                {
                    var injectionService = s.GetRequiredService<ContextInjectionService>();
                    return injectionService.Context as MessageContext ?? throw new InvalidOperationException
                    (
                        "No message context has been set for this scope."
                    );
                }
            );

        serviceCollection
            .TryAddTransient
            (
                s =>
                {
                    var injectionService = s.GetRequiredService<ContextInjectionService>();
                    return injectionService.Context as InteractionContext ?? throw new InvalidOperationException
                    (
                        "No interaction context has been set for this scope."
                    );
                }
            );

        // Configure option types
        serviceCollection.Configure<TokenizerOptions>(opt => opt);
        serviceCollection.Configure<TreeSearchOptions>
        (
            opt => opt with { KeyComparison = StringComparison.OrdinalIgnoreCase }
        );

        serviceCollection.AddCommands();

        // Add the default prefix matcher if the end user hasn't already registered one
        serviceCollection.TryAddTransient<ICommandPrefixMatcher, SimplePrefixMatcher>();

        if (useDefaultCommandResponder)
        {
            serviceCollection.AddCommandResponder();
        }

        serviceCollection.AddCondition<RequireContextCondition>();
        serviceCollection.AddCondition<RequireOwnerCondition>();
        serviceCollection.AddCondition<RequireDiscordPermissionCondition>();
        serviceCollection.AddCondition<RequireBotDiscordPermissionsCondition>();

        serviceCollection
            .AddParser<ChannelParser>()
            .AddParser<GuildMemberParser>()
            .AddParser<RoleParser>()
            .AddParser<UserParser>()
            .AddParser<SnowflakeParser>()
            .AddParser<EmojiParser>()
            .AddParser<OneOfParser>();

        serviceCollection.TryAddSingleton<ExecutionEventCollectorService>();

        serviceCollection.TryAddScoped<FeedbackService>();
        serviceCollection.AddSingleton(FeedbackTheme.DiscordLight);
        serviceCollection.AddSingleton(FeedbackTheme.DiscordDark);

        if (!enableSlash)
        {
            return serviceCollection;
        }

        if (useDefaultInteractionResponder)
        {
            serviceCollection.AddInteractionResponder();
        }

        serviceCollection.TryAddSingleton<SlashService>();
        serviceCollection.AddAutocompleteProvider(typeof(EnumAutocompleteProvider<>));
        serviceCollection.AddResponder<AutocompleteResponder>();
        serviceCollection.AddParser<AttachmentParser>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds the command responder to the system.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection, with the command responder.</returns>
    public static IServiceCollection AddCommandResponder
    (
        this IServiceCollection serviceCollection
    )
    {
        serviceCollection.AddResponder<CommandResponder>();
        return serviceCollection;
    }

    /// <summary>
    /// Adds the interaction responder to the system.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="optionsConfigurator">The option configurator.</param>
    /// <returns>The collection, with the interaction responder.</returns>
    public static IServiceCollection AddInteractionResponder
    (
        this IServiceCollection serviceCollection,
        Action<InteractionResponderOptions>? optionsConfigurator = null
    )
    {
        optionsConfigurator ??= _ => { };

        serviceCollection.AddResponder<InteractionResponder>();
        serviceCollection.Configure(optionsConfigurator);

        return serviceCollection;
    }

    /// <summary>
    /// Adds a pre-execution event to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <returns>The collection, with the event.</returns>
    public static IServiceCollection AddPreExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPreExecutionEvent
    {
        serviceCollection.AddScoped<IPreExecutionEvent, TEvent>();
        return serviceCollection;
    }

    /// <summary>
    /// Adds a post-execution event to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <returns>The collection, with the event.</returns>
    public static IServiceCollection AddPostExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPostExecutionEvent
    {
        serviceCollection.AddScoped<IPostExecutionEvent, TEvent>();
        return serviceCollection;
    }

    /// <summary>
    /// Adds a pre- and post-execution event to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <returns>The collection, with the event.</returns>
    public static IServiceCollection AddExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPreExecutionEvent, IPostExecutionEvent
    {
        serviceCollection
            .AddPreExecutionEvent<TEvent>()
            .AddPostExecutionEvent<TEvent>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds an autocomplete provider to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TProvider">The autocomplete provider.</typeparam>
    /// <returns>The collection, with the provider.</returns>
    public static IServiceCollection AddAutocompleteProvider<TProvider>(this IServiceCollection serviceCollection)
        where TProvider : class, IAutocompleteProvider
    {
        return serviceCollection.AddAutocompleteProvider(typeof(TProvider));
    }

    /// <summary>
    /// Adds an autocomplete provider to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="providerType">The autocomplete provider type.</param>
    /// <returns>The collection, with the provider.</returns>
    public static IServiceCollection AddAutocompleteProvider
    (
        this IServiceCollection serviceCollection,
        Type providerType
    )
    {
        if (!typeof(IAutocompleteProvider).IsAssignableFrom(providerType))
        {
            throw new InvalidOperationException
            (
                $"Autocomplete providers must implement {nameof(IAutocompleteProvider)}."
            );
        }

        IEnumerable<Type> autocompleteInterfaces;
        if (!providerType.IsGenericTypeDefinition)
        {
            serviceCollection.TryAddScoped(providerType);

            autocompleteInterfaces = providerType.GetInterfaces()
                .Where(i => typeof(IAutocompleteProvider).IsAssignableFrom(i));
        }
        else
        {
            autocompleteInterfaces = providerType.GetInterfaces()
                .Where(i => typeof(IAutocompleteProvider).IsAssignableFrom(i))
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericTypeDefinition())
                .Distinct();
        }

        foreach (var implementedInterface in autocompleteInterfaces)
        {
            serviceCollection.AddScoped(implementedInterface, providerType);
        }

        return serviceCollection;
    }
}
