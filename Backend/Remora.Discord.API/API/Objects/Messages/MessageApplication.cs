//
//  MessageApplication.cs
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
    public class MessageApplication : IMessageApplication
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public Optional<IImageHash> CoverImage { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public IImageHash? Icon { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageApplication"/> class.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <param name="coverImage">The cover image of the application.</param>
        /// <param name="description">The description of the application.</param>
        /// <param name="icon">The application's icon.</param>
        /// <param name="name">The name of the application.</param>
        public MessageApplication
        (
            Snowflake id,
            Optional<IImageHash> coverImage,
            string description,
            IImageHash? icon,
            string name
        )
        {
            this.ID = id;
            this.CoverImage = coverImage;
            this.Description = description;
            this.Icon = icon;
            this.Name = name;
        }
    }
}
