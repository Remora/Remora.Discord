//
//  SPDX-FileName: EchoResponder.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
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
        if ((gatewayEvent.Author.IsBot.TryGet(out var isBot) && isBot) ||
            (gatewayEvent.Author.IsSystem.TryGet(out var isSystem) && isSystem))
        {
            return Result.FromSuccess();
        }

        return (Result)await _channelAPI.CreateMessageAsync
        (
            gatewayEvent.ChannelID,
            ct: ct,
            messageReference: new MessageReference(MessageReferenceType.Forward, MessageID: gatewayEvent.ID, ChannelID: gatewayEvent.ChannelID, GuildID: gatewayEvent.GuildID)
        );
    }
}
