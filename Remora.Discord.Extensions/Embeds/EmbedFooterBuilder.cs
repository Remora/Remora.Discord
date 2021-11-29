//
//  EmbedFooterBuilder.cs
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
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Embeds
{
    /// <summary>
    /// A builder which validates and builds an <see cref="EmbedFooter"/>.
    /// </summary>
    public sealed class EmbedFooterBuilder : BuilderBase<EmbedFooter>
    {
        /// <summary>
        /// Gets or sets the text of the footer.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the icon url of the footer.
        /// </summary>
        public Optional<string> IconUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedFooterBuilder"/> class.
        /// </summary>
        /// <param name="text">The text of the footer.</param>
        /// <param name="iconUrl">The icon url of the footer.</param>
        public EmbedFooterBuilder(string text, Optional<string> iconUrl = default)
        {
            Text = text;
            IconUrl = iconUrl;
        }

        /// <inheritdoc />
        public override Result<EmbedFooter> Build()
        {
            var validationResult = this.Validate();

            return validationResult.IsSuccess
                ? new EmbedFooter(Text, IconUrl)
                : Result<EmbedFooter>.FromError(validationResult);
        }

        /// <summary>
        /// Converts an existing embed footer into a footer builder.
        /// </summary>
        /// <param name="footer">The footer.</param>
        /// <returns>A new <see cref="EmbedFooterBuilder"/> based on the provided footer.</returns>
        public static EmbedFooterBuilder FromFooter(IEmbedFooter footer)
            => new(footer.Text, footer.IconUrl);
    }
}
