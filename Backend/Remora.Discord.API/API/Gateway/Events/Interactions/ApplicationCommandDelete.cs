//
//  ApplicationCommandDelete.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Gateway.Events.IApplicationCommandDelete" />
    [PublicAPI]
    public record ApplicationCommandDelete
    (
        Snowflake ID,
        ApplicationCommandType Type,
        Snowflake ApplicationID,
        Optional<Snowflake> GuildID,
        string Name,
        string Description,
        Optional<IReadOnlyList<IApplicationCommandOption>> Options = default,
        Optional<bool> DefaultPermission = default
    ) :
        ApplicationCommand(ID, Type, ApplicationID, GuildID, Name, Description, Options, DefaultPermission),
        IApplicationCommandDelete;
}
