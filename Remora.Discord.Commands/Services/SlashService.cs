//
//  SlashService.cs
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
using OneOf;
using Remora.Commands.Results;
using Remora.Commands.Services;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Handles updating and verifying of slash commands.
/// </summary>
[PublicAPI]
public class SlashService
{
    private readonly CommandTreeAccessor _commandTreeAccessor;
    private readonly IDiscordRestOAuth2API _oauth2API;
    private readonly IDiscordRestApplicationAPI _applicationAPI;
    private readonly ILocalizationProvider _localizationProvider;

    /// <summary>
    /// Gets a mapping of Discord's assigned snowflakes to their corresponding command nodes.
    /// </summary>
    /// <remarks>
    /// The snowflake maps to the top-level entity, which may be a group or a command. If the top-level entity is
    /// a group, then any subcommands in that group will be provided in a sub-dictionary, with keys in the form
    /// <value>subgroup-name::command-name</value>, nested as required.
    /// </remarks>
    public IReadOnlyDictionary
    <
        (Optional<Snowflake> GuildID, Snowflake CommandID),
        OneOf<IReadOnlyDictionary<string, CommandNode>, CommandNode>
    > CommandMap { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SlashService"/> class.
    /// </summary>
    /// <param name="commandTreeAccessor">The command tree accessor.</param>
    /// <param name="oauth2API">The OAuth2 API.</param>
    /// <param name="applicationAPI">The application API.</param>
    /// <param name="localizationProvider">The localization provider.</param>
    public SlashService
    (
        CommandTreeAccessor commandTreeAccessor,
        IDiscordRestOAuth2API oauth2API,
        IDiscordRestApplicationAPI applicationAPI,
        ILocalizationProvider localizationProvider
    )
    {
        _commandTreeAccessor = commandTreeAccessor;
        _applicationAPI = applicationAPI;
        _oauth2API = oauth2API;
        _localizationProvider = localizationProvider;

        this.CommandMap = new Dictionary
        <
            (Optional<Snowflake> GuildID, Snowflake CommandID),
            OneOf<IReadOnlyDictionary<string, CommandNode>, CommandNode>
        >();
    }

    /// <summary>
    /// Determines whether the application's commands support being bound to Discord slash commands.
    /// </summary>
    /// <param name="treeName">The name of the tree to check.</param>
    /// <returns>true if slash commands are supported; otherwise, false.</returns>
    public Result SupportsSlashCommands(string? treeName = null)
    {
        if (!_commandTreeAccessor.TryGetNamedTree(treeName, out var tree))
        {
            return new TreeNotFoundError(treeName);
        }

        // TODO: Improve
        // Yes, this is inefficient. Generally, this method is only expected to be called a limited number of times on
        // startup.
        return (Result)tree.CreateApplicationCommands(_localizationProvider);
    }

    /// <summary>
    /// Updates the application's slash commands.
    /// </summary>
    /// <param name="guildID">The ID of the guild to update slash commands in, if any.</param>
    /// <param name="treeName">
    /// The name of the tree to update Discord with. Note that whatever is currently configured (either globally or on
    /// the provided guild) will be completely replaced by this tree.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    public async Task<Result> UpdateSlashCommandsAsync
    (
        Snowflake? guildID = null,
        string? treeName = null,
        CancellationToken ct = default
    )
    {
        if (!_commandTreeAccessor.TryGetNamedTree(treeName, out var tree))
        {
            return new TreeNotFoundError(treeName);
        }

        var getApplication = await _oauth2API.GetCurrentBotApplicationInformationAsync(ct);
        if (!getApplication.IsSuccess)
        {
            return (Result)getApplication;
        }

        var application = getApplication.Entity;
        var createCommands = tree.CreateApplicationCommands(_localizationProvider);
        if (!createCommands.IsSuccess)
        {
            return (Result)createCommands;
        }

        // Upsert the current valid command set
        var updateResult = await
        (
            guildID is null
                ? _applicationAPI.BulkOverwriteGlobalApplicationCommandsAsync
                (
                    application.ID,
                    createCommands.Entity,
                    ct
                )
                : _applicationAPI.BulkOverwriteGuildApplicationCommandsAsync
                (
                    application.ID,
                    guildID.Value,
                    createCommands.Entity,
                    ct
                )
        );

        if (!updateResult.IsSuccess)
        {
            return (Result)updateResult;
        }

        // Update our command mapping
        var discordTree = updateResult.Entity;
        var mergedDictionary = new Dictionary
        <
            (Optional<Snowflake> GuildID, Snowflake CommandID),
            OneOf<IReadOnlyDictionary<string, CommandNode>, CommandNode>
        >(this.CommandMap);

        var newMappings = tree.MapDiscordCommands(discordTree);
        foreach (var (key, value) in newMappings)
        {
            mergedDictionary[key] = value;
        }

        this.CommandMap = mergedDictionary;

        return Result.FromSuccess();
    }
}
