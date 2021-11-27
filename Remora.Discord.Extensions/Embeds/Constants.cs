//
//  Constants.cs
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

using System.Drawing;

namespace Remora.Discord.Extensions.Embeds
{
    /// <summary>
    /// Provides a set of constant values for embed validation.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Returns the maximum number of fields allowed by Discord.
        /// </summary>
        public const int MaxFieldCount = 25;

        /// <summary>
        /// Returns the maximum title length allowed by Discord.
        /// </summary>
        public const int MaxTitleLength = 256;

        /// <summary>
        /// Returns the maximum description length allowed by Discord.
        /// </summary>
        public const int MaxDescriptionLength = 2048;

        /// <summary>
        /// Returns the maximum total embed length allowed by Discord.
        /// </summary>
        public const int MaxEmbedLength = 6000;

        /// <summary>
        /// Returns the maximum author name length allowed by Discord.
        /// </summary>
        public const int MaxAuthorNameLength = 256;

        /// <summary>
        /// Returns the maximum footer length allowed by Discord.
        /// </summary>
        public const int MaxFooterTextLength = 2048;

        /// <summary>
        /// The default embed color.
        /// </summary>
        public static readonly Color DefaultColour = Color.FromArgb(95, 186, 125);
    }
}
