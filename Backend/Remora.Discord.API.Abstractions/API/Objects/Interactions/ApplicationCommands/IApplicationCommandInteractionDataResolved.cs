//
//  IApplicationCommandInteractionDataResolved.cs
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
/// Represents data that was resolved during serverside execution of the command.
/// </summary>
[PublicAPI]
public interface IApplicationCommandInteractionDataResolved
{
    /// <summary>
    /// Gets the resolved users, if any.
    /// </summary>
    Optional<IReadOnlyDictionary<Snowflake, IUser>> Users { get; }

    /// <summary>
    /// Gets the resolved guild members, if any.
    /// </summary>
    Optional<IReadOnlyDictionary<Snowflake, IPartialGuildMember>> Members { get; }

    /// <summary>
    /// Gets the resolved roles, if any.
    /// </summary>
    Optional<IReadOnlyDictionary<Snowflake, IRole>> Roles { get; }

    /// <summary>
    /// Gets the resolved channels, if any.
    /// </summary>
    Optional<IReadOnlyDictionary<Snowflake, IPartialChannel>> Channels { get; }

    /// <summary>
    /// Gets the resolved messages, if any.
    /// </summary>
    Optional<IReadOnlyDictionary<Snowflake, IPartialMessage>> Messages { get; }

    /// <summary>
    /// Gets the resolved attachments, if any.
    /// </summary>
    Optional<IReadOnlyDictionary<Snowflake, IAttachment>> Attachments { get; }
}
