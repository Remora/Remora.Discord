//
//  IBulkApplicationCommandData.cs
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
using Remora.Discord.Core;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a request to create or update an application command.
    /// </summary>
    [PublicAPI]
    public interface IBulkApplicationCommandData
    {
        /// <inheritdoc cref="IApplicationCommand.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IApplicationCommand.Description"/>
        Optional<string> Description { get; }

        /// <inheritdoc cref="IApplicationCommand.Options"/>
        Optional<IReadOnlyList<IApplicationCommandOption>> Options { get; }

        /// <inheritdoc cref="IApplicationCommand.DefaultPermission"/>
        Optional<bool> DefaultPermission { get; }

        /// <inheritdoc cref="IApplicationCommand.Type"/>
        Optional<ApplicationCommandType> Type { get; }
    }
}
