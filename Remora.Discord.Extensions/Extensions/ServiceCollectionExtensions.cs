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
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Commands.Groups;
using Remora.Discord.Extensions.Attributes;
using Remora.Discord.Extensions.Services;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Interactivity;

namespace Remora.Discord.Extensions.Extensions;

/// <summary>
/// A collection of extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services provided by Remora.Discord.Extensions.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection, with the services added.</returns>
    public static IServiceCollection AddExtensions(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IPermissionComputationService, PermissionComputationService>();
        return serviceCollection;
    }

    /// <summary>
    /// Automatically registers every command group in the given assembly.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add commands to.</param>
    /// <param name="assembly">The assembly to discover command groups from.</param>
    /// <param name="treeName">The name of the tree to register commands to, otherwise the default.</param>
    /// <param name="typeFilter">A function to select whether a given command group should be registered.</param>
    /// <returns>The service collection with registered commands.</returns>
    public static IServiceCollection AddCommandGroupsFromAssembly
    (
        this IServiceCollection serviceCollection,
        Assembly assembly,
        string? treeName = null,
        Func<Type, bool>? typeFilter = null
    )
    {
        var candidates = assembly.ExportedTypes.Where
        (
            t => t is { IsClass: true, IsAbstract: false } &&
                 typeof(CommandGroup).IsAssignableFrom(t) &&
                 !typeof(InteractionGroup).IsAssignableFrom(t)
        )
        .ToArray();

        var tree = serviceCollection.AddCommandTree(treeName);

        foreach (var candidate in candidates)
        {
            if (typeFilter?.Invoke(candidate) ?? true)
            {
                tree.WithCommandGroup(candidate);
            }
        }

        return serviceCollection;
    }

    /// <summary>
    /// Adds all responders in the given assembly to the service collection, using their attributed group if possible.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register responders in.</param>
    /// <param name="assembly">The assembly to discover responders from.</param>
    /// <returns>The service collection to chain calls.</returns>
    public static IServiceCollection AddRespondersFromAssembly
    (
        this IServiceCollection serviceCollection,
        Assembly assembly
    )
    {
        var candidates = assembly.GetTypes().Where
        (
            t => t is { IsClass: true, IsAbstract: false } &&
                 t.GetInterfaces().Any
                 (
                     i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResponder<>)
                 )
        )
        .ToArray();

        foreach (var candidate in candidates)
        {
            var grouping = candidate.GetCustomAttribute(typeof(ResponderAttribute)) as ResponderAttribute;

            serviceCollection.AddResponder(candidate, grouping?.Group ?? ResponderGroup.Normal);
        }

        return serviceCollection;
    }
}
