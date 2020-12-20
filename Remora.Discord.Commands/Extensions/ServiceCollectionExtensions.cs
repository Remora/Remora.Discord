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
using Remora.Discord.Commands.Responders;
using Remora.Discord.Gateway.Extensions;

namespace Remora.Discord.Commands.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the command responder to the system.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="optionsConfigurator">The option configurator.</param>
        /// <returns>The collection, with the command responder.</returns>
        public static IServiceCollection AddCommandResponder
        (
            this IServiceCollection serviceCollection,
            Action<CommandResponderOptions>? optionsConfigurator = null
        )
        {
            optionsConfigurator ??= options => options.Prefix = "!";

            serviceCollection.AddResponder<CommandResponder>();
            serviceCollection.Configure(optionsConfigurator);

            return serviceCollection;
        }

        /// <summary>
        /// Adds the interaction responder to the system.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The collection, with the interaction responder.</returns>
        public static IServiceCollection AddInteractionResponder
        (
            this IServiceCollection serviceCollection
        )
        {
            serviceCollection.AddResponder<InteractionResponder>();
            return serviceCollection;
        }
    }
}
