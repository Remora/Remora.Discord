﻿//
//  IBulkApplicationCommandData.cs
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
/// Represents a request to create or update an application command.
/// </summary>
[PublicAPI]
public interface IBulkApplicationCommandData
{
    /// <inheritdoc cref="IApplicationCommand.Name"/>
    string Name { get; }

    /// <inheritdoc cref="IApplicationCommand.Description"/>
    string Description { get; }

    /// <inheritdoc cref="IApplicationCommand.ID"/>
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IApplicationCommand.Options"/>
    Optional<IReadOnlyList<IApplicationCommandOption>> Options { get; }

    /// <inheritdoc cref="IApplicationCommand.Type"/>
    Optional<ApplicationCommandType> Type { get; }

    /// <inheritdoc cref="IApplicationCommand.NameLocalizations"/>
    Optional<IReadOnlyDictionary<string, string>?> NameLocalizations { get; }

    /// <inheritdoc cref="IApplicationCommand.DescriptionLocalizations"/>
    Optional<IReadOnlyDictionary<string, string>?> DescriptionLocalizations { get; }

    /// <inheritdoc cref="IApplicationCommand.DefaultMemberPermissions"/>
    IDiscordPermissionSet? DefaultMemberPermissions { get; }

    /// <inheritdoc cref="IApplicationCommand.DMPermission"/>
    Optional<bool> DMPermission { get; }

    /// <inheritdoc cref="IApplicationCommand.IsNsfw"/>
    Optional<bool> IsNsfw { get; }

    /// <inheritdoc cref="IApplicationCommand.IntegrationTypes"/>
    Optional<IReadOnlyList<ApplicationIntegrationType>> IntegrationTypes { get; }

    /// <inheritdoc cref="IApplicationCommand.Contexts"/>
    Optional<IReadOnlyList<InteractionContextType>> Contexts { get; }
}
