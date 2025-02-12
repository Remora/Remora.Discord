//
//  IStageInstance.cs
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

using System;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about a live stage.
/// </summary>
[PublicAPI]
public interface IStageInstance
{
    /// <summary>
    /// Gets the ID of the stage instance.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the guild the instance is in.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the ID of the stage channel the instance is associated with.
    /// </summary>
    Snowflake ChannelID { get; }

    /// <summary>
    /// Gets the topic of the instance.
    /// </summary>
    string Topic { get; }

    /// <summary>
    /// Gets the privacy level of the instance.
    /// </summary>
    StagePrivacyLevel PrivacyLevel { get; }

    /// <summary>
    /// Gets a value indicating whether stage discovery is disabled for the instance.
    /// </summary>
    [Obsolete("Marked obsolete by Discord")]
    bool IsDiscoveryDisabled { get; }

    /// <summary>
    /// Gets the ID of the scheduled event for this stage instance.
    /// </summary>
    Snowflake? GuildScheduledEventID { get; }
}
