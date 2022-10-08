//
//  RequireOwnerCondition.cs
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
using JetBrains.Annotations;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Commands.Conditions;

/// <summary>
/// Checks that the bot's owner is the same as the invoking user.
/// </summary>
[PublicAPI]
public class RequireOwnerCondition : ICondition<RequireOwnerAttribute>
{
    private readonly ICommandContext _context;
    private readonly IDiscordRestOAuth2API _oauth2API;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireOwnerCondition"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="oauth2API">The OAuth2 API.</param>
    public RequireOwnerCondition(ICommandContext context, IDiscordRestOAuth2API oauth2API)
    {
        _context = context;
        _oauth2API = oauth2API;
    }

    /// <inheritdoc />
    public async ValueTask<Result> CheckAsync(RequireOwnerAttribute attribute, CancellationToken ct = default)
    {
        var getApplication = await _oauth2API.GetCurrentBotApplicationInformationAsync(ct);
        if (!getApplication.IsSuccess)
        {
            return (Result)getApplication;
        }

        var application = getApplication.Entity;

        if (application.Owner is null || !application.Owner.ID.HasValue)
        {
            return new InvalidOperationError("The application owner's ID was not present.");
        }

        return application.Owner.ID.Value == _context.User.ID
            ? Result.FromSuccess()
            : new InvalidOperationError("You need to be the bot owner to do that.");
    }
}
