//
//  HttpCatCommands.cs
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

using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace Remora.Discord.Samples.Localization.Commands;

/// <summary>
/// Responds to a HttpCat command.
/// </summary>
public class HttpCatCommands : CommandGroup
{
    private readonly FeedbackService _feedbackService;
    private readonly ICommandContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpCatCommands"/> class.
    /// </summary>
    /// <param name="feedbackService">The feedback service.</param>
    /// <param name="context">The command context.</param>
    public HttpCatCommands(FeedbackService feedbackService, ICommandContext context)
    {
        _feedbackService = feedbackService;
        _context = context;
    }

    /// <summary>
    /// Posts a cat image that matches the user.
    /// </summary>
    /// <returns>The result of the command.</returns>
    [Command("Cattify")]
    [CommandType(ApplicationCommandType.User)]
    public async Task<IResult> PostContextualUserHttpCatAsync()
    {
        if (_context is not InteractionContext interactionContext)
        {
            return Result.FromSuccess();
        }

        if (!interactionContext.Data.TryPickT0(out var commandData, out _))
        {
            return Result.FromSuccess();
        }

        if (!commandData.Resolved.IsDefined(out var resolved))
        {
            return Result.FromSuccess();
        }

        if (!resolved.Users.IsDefined(out var users))
        {
            return Result.FromSuccess();
        }

        var user = users.First().Value;
        return await PostUserHttpCatAsync(user);
    }

    /// <summary>
    /// Posts a HTTP error code cat.
    /// This command will generate ephemeral responses.
    /// </summary>
    /// <param name="httpCode">The HTTP error code.</param>
    /// <returns>The result of the command.</returns>
    [Command("cat")]
    [Description("Posts a cat image that represents the given error code.")]
    public async Task<IResult> PostHttpCatAsync([Description("The HTTP code.")] int httpCode)
    {
        var embedImage = new EmbedImage($"https://http.cat/{httpCode}");
        var embed = new Embed(Colour: _feedbackService.Theme.Secondary, Image: embedImage);

        return (Result)await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
    }

    /// <summary>
    /// Posts a HTTP error code cat.
    /// This command will generate ephemeral responses.
    /// </summary>
    /// <param name="httpCode">The HTTP error code.</param>
    /// <returns>The result of the command.</returns>
    [Command("ephemeral-cat")]
    [Description("Posts a cat image that represents the given error code.")]
    [Ephemeral]
    public async Task<IResult> PostEphemeralHttpCatAsync([Description("The HTTP code.")] int httpCode)
    {
        var embedImage = new EmbedImage($"https://http.cat/{httpCode}");
        var embed = new Embed(Colour: _feedbackService.Theme.Secondary, Image: embedImage);

        return (Result)await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
    }

    /// <summary>
    /// Posts a HTTP error code cat.
    /// </summary>
    /// <param name="catUser">The user to cattify.</param>
    /// <returns>The result of the command.</returns>
    [Command("user-cat")]
    [Description("Posts a cat image that matches the user.")]
    public Task<IResult> PostUserHttpCatAsync([Description("The user to cattify")] IPartialUser catUser)
    {
        if (!catUser.ID.IsDefined(out var id))
        {
            return Task.FromResult<IResult>(Result.FromSuccess());
        }

        var values = Enum.GetValues<HttpStatusCode>();
        var index = Map(id.Value, 0, ulong.MaxValue, 0, (ulong)(values.Length - 1));

        var code = values[index];
        return PostHttpCatAsync((int)code);
    }

    /// <summary>
    /// Posts a HTTP error code cat that mathematically 'matches' a channel.
    /// The clientside autocompletion of channel selections is restricted to guild text channels
    /// by the <see cref="ChannelTypesAttribute"/> on the <paramref name="channel"/> parameter.
    /// </summary>
    /// <param name="channel">The channel to cattify.</param>
    /// <returns>The result of the command.</returns>
    [Command("channel-cat")]
    [Description("Posts a cat image that matches the provided channel.")]
    public Task<IResult> PostChannelHttpCatAsync
    (
        [Description("The channel to cattify")][ChannelTypes(ChannelType.GuildText)] IChannel channel
    )
    {
        var values = Enum.GetValues<HttpStatusCode>();
        var index = Map(channel.ID.Value, 0, ulong.MaxValue, 0, (ulong)(values.Length - 1));

        var code = values[index];
        return PostHttpCatAsync((int)code);
    }

    private static ulong Map(ulong value, ulong fromSource, ulong toSource, ulong fromTarget, ulong toTarget)
    {
        return ((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget)) + fromTarget;
    }
}
