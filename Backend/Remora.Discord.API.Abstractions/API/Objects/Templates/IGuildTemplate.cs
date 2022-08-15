//
//  IGuildTemplate.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a Discord Guild.
/// </summary>
[PublicAPI]
public interface IGuildTemplate
{
    /// <summary>
    /// Gets the name of the guild.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the guild.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the guild's icon hash.
    /// </summary>
    IImageHash? IconHash { get; }

    /// <summary>
    /// Gets the verification level required for the guild.
    /// </summary>
    VerificationLevel VerificationLevel { get; }

    /// <summary>
    /// Gets the default notification level for the guild.
    /// </summary>
    MessageNotificationLevel DefaultMessageNotifications { get; }

    /// <summary>
    /// Gets the explicit content level.
    /// </summary>
    ExplicitContentFilterLevel ExplicitContentFilter { get; }

    /// <summary>
    /// Gets the preferred locale of a community-enabled guild.
    /// </summary>
    string PreferredLocale { get; }

    /// <summary>
    /// Gets the AFK timeout (in seconds).
    /// </summary>
    int AFKTimeout { get; }

    /// <summary>
    /// Gets a list of the role templates in the server.
    /// </summary>
    IReadOnlyList<IRoleTemplate> Roles { get; }

    /// <summary>
    /// Gets the channel templates in the guild.
    /// </summary>
    IReadOnlyList<IChannelTemplate> Channels { get; }

    /// <summary>
    /// Gets the ID of the AFK channel.
    /// </summary>
    int? AFKChannelID { get; }

    /// <summary>
    /// Gets the ID of the channel that system messages are sent to.
    /// </summary>
    int? SystemChannelID { get; }

    /// <summary>
    /// Gets the flags on the system channel.
    /// </summary>
    SystemChannelFlags SystemChannelFlags { get; }
}
