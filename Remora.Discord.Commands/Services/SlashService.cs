//
//  SlashService.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OneOf;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Core;
using Remora.Results;
using static Remora.Discord.API.Abstractions.Objects.ApplicationCommandOptionType;

namespace Remora.Discord.Commands.Services
{
    /// <summary>
    /// Handles updating and verifying of slash commands.
    /// </summary>
    [PublicAPI]
    public class SlashService
    {
        private readonly CommandTree _commandTree;
        private readonly IDiscordRestOAuth2API _oauth2API;
        private readonly IDiscordRestApplicationAPI _applicationAPI;

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
            Snowflake,
            OneOf<IReadOnlyDictionary<string, CommandNode>, CommandNode>
        > CommandMap { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlashService"/> class.
        /// </summary>
        /// <param name="commandTree">The command tree.</param>
        /// <param name="oauth2API">The OAuth2 API.</param>
        /// <param name="applicationAPI">The application API.</param>
        public SlashService
        (
            CommandTree commandTree,
            IDiscordRestOAuth2API oauth2API,
            IDiscordRestApplicationAPI applicationAPI
        )
        {
            _commandTree = commandTree;
            _applicationAPI = applicationAPI;
            _oauth2API = oauth2API;

            this.CommandMap = new Dictionary<Snowflake, OneOf<IReadOnlyDictionary<string, CommandNode>, CommandNode>>();
        }

        /// <summary>
        /// Determines whether the application's commands support being bound to Discord slash commands.
        /// </summary>
        /// <returns>true if slash commands are supported; otherwise, false.</returns>
        public Result SupportsSlashCommands()
        {
            // TODO: Improve
            // Yes, this is inefficient. Generally, this method is only expected to be called once on startup.
            var couldCreate = _commandTree.CreateApplicationCommands();

            return couldCreate.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(couldCreate);
        }

        /// <summary>
        /// Updates the application's slash commands.
        /// </summary>
        /// <param name="guildID">The ID of the guild to update slash commands in, if any.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public async Task<Result> UpdateSlashCommandsAsync
        (
            Snowflake? guildID = null,
            CancellationToken ct = default
        )
        {
            var getApplication = await _oauth2API.GetCurrentBotApplicationInformationAsync(ct);
            if (!getApplication.IsSuccess)
            {
                return Result.FromError(getApplication);
            }

            var application = getApplication.Entity;
            var createCommands = _commandTree.CreateApplicationCommands();
            if (!createCommands.IsSuccess)
            {
                return Result.FromError(createCommands);
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
                return Result.FromError(updateResult);
            }

            // Update our command mapping
            var discordTree = updateResult.Entity;
            this.CommandMap = _commandTree.MapDiscordCommands(discordTree);

            return Result.FromSuccess();
        }
    }
}
