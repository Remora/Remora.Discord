//
//  ChannelEventHandler.cs
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

using System;
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
    /// A class which bundles all channel event responders and broadcasts them as mediator requests.
    /// </summary>
    [Obsolete("Replaced with MediatorEventHandler")]
    public partial class ChannelEventHandler :
        IResponder<IChannelCreate>,
        IResponder<IChannelDelete>,
        IResponder<IChannelUpdate>,
        IResponder<IChannelPinsUpdate>,
        IResponder<IStageInstanceCreate>,
        IResponder<IStageInstanceDelete>,
        IResponder<IStageInstanceUpdate>,
        IResponder<IThreadCreate>,
        IResponder<IThreadDelete>,
        IResponder<IThreadUpdate>,
        IResponder<IThreadListSync>,
        IResponder<IThreadMemberUpdate>,
        IResponder<IThreadMembersUpdate>
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelEventHandler"/> class.
        /// </summary>
        /// <param name="mediator">An <see cref="IMediator"/> instance.</param>
        public ChannelEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IChannelCreate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ChannelCreateRequest<IChannelCreate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IChannelDelete gatewayEvent, CancellationToken ct = default)
        {
            var request = new ChannelDeleteRequest<IChannelDelete>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IChannelUpdate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ChannelUpdateRequest<IChannelUpdate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IChannelPinsUpdate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ChannelPinsUpdateRequest<IChannelPinsUpdate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IStageInstanceCreate gatewayEvent, CancellationToken ct = default)
        {
            var request = new StageInstanceCreateRequest<IStageInstanceCreate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IStageInstanceDelete gatewayEvent, CancellationToken ct = default)
        {
            var request = new StageInstanceDeleteRequest<IStageInstanceDelete>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IStageInstanceUpdate gatewayEvent, CancellationToken ct = default)
        {
            var request = new StageInstanceUpdateRequest<IStageInstanceUpdate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IThreadCreate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ThreadCreateRequest<IThreadCreate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IThreadDelete gatewayEvent, CancellationToken ct = default)
        {
            var request = new ThreadDeleteRequest<IThreadDelete>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IThreadUpdate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ThreadUpdateRequest<IThreadUpdate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IThreadListSync gatewayEvent, CancellationToken ct = default)
        {
            var request = new ThreadListSyncRequest<IThreadListSync>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IThreadMemberUpdate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ThreadMemberUpdateRequest<IThreadMemberUpdate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IThreadMembersUpdate gatewayEvent, CancellationToken ct = default)
        {
            var request = new ThreadMembersUpdateRequest<IThreadMembersUpdate>(gatewayEvent);
            return await _mediator.Send(request, ct);
        }
    }
}
