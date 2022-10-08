//
//  IDiscordRestStickerAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Sticker API.
/// </summary>
[PublicAPI]
public interface IDiscordRestStickerAPI
{
    /// <summary>
    /// Gets the sticker for the given ID.
    /// </summary>
    /// <param name="stickerID">The ID of the sticker.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<ISticker>> GetStickerAsync(Snowflake stickerID, CancellationToken ct = default);

    /// <summary>
    /// Gets a list of sticker packs available to Nitro subscribers.
    /// </summary>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<INitroStickerPacks>> ListNitroStickerPacksAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets a list of stickers belonging to the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<ISticker>>> ListGuildStickersAsync(Snowflake guildID, CancellationToken ct = default);

    /// <summary>
    /// Gets a specific sticker from a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="stickerID">The ID of the sticker.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<ISticker>> GetGuildStickerAsync
    (
        Snowflake guildID,
        Snowflake stickerID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new sticker in the given guild.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the new sticker.</param>
    /// <param name="description">The description of the new sticker.</param>
    /// <param name="tags">
    /// The tags related to the expression of the sticker. Typically, one would use a single unicode emoji.
    /// </param>
    /// <param name="file">The file contents.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<ISticker>> CreateGuildStickerAsync
    (
        Snowflake guildID,
        string name,
        string description,
        string tags,
        FileData file,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given sticker in the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="stickerID">The ID of the sticker.</param>
    /// <param name="name">The new name, if any.</param>
    /// <param name="description">The new description, if any.</param>
    /// <param name="tags">The new tags, if any.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<ISticker>> ModifyGuildStickerAsync
    (
        Snowflake guildID,
        Snowflake stickerID,
        Optional<string> name = default,
        Optional<string?> description = default,
        Optional<string> tags = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given sticker in the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="stickerID">The ID of the sticker.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildStickerAsync
    (
        Snowflake guildID,
        Snowflake stickerID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );
}
