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

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Extensions;
using Remora.Discord.API.Objects;
using Remora.Discord.Rest.API;

// ReSharper disable once CheckNamespace
namespace Remora.Discord.Unstable.Extensions
{
    /// <summary>
    /// Defines various extension methods to the <see cref="IServiceCollection"/> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds experimental features from the Discord API.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection, with the services.</returns>
        public static IServiceCollection AddExperimentalDiscordApi
        (
            this IServiceCollection serviceCollection
        )
        {
            serviceCollection
                .Configure<JsonSerializerOptions>
                (
                    options =>
                    {
                        options
                            .AddTemplateObjectConverters();
                    }
                );

            serviceCollection
                .AddScoped<IDiscordRestTemplateAPI, DiscordRestTemplateAPI>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds the JSON converters that handle template objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddTemplateObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<ITemplate, Template>();
            options.AddDataObjectConverter<IGuildTemplate, GuildTemplate>();
            options.AddDataObjectConverter<IRoleTemplate, RoleTemplate>()
                .WithPropertyName(r => r.Colour, "color")
                .WithPropertyName(r => r.IsHoisted, "hoist")
                .WithPropertyName(r => r.IsMentionable, "mentionable");

            options.AddDataObjectConverter<IChannelTemplate, ChannelTemplate>()
                .WithPropertyName(c => c.IsNsfw, "nsfw");

            options.AddDataObjectConverter<IPermissionOverwriteTemplate, PermissionOverwriteTemplate>();

            return options;
        }
    }
}
