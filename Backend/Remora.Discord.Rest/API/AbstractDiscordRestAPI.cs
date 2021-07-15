//
//  AbstractDiscordRestAPI.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Rest.API
{
    /// <summary>
    /// Acts as an abstract base for REST API instances.
    /// </summary>
    [PublicAPI]
    public abstract class AbstractDiscordRestAPI
    {
        /// <summary>
        /// Gets the <see cref="Remora.Discord.Rest.DiscordHttpClient"/> available to the API instance.
        /// </summary>
        protected DiscordHttpClient DiscordHttpClient { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDiscordRestAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord-specialized Http client.</param>
        protected AbstractDiscordRestAPI(DiscordHttpClient discordHttpClient)
        {
            this.DiscordHttpClient = discordHttpClient;
        }

        /// <inheritdoc cref="Remora.Discord.Rest.DiscordHttpClient.WithCustomization"/>
        public DiscordRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
        {
            return this.DiscordHttpClient.WithCustomization(requestCustomizer);
        }
    }
}
