//
//  MediatorEventResponder.cs
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
using MediatR;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Extensions.MediatR.Requests;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Extensions.MediatR.Responders
{
    /// <summary>
    /// Accepts any <see cref="IGatewayEvent"/> and sends it as a MediatR Request.
    /// </summary>
    public sealed class MediatorEventResponder : IResponder<IGatewayEvent>
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediatorEventResponder"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public MediatorEventResponder(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <inheritdoc />
        public Task<Result> RespondAsync(IGatewayEvent gatewayEvent, CancellationToken ct = default)
            => _mediator.Send(new GatewayEventRequest<IGatewayEvent>(gatewayEvent), ct);
    }
}
