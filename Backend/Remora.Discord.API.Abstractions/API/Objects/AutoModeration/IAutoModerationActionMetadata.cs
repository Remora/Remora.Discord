//
//  IAutoModerationActionMetadata.cs
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

using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects.AutoModeration;

/// <summary>
/// Additional data used when an action is executed. Different fields are relevant based on value of the
/// <see cref="AutoModerationActionType"/>.
/// </summary>
public interface IAutoModerationActionMetadata
{
    /// <summary>
    /// Gets the channel to which user content should be logged.
    /// </summary>
    /// <remarks>
    /// The associated action type for this property is <see cref="AutoModerationActionType.SendAlertMessage"/>.
    /// </remarks>
    Optional<Snowflake> ChannelID { get; }

    /// <summary>
    /// Gets the timeout duration in seconds.
    /// </summary>
    /// <remarks>
    /// <para>Maximum of 2419200 seconds (4 weeks).</para>
    /// <para>The associated action type for this property is <see cref="AutoModerationActionType.Timeout"/>.</para>
    /// </remarks>
    Optional<int> Duration { get; }
}
