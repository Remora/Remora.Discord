//
//  EmbedAuthor.cs
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

using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class EmbedAuthor : IEmbedAuthor
    {
        /// <inheritdoc />
        public Optional<string> Name { get; }

        /// <inheritdoc />
        public Optional<string> Url { get; }

        /// <inheritdoc />
        public Optional<string> IconUrl { get; }

        /// <inheritdoc />
        public Optional<string> ProxyIconUrl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedAuthor"/> class.
        /// </summary>
        /// <param name="name">The name of the author.</param>
        /// <param name="url">The author's URL.</param>
        /// <param name="iconUrl">The author's icon URL.</param>
        /// <param name="proxyIconUrl">The author's proxied icon URL.</param>
        public EmbedAuthor
        (
            Optional<string> name,
            Optional<string> url,
            Optional<string> iconUrl,
            Optional<string> proxyIconUrl
        )
        {
            this.Name = name;
            this.Url = url;
            this.IconUrl = iconUrl;
            this.ProxyIconUrl = proxyIconUrl;
        }
    }
}
