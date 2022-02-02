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
        /// <typeparam name="TEntryPoint">The entry point of the application, typically program.cs.</typeparam>
        /// <param name="services">This service collection instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddDiscordMessaging<TEntryPoint>(this IServiceCollection services)
            where TEntryPoint : class
        {
            services.AddMediatR(typeof(TEntryPoint));

            services.AddTransient(typeof(RequestExceptionActionProcessorBehavior<,>), typeof(GatewayEventExceptionHandlerBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultLoggingBehavior<,>));

            services.AddResponder<ChannelEventHandler>();

            return services;
        }
    }
}
