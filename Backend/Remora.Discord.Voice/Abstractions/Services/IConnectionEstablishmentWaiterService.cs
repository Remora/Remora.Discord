//
//  IConnectionEstablishmentWaiterService.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Voice.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Voice.Abstractions.Services
{
    /// <summary>
    /// Defines an intermediary between requesting a voice connection and the gateway confirming it.
    /// </summary>
    [PublicAPI]
    public interface IConnectionEstablishmentWaiterService
    {
        /// <summary>
        /// Waits for confirmation of a voice connection request.
        /// </summary>
        /// <param name="guildID">The ID of the guild that a voice request has been made for.</param>
        /// <param name="timeoutMilliseconds">Defines the amount of time in milliseconds before the wait operation times out.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result indicating if the request was successfully confirmed.</returns>
        Task<Result<VoiceConnectionEstablishmentDetails>> WaitForRequestConfirmation
        (
            Snowflake guildID,
            int timeoutMilliseconds = 5000,
            CancellationToken ct = default
        );

        /// <summary>
        /// Submits a voice state update event that may or may not be in response to a voice connection request.
        /// </summary>
        /// <param name="voiceStateUpdate">The voice state udpate event.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Result> SubmitVoiceStateUpdateAsync
        (
            IVoiceStateUpdate voiceStateUpdate,
            CancellationToken ct = default
        );

        /// <summary>
        /// Submits a voice server update event that is in response to a voice connection request.
        /// </summary>
        /// <param name="voiceServerUpdate">The voice server update event.</param>
        void SubmitVoiceServerUpdate(IVoiceServerUpdate voiceServerUpdate);
    }
}
