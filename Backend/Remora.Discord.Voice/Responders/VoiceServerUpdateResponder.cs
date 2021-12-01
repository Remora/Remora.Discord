//
//  VoiceServerUpdateResponder.cs
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
    /// Contains logic for responding to <see cref="IVoiceServerUpdate"/> gateway events.
    /// </summary>
    [PublicAPI]
    public class VoiceServerUpdateResponder : IResponder<IVoiceServerUpdate>
    {
        private readonly IConnectionEstablishmentWaiterService _connectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceServerUpdateResponder"/> class.
        /// </summary>
        /// <param name="connectionService">The connection waiter service.</param>
        public VoiceServerUpdateResponder(IConnectionEstablishmentWaiterService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Responds to a voice server update event.
        /// </summary>
        /// <param name="gatewayEvent">The voice server update.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>The result of the operation.</returns>
        public Task<Result> RespondAsync(IVoiceServerUpdate gatewayEvent, CancellationToken ct = default)
        {
            _connectionService.SubmitVoiceServerUpdate(gatewayEvent);
            return Task.FromResult(Result.FromSuccess());
        }
    }
}
