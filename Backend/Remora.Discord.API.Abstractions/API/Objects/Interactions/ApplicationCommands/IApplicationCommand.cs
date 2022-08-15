//
//  IApplicationCommand.cs
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
/// Represents an application command.
/// </summary>
[PublicAPI]
public interface IApplicationCommand
{
    /// <summary>
    /// Gets the ID of the command.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the type of the command.
    /// </summary>
    Optional<ApplicationCommandType> Type { get; }

    /// <summary>
    /// Gets the ID of the application.
    /// </summary>
    Snowflake ApplicationID { get; }

    /// <summary>
    /// Gets the ID of the guild the command belongs to.
    /// </summary>
    Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    /// <remarks>The length of the name must be between 3 and 32 characters.</remarks>
    string Name { get; }

    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    /// <remarks>The length of the description must be between 1 and 100 characters.</remarks>
    string Description { get; }

    /// <summary>
    /// Gets the parameters of the command.
    /// </summary>
    Optional<IReadOnlyList<IApplicationCommandOption>> Options { get; }

    /// <summary>
    /// Gets a value that increments on substantial changes.
    /// </summary>
    Snowflake Version { get; }

    /// <summary>
    /// Gets the localized names of the command.
    /// </summary>
    Optional<IReadOnlyDictionary<string, string>?> NameLocalizations { get; }

    /// <summary>
    /// Gets the localized name of the command.
    /// </summary>
    /// <remarks>
    /// This field is only supplied by Discord as a response, and is not used to set the actual localized string.
    /// </remarks>
    Optional<string> NameLocalized { get; }

    /// <summary>
    /// Gets the localized descriptions of the command.
    /// </summary>
    Optional<IReadOnlyDictionary<string, string>?> DescriptionLocalizations { get; }

    /// <summary>
    /// Gets the localized description of the command.
    /// </summary>
    /// <remarks>
    /// This field is only supplied by Discord as a response, and is not used to set the actual localized string.
    /// </remarks>
    Optional<string> DescriptionLocalized { get; }

    /// <summary>
    /// Gets a value that indicates the requisite permissions to execute the command.
    /// </summary>
    IDiscordPermissionSet? DefaultMemberPermissions { get; }

    /// <summary>
    /// Gets a value that indicates whether this command can be executed in DMs.
    /// </summary>
    Optional<bool> DMPermission { get; }
}
