//
//  IDiscordRestEmojiAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Emoji API.
/// </summary>
[PublicAPI]
public interface IDiscordRestEmojiAPI
{
    /// <summary>
    /// Gets a list of emojis for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IEmoji>>> ListGuildEmojisAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the emoji on the given guild with the given ID.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="emojiID">The ID of the emoji.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IEmoji>> GetGuildEmojiAsync
    (
        Snowflake guildID,
        Snowflake emojiID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new emoji for the given guild with the given parameters.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the new emoji.</param>
    /// <param name="image">The image data.</param>
    /// <param name="roles">The roles that the emoji will be restricted to.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IEmoji>> CreateGuildEmojiAsync
    (
        Snowflake guildID,
        string name,
        Stream image,
        IReadOnlyList<Snowflake> roles,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given emoji.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="emojiID">The ID of the emoji.</param>
    /// <param name="name">The new name of the emoji.</param>
    /// <param name="roles">The new restricted roles.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IEmoji>> ModifyGuildEmojiAsync
    (
        Snowflake guildID,
        Snowflake emojiID,
        Optional<string> name = default,
        Optional<IReadOnlyList<Snowflake>?> roles = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given emoji.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="emojiID">The ID of the emoji.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildEmojiAsync
    (
        Snowflake guildID,
        Snowflake emojiID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );
}
