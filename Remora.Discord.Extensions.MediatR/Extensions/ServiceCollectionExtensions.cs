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
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Extensions.MediatR.Behaviors;
using Remora.Discord.Extensions.MediatR.Responders;
using Remora.Discord.Gateway.Extensions;

namespace Remora.Discord.Extensions.MediatR.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add scoped messaging.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Creates a new mediator and uses it to broadcast gateway events as requests.
        /// </summary>
        /// <typeparam name="TEntryPoint">The entry point of the application, typically Program.cs.</typeparam>
        /// <param name="services">This service collection instance.</param>
        /// <param name="mediatrConfiguration">An optional action used to configure MediatR.</param>
        /// <returns>The current <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddDiscordMessaging<TEntryPoint>(this IServiceCollection services, Action<MediatRServiceConfiguration>? mediatrConfiguration = null)
            where TEntryPoint : class
        {
            // Add MediatR itself.
            services.AddMediatR(mediatrConfiguration, typeof(TEntryPoint));

            // All gateway events return a Result. This handles uncaught exceptions by forcing the handler to return a failed Result with an ExceptionError.
            services.AddTransient(typeof(RequestExceptionActionProcessorBehavior<,>), typeof(GatewayEventExceptionHandlerBehavior<,>));

            // This pipeline behavior adds logging of the result. Starting/stopping of handling is logged as Trace events.
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultLoggingBehavior<,>));

            // Registers a generic Discord Event Responder which forwards events as MediatR events.
            services.AddResponder<MediatorEventResponder>();

            return services;
        }
    }
}
