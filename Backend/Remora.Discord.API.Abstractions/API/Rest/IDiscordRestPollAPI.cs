//
//  IDiscordRestPollAPI.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Poll API.
/// </summary>
[PublicAPI]
public interface IDiscordRestPollAPI
{
    /// <summary>
    /// Gets a list of users that voted for this specific answer.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="answerID">The ID of the answer.</param>
    /// <param name="after">The ID of the user to get users after.</param>
    /// <param name="limit">The maximum number of users to retrieve. Ranges between 1 and 100.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IPollAnswerVoters>> GetAnswerVotersAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        int answerID,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Immediately ends the poll.
    /// </summary>
    /// <remarks>
    /// You cannot end polls from other users.
    /// Fires a <see cref="IMessageUpdate"/> Gateway event.
    /// </remarks>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>An ending result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> EndPollAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    );
}
