//
//  SPDX-FileName: EchoResponder.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
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
    public EchoResponder(IDiscordRestChannelAPI channelAPI)
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
