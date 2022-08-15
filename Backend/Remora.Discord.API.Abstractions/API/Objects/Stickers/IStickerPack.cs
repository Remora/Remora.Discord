//
//  IStickerPack.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a pack of stickers.
/// </summary>
[PublicAPI]
public interface IStickerPack
{
    /// <summary>
    /// Gets the ID of the sticker pack.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the stickers in the pack.
    /// </summary>
    IReadOnlyList<ISticker> Stickers { get; }

    /// <summary>
    /// Gets the name of the sticker pack.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the sticker pack's stock keeping unit ID.
    /// </summary>
    Snowflake SKUID { get; }

    /// <summary>
    /// Gets the ID of the sticker in the pack which is shown as the pack's icon.
    /// </summary>
    Optional<Snowflake> CoverStickerID { get; }

    /// <summary>
    /// Gets the description of the pack.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the ID of the sticker pack's banner image.
    /// </summary>
    Optional<Snowflake> BannerAssetID { get; }
}
