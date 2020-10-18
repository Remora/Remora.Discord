//
//  EmbedImage.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class EmbedImage : IEmbedImage
    {
        /// <inheritdoc />
        public Optional<string> Url { get; }

        /// <inheritdoc />
        public Optional<string> ProxyUrl { get; }

        /// <inheritdoc />
        public Optional<int> Height { get; }

        /// <inheritdoc />
        public Optional<int> Width { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedImage"/> class.
        /// </summary>
        /// <param name="url">The image URL.</param>
        /// <param name="proxyUrl">The proxied image URL.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="width">The width of the image.</param>
        public EmbedImage
        (
            Optional<string> url = default,
            Optional<string> proxyUrl = default,
            Optional<int> height = default,
            Optional<int> width = default
        )
        {
            this.Url = url;
            this.ProxyUrl = proxyUrl;
            this.Height = height;
            this.Width = width;
        }
    }
}
