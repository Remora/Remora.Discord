//
//  TextCommandContext.cs
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
using Remora.Commands.Services;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Commands.Contexts;

/// <summary>
/// Represents contextual information about a currently executing text-based command.
/// </summary>
/// <param name="Message">The message that initiated the command.</param>
/// <param name="GuildID">The ID of the guild the message is in, if any.</param>
/// <param name="Command">The command associated with the context.</param>
[PublicAPI]
public record TextCommandContext(IPartialMessage Message, Optional<Snowflake> GuildID, PreparedCommand Command) :
    MessageContext(Message, GuildID),
    ITextCommandContext;
