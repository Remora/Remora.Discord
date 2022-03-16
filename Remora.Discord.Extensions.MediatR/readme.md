# Remora.Discord.Extensions.MediatR

This package provides utilities and components which translate Discord gateway events into MediatR notifications and requests.

This extensions only registers its responder with the Normal group. Early and Late responder groups are unsupported at this time.

# Usage

1. Register MediatR with Remora.Discord:
```cs
public void ConfigureServices(IServiceCollection services)
{
    // Add MediatR
    services.AddDiscordMessaging<Program>();
}
```
2. Create a handler. An interface has been created to make this easier.
```cs
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Extensions.MediatR.Requests;
using Remora.Results;

/// <summary>
/// Handles <see cref="IMessageCreate"/> events sent with MediatR.
/// </summary>
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
```
