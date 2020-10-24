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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Commands.Services;
using Remora.Commands.Trees;

namespace Remora.Commands.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures command modules, allowing a user to register modules.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="builder">The registration action.</param>
        /// <returns>The service collection, with the configured modules.</returns>
        public static IServiceCollection ConfigureCommandModules
        (
            this IServiceCollection serviceCollection,
            Action<CommandTreeBuilder> builder
        )
        {
            serviceCollection.Configure(builder);
            return serviceCollection;
        }

        /// <summary>
        /// Adds the services needed by the command subsystem.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection, with the required command services.</returns>
        public static IServiceCollection AddCommands
        (
            this IServiceCollection serviceCollection
        )
        {
            serviceCollection.TryAddSingleton
            (
                services =>
                {
                    var treeBuilder = services.GetRequiredService<IOptions<CommandTreeBuilder>>();
                    return treeBuilder.Value.Build();
                }
            );

            serviceCollection.TryAddScoped
            (
                services =>
                {
                    var tree = services.GetRequiredService<CommandTree>();
                    return new CommandService(tree, services);
                }
            );

            return serviceCollection;
        }
    }
}
