//
//  IDiscordRestSoundboardAPI.cs
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
using Remora.Discord.API.Abstractions.Objects.Soundboard;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord soundboard API.
/// </summary>
[PublicAPI]
public interface IDiscordRestSoundboardAPI
{
    /// <summary>
    /// Sends a soundboard sound to a voice channel the user is connected to.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="soundID">The ID of the sound to play.</param>
    /// <param name="sourceGuildID">The ID of the guild the soundboard sound is in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result representing the completion of the action.</returns>
    Task<Result> SendSoundboardSoundAsync
    (
        Snowflake channelID,
        Snowflake soundID,
        Optional<Snowflake> sourceGuildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a list of the default soundboard sounds available on all servers.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The default soundboard sounds.</returns>
    Task<Result<IReadOnlyList<ISoundboardSound>>> ListDefaultSoundboardSoundsAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets a list of soundboard sounds in the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The guild's soundboard sounds.</returns>
    Task<Result<IListGuildSoundboardSoundsResponse>> ListGuildSoundboardSoundsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the soundboard sound in the given guild with the given ID.
    /// </summary>
    /// <param name="guildID">The guild ID.</param>
    /// <param name="soundID">The sound ID.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The soundboard sound.</returns>
    Task<Result<ISoundboardSound>> GetGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        Snowflake soundID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new soundboard sound for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the sound.</param>
    /// <param name="sound">The MP3- or OGG-encoded sound data.</param>
    /// <param name="volume">The volume to play the sound at.</param>
    /// <param name="emojiID">The ID of the custom emoji to use for the sound.</param>
    /// <param name="emojiName">The unicode string of the standard emoji to use for the sound.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The new soundboard sound.</returns>
    Task<Result<ISoundboardSound>> CreateGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        string name,
        byte[] sound,
        Optional<double?> volume = default,
        Optional<Snowflake?> emojiID = default,
        Optional<string?> emojiName = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies an existing soundboard sound for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="soundID">The ID of the sound.</param>
    /// <param name="name">The name of the sound.</param>
    /// <param name="volume">The volume to play the sound at.</param>
    /// <param name="emojiID">The ID of the custom emoji to use for the sound.</param>
    /// <param name="emojiName">The unicode string of the standard emoji to use for the sound.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The new soundboard sound.</returns>
    Task<Result<ISoundboardSound>> ModifyGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        Snowflake soundID,
        string name,
        Optional<double?> volume = default,
        Optional<Snowflake?> emojiID = default,
        Optional<string?> emojiName = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given soundboard sound.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="soundID">The ID of the sound.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A value representing result of the operation.</returns>
    Task<Result> DeleteGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        Snowflake soundID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );
}
