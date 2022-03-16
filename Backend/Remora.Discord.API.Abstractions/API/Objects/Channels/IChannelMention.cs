//
//  IChannelMention.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
/// Represents a mentioned channel.
/// </summary>
[PublicAPI]
public interface IChannelMention
{
    /// <summary>
    /// Gets the ID of the channel.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the guild the channel is in.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the channel type.
    /// </summary>
    ChannelType Type { get; }

    /// <summary>
    /// Gets the name of the channel.
    /// </summary>
    string Name { get; }
}
