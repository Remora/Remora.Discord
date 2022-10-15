//
//  UserParser.cs
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
/// Parses instances of <see cref="IUser"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class UserParser : AbstractTypeParser<IUser>, ITypeParser<IPartialUser>
{
    private readonly IDiscordRestUserAPI _userAPI;
    private readonly ContextInjectionService _contextInjection;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserParser"/> class.
    /// </summary>
    /// <param name="userAPI">The user API.</param>
    /// <param name="contextInjection">The context injection service.</param>
    public UserParser(IDiscordRestUserAPI userAPI, ContextInjectionService contextInjection)
    {
        _userAPI = userAPI;
        _contextInjection = contextInjection;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IUser>> TryParseAsync(string value, CancellationToken ct = default)
    {
        if (!DiscordSnowflake.TryParse(value.Unmention(), out var userID))
        {
            return new ParsingError<IUser>(value.Unmention());
        }

        var resolvedUser = GetResolvedUserOrDefault(userID.Value);
        return resolvedUser is not null
            ? Result<IUser>.FromSuccess(resolvedUser)
            : await _userAPI.GetUserAsync(userID.Value, ct);
    }

    private IUser? GetResolvedUserOrDefault(Snowflake userID)
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
        if (!resolvedData.IsDefined(out var resolved))
        {
            return null;
        }

        if (resolved.Users.IsDefined(out var users) && users.TryGetValue(userID, out var user))
        {
            return user;
        }

        if (resolved.Members.IsDefined(out var members) && members.TryGetValue(userID, out var member))
        {
            _ = member.User.IsDefined(out user);
        }
        else
        {
            user = default;
        }

        return user;
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialUser>> ITypeParser<IPartialUser>.TryParseAsync
    (
        IReadOnlyList<string> tokens,
        CancellationToken ct
    )
    {
        return (await (this as ITypeParser<IUser>).TryParseAsync(tokens, ct)).Map(a => a as IPartialUser);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialUser>> ITypeParser<IPartialUser>.TryParseAsync(string token, CancellationToken ct)
    {
        return (await (this as ITypeParser<IUser>).TryParseAsync(token, ct)).Map(a => a as IPartialUser);
    }
}
