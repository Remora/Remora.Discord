//
//  IDiscordRestStageInstanceAPI.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Stage Instance API.
/// </summary>
[PublicAPI]
public interface IDiscordRestStageInstanceAPI
{
    /// <summary>
    /// Creates a new stage instance associated with the given stage channel.
    /// </summary>
    /// <param name="channelID">The ID of the stage channel.</param>
    /// <param name="topic">The topic of the stage instance (1-120 characters).</param>
    /// <param name="privacyLevel">The privacy level of the stage instance.</param>
    /// <param name="sendStartNotification">
    /// Indicates whether @everyone should be notified that a stage instance has started.
    /// </param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IStageInstance>> CreateStageInstanceAsync
    (
        Snowflake channelID,
        string topic,
        Optional<StagePrivacyLevel> privacyLevel = default,
        Optional<bool> sendStartNotification = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the stage instance associated with the given stage channel.
    /// </summary>
    /// <param name="channelID">The ID of the stage channel.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IStageInstance>> GetStageInstanceAsync(Snowflake channelID, CancellationToken ct = default);

    /// <summary>
    /// Modifies the stage instance of the given stage channel.
    /// </summary>
    /// <param name="channelID">The ID of the stage channel.</param>
    /// <param name="topic">The topic of the stage instance (1-120 characters).</param>
    /// <param name="privacyLevel">The privacy level of the stage instance.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IStageInstance>> ModifyStageInstanceAsync
    (
        Snowflake channelID,
        Optional<string> topic = default,
        Optional<StagePrivacyLevel> privacyLevel = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the stage instance associated with the given stage channel.
    /// </summary>
    /// <param name="channelID">The ID of the stage channel.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> DeleteStageInstance
    (
        Snowflake channelID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );
}
