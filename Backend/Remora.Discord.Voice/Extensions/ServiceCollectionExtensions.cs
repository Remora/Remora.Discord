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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Voice.Abstractions.Services;
using Remora.Discord.Voice.Responders;
using Remora.Discord.Voice.Services;

namespace Remora.Discord.Voice.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> class.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required by the Discord Gateway system.
        /// </summary>
        /// <remarks>
        /// This method expects that the gateway services have been registered - see
        /// <see cref="Gateway.Extensions.ServiceCollectionExtensions.AddDiscordGateway(IServiceCollection, System.Func{System.IServiceProvider, string})"/>.
        /// </remarks>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection, with the services added.</returns>
        public static IServiceCollection AddDiscordVoice(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<RecyclableMemoryStreamManager>();
            serviceCollection.TryAddSingleton<IConnectionEstablishmentWaiterService, ConnectionEstablishmentWaiterService>();
            serviceCollection.TryAddTransient<IVoicePayloadTransportService>(s => new WebSocketVoicePayloadTransportService
            (
                s,
                s.GetRequiredService<RecyclableMemoryStreamManager>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord")
            ));

            serviceCollection.TryAddTransient<DiscordVoiceClient>();
            serviceCollection.TryAddSingleton<DiscordVoiceClientFactory>();

            serviceCollection.TryAddTransient<IAudioTranscoderService, Pcm16AudioTranscoderService>();
            serviceCollection.TryAddTransient<IVoiceDataTranportService, UdpVoiceDataTransportService>();

            serviceCollection.AddResponder<VoiceStateUpdateResponder>();
            serviceCollection.AddResponder<VoiceServerUpdateResponder>();

            serviceCollection.Configure<DiscordGatewayClientOptions>
            (
                o => o.Intents |= GatewayIntents.GuildVoiceStates
            );

            return serviceCollection;
        }
    }
}
