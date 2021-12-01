//
//  DiscordVoiceClientFactory.cs
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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Rest.Core;

namespace Remora.Discord.Voice
{
    /// <summary>
    /// Represents a factory for obtaining instances of a <see cref="DiscordVoiceClient"/>.
    /// </summary>
    [PublicAPI]
    public sealed class DiscordVoiceClientFactory : IAsyncDisposable
    {
        private readonly IServiceProvider _services;
        private readonly Dictionary<Snowflake, DiscordVoiceClient> _guildClients;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordVoiceClientFactory"/> class.
        /// </summary>
        /// <param name="services">The service provider.</param>
        public DiscordVoiceClientFactory(IServiceProvider services)
        {
            _services = services;
            _guildClients = new Dictionary<Snowflake, DiscordVoiceClient>();
        }

        /// <summary>
        /// Gets a voice client for the given guild.
        /// </summary>
        /// <param name="guildID">The ID of the guild to retrieve a voice client for.</param>
        /// <returns>A voice client instance.</returns>
        public DiscordVoiceClient Get(Snowflake guildID)
        {
            if (_guildClients.ContainsKey(guildID))
            {
                return _guildClients[guildID];
            }

            var voiceClient = _services.GetRequiredService<DiscordVoiceClient>();
            _guildClients.Add(guildID, voiceClient);

            return voiceClient;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            foreach (DiscordVoiceClient client in _guildClients.Values)
            {
                await client.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
