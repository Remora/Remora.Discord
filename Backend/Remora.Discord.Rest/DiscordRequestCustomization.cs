//
//  DiscordRequestCustomization.cs
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
using Remora.Discord.Rest.API;

namespace Remora.Discord.Rest
{
    /// <summary>
    /// Represents a set of customizations that will be applied to a REST request made to Discord.
    /// </summary>
    [PublicAPI]
    public class DiscordRequestCustomization : IDisposable
    {
        private readonly DiscordHttpClient _parentClient;

        /// <summary>
        /// Gets the request customizer.
        /// </summary>
        internal Action<RestRequestBuilder> RequestCustomizer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRequestCustomization"/> class.
        /// </summary>
        /// <param name="parentClient">The client that created the customization.</param>
        /// <param name="requestCustomizer">The request customizer.</param>
        internal DiscordRequestCustomization
        (
            DiscordHttpClient parentClient,
            Action<RestRequestBuilder> requestCustomizer
        )
        {
            _parentClient = parentClient;
            this.RequestCustomizer = requestCustomizer;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _parentClient.RemoveCustomization(this);
        }
    }
}
