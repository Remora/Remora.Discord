//
//  MessageCreateHandler.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Extensions.MediatR.Requests;
using Remora.Results;

namespace Remora.Discord.Extensions.MediatR
{
    /// <summary>
    /// Handles <see cref="IMessageCreate"/> events sent with MediatR.
    /// </summary>
    [Obsolete("Included only for documentation purposes. Will be removed during final cleanups.")]
    public class MessageCreateHandler : IResultRequestHandler<IMessageCreate>
    {
        // NOTE: There is no message context forwarded, so the FeedbackService will not work.
        // private readonly FeedbackService _feedback;
        private readonly IDiscordRestChannelAPI _channelAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCreateHandler"/> class.
        /// </summary>
        /// <param name="channelAPI">The channel API used for this instance.</param>
        public MessageCreateHandler(IDiscordRestChannelAPI channelAPI)
        {
            _channelAPI = channelAPI;
        }

        /// <inheritdoc/>
        public Task<Result> Handle(IGatewayEventRequest<IMessageCreate> request, CancellationToken cancellationToken)
            => _channelAPI.CreateReactionAsync(request.Event.ChannelID, request.Event.ID, "👍", cancellationToken);
    }
}
