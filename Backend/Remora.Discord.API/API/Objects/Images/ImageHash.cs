//
//  ImageHash.cs
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

using Remora.Discord.API.Abstractions.Images;

namespace Remora.Discord.API.API.Objects.Images
{
    /// <inheritdoc />
    public class ImageHash : IImageHash
    {
        /// <inheritdoc />
        public string Value { get; }

        /// <inheritdoc />
        public bool HasGif => this.Value.StartsWith("a_");

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageHash"/> class.
        /// </summary>
        /// <param name="value">The hash.</param>
        public ImageHash(string value)
        {
            this.Value = value;
        }
    }
}
