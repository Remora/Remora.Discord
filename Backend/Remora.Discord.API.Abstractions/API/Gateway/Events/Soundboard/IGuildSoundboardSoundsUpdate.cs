//
//  IGuildSoundboardSoundsUpdate.cs
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
using Remora.Discord.API.Abstractions.Objects.Soundboard;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents a batch of update events for soundboard sounds.
/// </summary>
[PublicAPI]
public interface IGuildSoundboardSoundsUpdate : IGatewayEvent
{
    /// <summary>
    /// Gets the updated soundboard sounds.
    /// </summary>
    IReadOnlyList<ISoundboardSound> SoundboardSounds { get; }

    /// <summary>
    /// Gets the ID of the guild that the sounds were updated in.
    /// </summary>
    Snowflake GuildID { get; }
}
