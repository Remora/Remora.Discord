//
//  ServiceCollectionExtensions.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;
using Remora.Extensions.Options.Immutable;

namespace Remora.Discord.Interactivity.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required for interactivity.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection, with the added services.</returns>
    public static IServiceCollection AddInteractivity(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddResponder<InteractivityResponder>();

        serviceCollection.Configure(() => new InteractivityResponderOptions());

        return serviceCollection;
    }

    /// <summary>
    /// Adds an interactive command group to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TInteractionGroup">The entity type.</typeparam>
    /// <returns>The collection, with the entity added.</returns>
    public static IServiceCollection AddInteractionGroup
    <
        [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] TInteractionGroup
    >
    (
        this IServiceCollection serviceCollection
    )
        where TInteractionGroup : InteractionGroup
            => serviceCollection.AddInteractiveEntity(typeof(TInteractionGroup));

    /// <summary>
    /// Adds an interactive entity to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The collection, with the entity added.</returns>
    public static IServiceCollection AddInteractiveEntity
    (
        this IServiceCollection serviceCollection,
        Type entityType
    )
    {
        serviceCollection.AddCommandTree(Constants.InteractionTree)
            .WithCommandGroup(entityType);

        return serviceCollection;
    }
}
