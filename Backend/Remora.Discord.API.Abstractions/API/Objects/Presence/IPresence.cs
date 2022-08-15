//
//  IPresence.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a user's presence.
/// </summary>
[PublicAPI]
public interface IPresence : IPartialPresence
{
    /// <summary>
    /// Gets the user the presence is being updated for.
    /// </summary>
    new IPartialUser User { get; }

    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    new Snowflake GuildID { get; }

    /// <summary>
    /// Gets the current status of the user.
    /// </summary>
    new ClientStatus Status { get; }

    /// <summary>
    /// Gets the user's current activities.
    /// </summary>
    new IReadOnlyList<IActivity>? Activities { get; }

    /// <summary>
    /// Gets the user's platform-dependent status.
    /// </summary>
    new IClientStatuses ClientStatus { get; }

    /// <inheritdoc/>
    Optional<IPartialUser> IPartialPresence.User => new(this.User);

    /// <inheritdoc/>
    Optional<Snowflake> IPartialPresence.GuildID => this.GuildID;

    /// <inheritdoc/>
    Optional<ClientStatus> IPartialPresence.Status => this.Status;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IActivity>?> IPartialPresence.Activities => new(this.Activities);

    /// <inheritdoc/>
    Optional<IClientStatuses> IPartialPresence.ClientStatus => new(this.ClientStatus);
}
