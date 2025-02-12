//
//  IMessageContext.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Commands.Contexts;

/// <summary>
/// Represents contextual information about an ongoing operation on a message.
/// </summary>
[PublicAPI]
public interface IMessageContext : IOperationContext
{
    /// <summary>
    /// Gets the message.
    /// </summary>
    IPartialMessage Message { get; }

    /// <summary>
    /// Gets the ID of the guild the message is in, if any.
    /// </summary>
    Optional<Snowflake> GuildID { get; }
}
