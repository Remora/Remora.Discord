//
//  IInviteCreate.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents the creation of an invite.
/// </summary>
[PublicAPI]
public interface IInviteCreate : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the channel the invite is for.
    /// </summary>
    Snowflake ChannelID { get; }

    /// <summary>
    /// Gets the unique code of the invite.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Gets the time the invite was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the ID of the guild the invite is for.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the user that created the invite.
    /// </summary>
    Optional<IUser> Inviter { get; }

    /// <summary>
    /// Gets the time (in seconds) the invite is valid for.
    /// </summary>
    TimeSpan MaxAge { get; }

    /// <summary>
    /// Gets the maximum number of times the invite can be used.
    /// </summary>
    int MaxUses { get; }

    /// <summary>
    /// Gets the type of user target for this invite.
    /// </summary>
    Optional<InviteTarget> TargetType { get; }

    /// <summary>
    /// Gets the target user for this invite.
    /// </summary>
    Optional<IPartialUser> TargetUser { get; }

    /// <summary>
    /// Gets the embedded application this invite is for.
    /// </summary>
    Optional<IPartialApplication> TargetApplication { get; }

    /// <summary>
    /// Gets a value indicating whether the invite is temporary (invited users will be kicked on disconnect unless
    /// they're assigned to a role).
    /// </summary>
    bool IsTemporary { get; }

    /// <summary>
    /// Gets how many times the invite has been used. Always zero.
    /// </summary>
    int Uses { get; }
}
