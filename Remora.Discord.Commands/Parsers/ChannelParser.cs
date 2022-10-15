//
//  ChannelParser.cs
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Parsers;
using Remora.Commands.Results;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IChannel"/> and <see cref="IPartialChannel"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class ChannelParser : AbstractTypeParser<IChannel>, ITypeParser<IPartialChannel>
{
    private readonly IDiscordRestChannelAPI _channelAPI;
    private readonly ContextInjectionService _contextInjection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelParser"/> class.
    /// </summary>
    /// <param name="channelAPI">The channel API.</param>
    /// <param name="contextInjection">The context injection service.</param>
    public ChannelParser(IDiscordRestChannelAPI channelAPI, ContextInjectionService contextInjection)
    {
        _channelAPI = channelAPI;
        _contextInjection = contextInjection;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IChannel>> TryParseAsync(string value, CancellationToken ct = default)
    {
        if (!DiscordSnowflake.TryParse(value.Unmention(), out var channelID))
        {
            return new ParsingError<IChannel>(value.Unmention());
        }

        return await _channelAPI.GetChannelAsync(channelID.Value, ct);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialChannel>> ITypeParser<IPartialChannel>.TryParseAsync(IReadOnlyList<string> tokens, CancellationToken ct)
    {
        return (await (this as ITypeParser<IChannel>).TryParseAsync(tokens, ct)).Map(a => a as IPartialChannel);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialChannel>> ITypeParser<IPartialChannel>.TryParseAsync(string token, CancellationToken ct)
    {
        _ = DiscordSnowflake.TryParse(token.Unmention(), out var channelID);
        if (channelID is null)
        {
            return new ParsingError<IPartialChannel>(token, "Unrecognized input format.");
        }

        var resolvedChannel = GetResolvedChannelOrDefault(channelID.Value);
        return resolvedChannel is null
            ? (await (this as ITypeParser<IChannel>).TryParseAsync(token, ct)).Map(a => a as IPartialChannel)
            : Result<IPartialChannel>.FromSuccess(resolvedChannel);
    }

    private IPartialChannel? GetResolvedChannelOrDefault(Snowflake channelID)
    {
        if (_contextInjection.Context is not InteractionContext interactionContext)
        {
            return null;
        }

        if (!interactionContext.Interaction.Data.IsDefined(out var data))
        {
            return null;
        }

        var resolvedData = data.Match(a => a.Resolved, _ => default, _ => default);
        if (!resolvedData.IsDefined(out var resolved) || !resolved.Channels.IsDefined(out var channels))
        {
            return null;
        }

        _ = channels.TryGetValue(channelID, out var channel);
        return channel;
    }
}
