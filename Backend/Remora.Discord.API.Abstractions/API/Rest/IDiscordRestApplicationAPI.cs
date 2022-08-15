//
//  IDiscordRestApplicationAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Application API.
/// </summary>
[PublicAPI]
public interface IDiscordRestApplicationAPI
{
    /// <summary>
    /// Gets the global commands for the application.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="withLocalizations">
    /// Indicates whether the full localization dictionaries should be returned, instead of just the requested locale.
    /// </param>
    /// <param name="locale">The locale to request the response in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IApplicationCommand>>> GetGlobalApplicationCommandsAsync
    (
        Snowflake applicationID,
        Optional<bool> withLocalizations = default,
        Optional<string> locale = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new global command.
    /// </summary>
    /// <remarks>
    /// Creating a new command with the same name as an existing command will overwrite the old command.
    /// </remarks>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="name">The name of the command. 3-32 characters.</param>
    /// <param name="description">The description of the command. 1-100 characters.</param>
    /// <param name="options">The parameters for the command.</param>
    /// <param name="type">The type of the application command.</param>
    /// <param name="nameLocalizations">The localized names of the command.</param>
    /// <param name="descriptionLocalizations">The localized descriptions of the command.</param>
    /// <param name="defaultMemberPermissions">The permissions required to execute the command.</param>
    /// <param name="dmPermission">Whether this command is executable in DMs.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IApplicationCommand>> CreateGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        string name,
        string description,
        Optional<IReadOnlyList<IApplicationCommandOption>> options = default,
        Optional<ApplicationCommandType> type = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        Optional<bool?> dmPermission = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Overwrites all global commands with the given command set. Any commands not in the set will be deleted.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="commands">The commands.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IApplicationCommand>>> BulkOverwriteGlobalApplicationCommandsAsync
    (
        Snowflake applicationID,
        IReadOnlyList<IBulkApplicationCommandData> commands,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a global command.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IApplicationCommand>> GetGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake commandID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits a new global command.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="name">The name of the command. 3-32 characters.</param>
    /// <param name="description">The description of the command. 1-100 characters.</param>
    /// <param name="options">The parameters for the command.</param>
    /// <param name="nameLocalizations">The localized names of the command.</param>
    /// <param name="descriptionLocalizations">The localized descriptions of the command.</param>
    /// <param name="defaultMemberPermissions">The permissions required to execute the command.</param>
    /// <param name="dmPermission">Whether this command is executable in DMs.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IApplicationCommand>> EditGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake commandID,
        Optional<string> name = default,
        Optional<string> description = default,
        Optional<IReadOnlyList<IApplicationCommandOption>?> options = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        Optional<bool?> dmPermission = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given global command.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake commandID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the guild commands for the application.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="withLocalizations">
    /// Indicates whether the full localization dictionaries should be returned, instead of just the requested locale.
    /// </param>
    /// <param name="locale">The locale to request the response in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IApplicationCommand>>> GetGuildApplicationCommandsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Optional<bool> withLocalizations = default,
        Optional<string> locale = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new guild command.
    /// </summary>
    /// <remarks>
    /// Creating a new command with the same name as an existing command will overwrite the old command.
    /// </remarks>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the command. 3-32 characters.</param>
    /// <param name="description">The description of the command. 1-100 characters.</param>
    /// <param name="options">The parameters for the command.</param>
    /// <param name="type">The type of the application command.</param>
    /// <param name="nameLocalizations">The localized names of the command.</param>
    /// <param name="descriptionLocalizations">The localized descriptions of the command.</param>
    /// <param name="defaultMemberPermissions">The permissions required to execute the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IApplicationCommand>> CreateGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        string name,
        string description,
        Optional<IReadOnlyList<IApplicationCommandOption>> options = default,
        Optional<ApplicationCommandType> type = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Overwrites all guild commands with the given command set. Any commands not in the set will be deleted.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="commands">The commands.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IApplicationCommand>>> BulkOverwriteGuildApplicationCommandsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        IReadOnlyList<IBulkApplicationCommandData> commands,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a guild command.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IApplicationCommand>> GetGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits a new guild command.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="name">The name of the command. 3-32 characters.</param>
    /// <param name="description">The description of the command. 1-100 characters.</param>
    /// <param name="options">The parameters for the command.</param>
    /// <param name="nameLocalizations">The localized names of the command.</param>
    /// <param name="descriptionLocalizations">The localized descriptions of the command.</param>
    /// <param name="defaultMemberPermissions">The permissions required to execute the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    /// <remarks>This method requires a bearer token authorized with the applications.commands.permissions.update scope.</remarks>
    Task<Result<IApplicationCommand>> EditGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        Optional<string> name = default,
        Optional<string> description = default,
        Optional<IReadOnlyList<IApplicationCommandOption>?> options = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given guild command.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the permissions for all of the application's commands in a guild.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IGuildApplicationCommandPermissions>>> GetGuildApplicationCommandPermissionsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the permissions for a specific command in a guild.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildApplicationCommandPermissions>> GetApplicationCommandPermissionsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits command permissions for a specific command in a guild.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="commandID">The ID of the command.</param>
    /// <param name="permissions">The permissions to overwrite the existing ones with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>An edit result which may or may not have succeeded.</returns>
    /// <remarks>This method requires a bearer token authorized with the applications.commands.permissions.update scope.</remarks>
    Task<Result<IGuildApplicationCommandPermissions>> EditApplicationCommandPermissionsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        IReadOnlyList<IApplicationCommandPermissions> permissions,
        CancellationToken ct = default
    );
}
