//
//  IMessageSticker.cs
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

using System.Collections.Generic;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a sticker sent with a message.
    /// </summary>
    public interface IMessageSticker
    {
        /// <summary>
        /// Gets the ID of the sticker.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the ID of the sticker pack.
        /// </summary>
        Snowflake PackID { get; }

        /// <summary>
        /// Gets the name of the sticker.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the sticker.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a list of sticker tags.
        /// </summary>
        Optional<IReadOnlyList<string>> Tags { get; }

        /// <summary>
        /// Gets the sticker asset hash.
        /// </summary>
        IImageHash Asset { get; }

        /// <summary>
        /// Gets the sticker preview asset hash.
        /// </summary>
        IImageHash PreviewAsset { get; }

        /// <summary>
        /// Gets the format of the sticker.
        /// </summary>
        MessageStickerFormatType FormatType { get; }
    }
}
