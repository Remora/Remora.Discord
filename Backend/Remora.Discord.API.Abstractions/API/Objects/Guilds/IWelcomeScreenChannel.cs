//
//  IWelcomeScreenChannel.cs
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
/// Represents a channel in a welcome screen.
/// </summary>
[PublicAPI]
public interface IWelcomeScreenChannel
{
    /// <summary>
    /// Gets the ID of the channel.
    /// </summary>
    Snowflake ChannelID { get; }

    /// <summary>
    /// Gets the description shown for the channel.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the ID of the guild emoji used for the channel.
    /// </summary>
    Snowflake? EmojiID { get; }

    /// <summary>
    /// Gets the name or unicode string of the emoji used for the channel.
    /// </summary>
    string? EmojiName { get; }
}
