//
//  ISoundboardSound.cs
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

namespace Remora.Discord.API.Abstractions.Objects.Soundboard;

/// <summary>
/// Represents a sound in a guild's soundboard.
/// </summary>
[PublicAPI]
public interface ISoundboardSound
{
    /// <summary>
    /// Gets the name of the sound.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the ID of the sound.
    /// </summary>
    Snowflake SoundID { get; }

    /// <summary>
    /// Gets the default volume of this sound (0 to 1).
    /// </summary>
    double Volume { get; }

    /// <summary>
    /// Gets the ID of the sound's custom emoji.
    /// </summary>
    Snowflake? EmojiID { get; }

    /// <summary>
    /// Gets the unicode string of the sound's standard emoji.
    /// </summary>
    string? EmojiName { get; }

    /// <summary>
    /// Gets the ID of the guild this sound is in.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets a value indicating whether the sound can currently be used.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Gets the user who created the sound.
    /// </summary>
    Optional<IUser> User { get; }
}
