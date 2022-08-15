//
//  EchoResponder.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Samples.LoadRespondersFromAssembly.Responders;

/// <summary>
/// A Responder that sends all messages received back to where they were received.
/// </summary>
public class EchoResponder : IResponder<IMessageCreate>
{
    private readonly IDiscordRestChannelAPI _channelAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="EchoResponder"/> class.
    /// </summary>
    /// <param name="channelAPI">The <see cref="IDiscordRestChannelAPI"/>.</param>
    public EchoResponder(
        IDiscordRestChannelAPI channelAPI)
    {
        _channelAPI = channelAPI;
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
    {
        if ((gatewayEvent.Author.IsBot.IsDefined(out var isBot) && isBot) ||
            (gatewayEvent.Author.IsSystem.IsDefined(out var isSystem) && isSystem))
        {
            return Result.FromSuccess();
        }

        return (Result)await _channelAPI.CreateMessageAsync
        (
            gatewayEvent.ChannelID,
            gatewayEvent.Content,
            ct: ct
        );
    }
}
