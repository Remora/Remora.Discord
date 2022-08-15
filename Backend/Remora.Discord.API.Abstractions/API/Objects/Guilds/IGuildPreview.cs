//
//  IGuildPreview.cs
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
/// Represents a preview of a public guild.
/// </summary>
[PublicAPI]
public interface IGuildPreview
{
    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the guild.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the guild's icon.
    /// </summary>
    IImageHash? Icon { get; }

    /// <summary>
    /// Gets the guild's splash banner.
    /// </summary>
    IImageHash? Splash { get; }

    /// <summary>
    /// Gets the guild's Discovery splash banner.
    /// </summary>
    IImageHash? DiscoverySplash { get; }

    /// <summary>
    /// Gets a list of emojis in the server.
    /// </summary>
    IReadOnlyList<IEmoji> Emojis { get; }

    /// <summary>
    /// Gets a list of guild features.
    /// </summary>
    IReadOnlyList<GuildFeature> Features { get; }

    /// <summary>
    /// Gets the approximate count of online members.
    /// </summary>
    Optional<int> ApproximatePresenceCount { get; }

    /// <summary>
    /// Gets the approximate count of total members.
    /// </summary>
    Optional<int> ApproximateMemberCount { get; }

    /// <summary>
    /// Gets the description of the guild, if the guild is discoverable.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the custom stickers the guild has.
    /// </summary>
    IReadOnlyList<ISticker> Stickers { get; }
}
