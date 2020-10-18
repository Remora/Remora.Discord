//
//  EmbedFooter.cs
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
    public class EmbedFooter : IEmbedFooter
    {
        /// <inheritdoc />
        public string Text { get; }

        /// <inheritdoc />
        public Optional<string> IconUrl { get; }

        /// <inheritdoc />
        public Optional<string> ProxyIconUrl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedFooter"/> class.
        /// </summary>
        /// <param name="text">The footer text.</param>
        /// <param name="iconUrl">The footer icon.</param>
        /// <param name="proxyIconUrl">The proxied url of the icon.</param>
        public EmbedFooter(string text, Optional<string> iconUrl, Optional<string> proxyIconUrl)
        {
            this.Text = text;
            this.IconUrl = iconUrl;
            this.ProxyIconUrl = proxyIconUrl;
        }
    }
}
