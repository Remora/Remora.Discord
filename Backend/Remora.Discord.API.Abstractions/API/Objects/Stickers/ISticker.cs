//
//  ISticker.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a sticker.
/// </summary>
[PublicAPI]
public interface ISticker
{
    /// <summary>
    /// Gets the ID of the sticker.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the sticker pack.
    /// </summary>
    Optional<Snowflake> PackID { get; }

    /// <summary>
    /// Gets the name of the sticker.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the sticker.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the autocomplete/suggestion tags for the sticker. By convention, this tends to be a comma-separated
    /// list.
    /// </summary>
    string Tags { get; }

    /// <summary>
    /// Gets the type of the sticker.
    /// </summary>
    StickerType Type { get; }

    /// <summary>
    /// Gets the format of the sticker.
    /// </summary>
    StickerFormatType FormatType { get; }

    /// <summary>
    /// Gets a value indicating whether the sticker is available.
    /// </summary>
    Optional<bool> IsAvailable { get; }

    /// <summary>
    /// Gets the ID of the guild the sticker belongs to.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the user that uploaded the sticker.
    /// </summary>
    Optional<IUser> User { get; }

    /// <summary>
    /// Gets the sticker's sorting order within a sticker pack.
    /// </summary>
    Optional<int> SortValue { get; }
}
