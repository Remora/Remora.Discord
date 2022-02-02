//
//  IGatewayEventRequest.cs
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

using MediatR;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Results;

namespace Remora.Discord.Extensions.MediatR.Requests
{
    /// <summary>
    /// A request which represents an <typeparamref name="TGatewayEvent"/> gateway event.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The type of event which raised this request.</typeparam>
    public interface IGatewayEventRequest<TGatewayEvent> : IRequest<Result>
        where TGatewayEvent : IGatewayEvent
    {
    }
}
