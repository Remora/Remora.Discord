//
//  ActivityAssets.cs
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

using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Objects
{
    /// <summary>
    /// Represents a set of descriptive assets related to an activity.
    /// </summary>
    public sealed class ActivityAssets
    {
        /// <summary>
        /// Gets the ID for a large image related to the activity. Usually, this is a snowflake.
        /// </summary>
        public Optional<string> LargeImage { get; }

        /// <summary>
        /// Gets the text displayed when hovering over the large image.
        /// </summary>
        public Optional<string> LargeText { get; }

        /// <summary>
        /// Gets the ID for a small image related to the activity. Usually, this is a snowflake.
        /// </summary>
        public Optional<string> SmallImage { get; }

        /// <summary>
        /// Gets the text displayed when hovering over the small image.
        /// </summary>
        public Optional<string> SmallText { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAssets"/> class.
        /// </summary>
        /// <param name="largeImage">The ID of the large image.</param>
        /// <param name="largeText">The hover text of the large image.</param>
        /// <param name="smallImage">The ID of the small image.</param>
        /// <param name="smallText">The hover text of the small image.</param>
        public ActivityAssets
        (
            Optional<string> largeImage = default,
            Optional<string> largeText = default,
            Optional<string> smallImage = default,
            Optional<string> smallText = default
        )
        {
            this.LargeImage = largeImage;
            this.LargeText = largeText;
            this.SmallImage = smallImage;
            this.SmallText = smallText;
        }
    }
}
