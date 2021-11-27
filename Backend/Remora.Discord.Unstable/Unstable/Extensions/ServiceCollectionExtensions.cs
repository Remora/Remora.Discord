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

using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Json;
using Remora.Discord.API.VoiceGateway.Events;
using Remora.Rest.Extensions;

namespace Remora.Discord.Unstable.Extensions
{
    /// <summary>
    /// Defines various extension methods to the <see cref="IServiceCollection"/> class.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds experimental features from the Discord API.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="optionsName">The name of the serializer options, if any. You should probably leave this set to the default value.</param>
        /// <returns>The service collection, with the services.</returns>
        public static IServiceCollection AddExperimentalDiscordApi
        (
            this IServiceCollection serviceCollection,
            string? optionsName = "Discord"
        )
        {
            serviceCollection.Configure<JsonSerializerOptions>(optionsName, jsonOptions =>
            {
                jsonOptions.Converters.Add(new UnstableVoicePayloadConverter());

                var existingConverter = jsonOptions.Converters.FirstOrDefault(c => c.GetType() == typeof(VoicePayloadConverter));
                if (existingConverter is not null)
                {
                    jsonOptions.Converters.Remove(existingConverter);
                }

                jsonOptions.AddDataObjectConverter<IGuildScheduledEventUserAdd, GuildScheduledEventUserAdd>();
                jsonOptions.AddDataObjectConverter<IGuildScheduledEventUserRemove, GuildScheduledEventUserRemove>();

                jsonOptions.AddDataObjectConverter<IVoiceClientDisconnect, VoiceClientDisconnect>();
                jsonOptions.AddDataObjectConverter<IVoiceSpeakingEvent, VoiceSpeakingEvent>();
            });

            return serviceCollection;
        }
    }
}
