//
//  VoiceStateUpdateResponder.cs
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
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Voice.Abstractions.Services;
using Remora.Results;

namespace Remora.Discord.Voice.Responders
{
    /// <summary>
    /// Contains logic for responding to <see cref="IVoiceStateUpdate"/> gateway events.
    /// </summary>
    [PublicAPI]
    public class VoiceStateUpdateResponder : IResponder<IVoiceStateUpdate>
    {
        private readonly IConnectionEstablishmentWaiterService _connectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceStateUpdateResponder"/> class.
        /// </summary>
        /// <param name="connectionService">The connection waiter service.</param>
        public VoiceStateUpdateResponder(IConnectionEstablishmentWaiterService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Responds to a voice state update event.
        /// </summary>
        /// <param name="gatewayEvent">The voice state update.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<Result> RespondAsync(IVoiceStateUpdate gatewayEvent, CancellationToken ct = default)
        {
            return await _connectionService.SubmitVoiceStateUpdateAsync(gatewayEvent, ct).ConfigureAwait(false);
        }
    }
}
