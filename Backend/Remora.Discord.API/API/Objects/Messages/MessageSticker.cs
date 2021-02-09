//
//  MessageSticker.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class MessageSticker : IMessageSticker
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public Snowflake PackID { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<string>> Tags { get; }

        /// <inheritdoc />
        public IImageHash Asset { get; }

        /// <inheritdoc />
        public IImageHash PreviewAsset { get; }

        /// <inheritdoc />
        public MessageStickerFormatType FormatType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSticker"/> class.
        /// </summary>
        /// <param name="id">The ID of the sticker.</param>
        /// <param name="packID">The ID of the sticker pack.</param>
        /// <param name="name">The name of the sticker.</param>
        /// <param name="description">The description of the sticker.</param>
        /// <param name="tags">The tags associated with the sticker.</param>
        /// <param name="asset">The asset hash of the sticker.</param>
        /// <param name="previewAsset">The preview asset hash of the sticker.</param>
        /// <param name="formatType">The format type of the sticker.</param>
        public MessageSticker
        (
            Snowflake id,
            Snowflake packID,
            string name,
            string description,
            Optional<IReadOnlyList<string>> tags,
            IImageHash asset,
            IImageHash previewAsset,
            MessageStickerFormatType formatType
        )
        {
            this.ID = id;
            this.PackID = packID;
            this.Name = name;
            this.Description = description;
            this.Tags = tags;
            this.Asset = asset;
            this.PreviewAsset = previewAsset;
            this.FormatType = formatType;
        }
    }
}
