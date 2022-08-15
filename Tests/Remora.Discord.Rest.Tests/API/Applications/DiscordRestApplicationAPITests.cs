//
//  DiscordRestApplicationAPITests.cs
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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Applications;

/// <summary>
/// Tests the <see cref="DiscordRestApplicationAPI"/> class.
/// </summary>
public class DiscordRestApplicationAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetGlobalApplicationCommandsAsync"/> method.
    /// </summary>
    public class GetGlobalApplicationCommandsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var withLocalizations = true;
            var locale = "en-GB";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithQueryString("with_localizations", withLocalizations.ToString())
                    .WithHeaders(Constants.LocaleHeaderName, locale)
                    .WithNoContent()
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGlobalApplicationCommandsAsync
            (
                applicationID,
                withLocalizations,
                locale
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.CreateGlobalApplicationCommandAsync"/> method.
    /// </summary>
    public class CreateGlobalApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly for chat command.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectlyForChatCommand()
        {
            var applicationID = DiscordSnowflake.New(0);
            var type = ApplicationCommandType.ChatInput;
            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();
            var permissions = new DiscordPermissionSet(DiscordPermission.Administrator);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                                .WithProperty("default_member_permissions", p => p.Is(permissions.Value.ToString()))
                                .WithProperty("dm_permission", p => p.Is(false))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options,
                defaultMemberPermissions: permissions,
                dmPermission: false,
                type: type
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly for user commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectlyForUserCommand()
        {
            var applicationID = DiscordSnowflake.New(0);
            var type = ApplicationCommandType.User;
            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("description", p => p.Is(string.Empty))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options,
                type: type
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly for message commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectlyForMessageCommand()
        {
            var applicationID = DiscordSnowflake.New(0);
            var type = ApplicationCommandType.Message;
            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("description", p => p.Is(string.Empty))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options,
                type: type
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var name = string.Empty;
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();
            var type = ApplicationCommandType.ChatInput;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options,
                type: type
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var name = new string('a', 33);
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var name = "aaa";
            var description = string.Empty;
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var name = "aaa";
            var description = new string('a', 101);
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.BulkOverwriteGlobalApplicationCommandsAsync"/> method.
    /// </summary>
    public class BulkOverwriteGlobalApplicationCommandsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    "aaa",
                    "bbbb",
                    Options: new List<ApplicationCommandOption>(),
                    DefaultMemberPermissions: new DiscordPermissionSet(default(BigInteger)),
                    Type: ApplicationCommandType.ChatInput
                ),
                new BulkApplicationCommandData
                (
                    "ccc",
                    "dddd",
                    Options: new List<ApplicationCommandOption>(),
                    DefaultMemberPermissions: new DiscordPermissionSet(DiscordPermission.Administrator),
                    Type: ApplicationCommandType.Message
                ),
                new BulkApplicationCommandData
                (
                    "eee",
                    "ffff"
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .WithJson
                    (
                        json => json.IsArray
                        (
                            a => a
                                .WithElement
                                (
                                    0,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(commands[0].Name))
                                            .WithProperty("type", p => p.Is((int)commands[0].Type.Value))
                                            .WithProperty("description", p => p.Is(commands[0].Description))
                                            .WithProperty("options", p => p.IsArray(
                                                ar => ar.WithCount(0)))
                                            .WithProperty("default_member_permissions", p => p.Is(commands[0].DefaultMemberPermissions!.Value.ToString()))
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(commands[1].Name))
                                            .WithProperty("type", p => p.Is((int)commands[1].Type.Value))
                                            .WithProperty("options", p => p.IsArray(
                                                ar => ar.WithCount(0)))
                                            .WithProperty("default_member_permissions", p => p.Is(commands[1].DefaultMemberPermissions!.Value.ToString()))
                                    )
                                )
                                .WithElement
                                (
                                    2,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(commands[2].Name))
                                            .WithoutProperty("type")
                                            .WithProperty("description", p => p.Is(commands[2].Description))
                                            .WithoutProperty("options")
                                            .WithProperty("default_member_permissions", p => p.IsNull())
                                    )
                                )
                        )
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.BulkOverwriteGlobalApplicationCommandsAsync
            (
                applicationID,
                commands
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    string.Empty,
                    "wwww"
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGlobalApplicationCommandsAsync
            (
                applicationID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    new string('a', 33),
                    "wwww"
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGlobalApplicationCommandsAsync
            (
                applicationID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    "aaa",
                    string.Empty
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGlobalApplicationCommandsAsync
            (
                applicationID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    "aaa",
                    new string('w', 101)
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGlobalApplicationCommandsAsync
            (
                applicationID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetGlobalApplicationCommandAsync"/> method.
    /// </summary>
    public class GetGlobalApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.GetGlobalApplicationCommandAsync
            (
                applicationID,
                commandID
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.EditGlobalApplicationCommandAsync"/> method.
    /// </summary>
    public class EditGlobalApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGlobalApplicationCommandAsync
            (
                applicationID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var name = string.Empty;
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGlobalApplicationCommandAsync
            (
                applicationID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var name = new string('a', 33);
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGlobalApplicationCommandAsync
            (
                applicationID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var name = "aaa";
            var description = string.Empty;
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGlobalApplicationCommandAsync
            (
                applicationID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var name = "aaa";
            var description = new string('a', 101);
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}")
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGlobalApplicationCommandAsync
            (
                applicationID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.DeleteGlobalApplicationCommandAsync"/> method.
    /// </summary>
    public class DeleteGlobalApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var commandID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete, $"{Constants.BaseURL}applications/{applicationID}/commands/{commandID}"
                    )
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGlobalApplicationCommandAsync(applicationID, commandID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetGuildApplicationCommandsAsync"/> method.
    /// </summary>
    public class GetGuildApplicationCommandsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var withLocalizations = true;
            var locale = "en-GB";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithQueryString("with_localizations", withLocalizations.ToString())
                    .WithHeaders(Constants.LocaleHeaderName, locale)
                    .WithNoContent()
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildApplicationCommandsAsync
            (
                applicationID,
                guildID,
                withLocalizations,
                locale
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.CreateGuildApplicationCommandAsync"/> method.
    /// </summary>
    public class CreateGuildApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly for chat commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectlyForChatCommands()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var type = ApplicationCommandType.ChatInput;
            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();
            var permissions = new DiscordPermissionSet(DiscordPermission.Administrator);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                                .WithProperty("default_member_permissions", p => p.Is(permissions.Value.ToString()))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options,
                defaultMemberPermissions: permissions,
                type: type
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly for user commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectlyForUserCommands()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var type = ApplicationCommandType.User;
            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("description", p => p.Is(string.Empty))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options,
                type: type
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly for message commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectlyForMessageCommands()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var type = ApplicationCommandType.Message;
            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("description", p => p.Is(string.Empty))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options,
                type: type
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var name = string.Empty;
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var name = new string('a', 33);
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var name = "aaa";
            var description = string.Empty;
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var name = "aaa";
            var description = new string('a', 101);
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.BulkOverwriteGuildApplicationCommandsAsync"/> method.
    /// </summary>
    public class BulkOverwriteGuildApplicationCommandsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    "aaa",
                    "bbbb",
                    Options: new List<ApplicationCommandOption>(),
                    DefaultMemberPermissions: new DiscordPermissionSet(default(BigInteger)),
                    Type: ApplicationCommandType.ChatInput
                ),
                new BulkApplicationCommandData
                (
                    "ccc",
                    "dddd",
                    Options: new List<ApplicationCommandOption>(),
                    DefaultMemberPermissions: new DiscordPermissionSet(DiscordPermission.Administrator),
                    Type: ApplicationCommandType.Message
                ),
                new BulkApplicationCommandData
                (
                    "eee",
                    "ffff"
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                    .WithJson
                    (
                        json => json.IsArray
                        (
                            a => a
                                .WithElement
                                (
                                    0,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(commands[0].Name))
                                            .WithProperty("type", p => p.Is((int)commands[0].Type.Value))
                                            .WithProperty("description", p => p.Is(commands[0].Description))
                                            .WithProperty("options", p => p.IsArray(
                                                ar => ar.WithCount(0)))
                                            .WithProperty("default_member_permissions", p => p.Is(commands[0].DefaultMemberPermissions!.Value.ToString()))
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(commands[1].Name))
                                            .WithProperty("type", p => p.Is((int)commands[1].Type.Value))
                                            .WithProperty("options", p => p.IsArray(
                                                ar => ar.WithCount(0)))
                                            .WithProperty("default_member_permissions", p => p.Is(commands[1].DefaultMemberPermissions!.Value.ToString()))
                                    )
                                )
                                .WithElement
                                (
                                    2,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(commands[2].Name))
                                            .WithoutProperty("type")
                                            .WithProperty("description", p => p.Is(commands[2].Description))
                                            .WithoutProperty("options")
                                            .WithProperty("default_member_permissions", p => p.IsNull())
                                    )
                                )
                        )
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.BulkOverwriteGuildApplicationCommandsAsync
            (
                applicationID,
                guildID,
                commands
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    string.Empty,
                    "wwww"
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGuildApplicationCommandsAsync
            (
                applicationID,
                guildID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    new string('a', 33),
                    "wwww"
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGuildApplicationCommandsAsync
            (
                applicationID,
                guildID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    "aaa",
                    string.Empty
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGuildApplicationCommandsAsync
            (
                applicationID,
                guildID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commands = new[]
            {
                new BulkApplicationCommandData
                (
                    "aaa",
                    new string('a', 101)
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.BulkOverwriteGuildApplicationCommandsAsync
            (
                applicationID,
                guildID,
                commands
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetGuildApplicationCommandAsync"/> method.
    /// </summary>
    public class GetGuildApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.GetGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                commandID
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.EditGuildApplicationCommandAsync"/> method.
    /// </summary>
    public class EditGuildApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var name = "aaa";
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var name = string.Empty;
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfNameIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var name = new string('a', 33);
            var description = "wwww";
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooShort()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var name = "aaa";
            var description = string.Empty;
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfDescriptionIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var name = "aaa";
            var description = new string('a', 101);
            var options = new List<ApplicationCommandOption>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.EditGuildApplicationCommandAsync
            (
                applicationID,
                guildID,
                commandID,
                name,
                description,
                options
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.DeleteGuildApplicationCommandAsync"/> method.
    /// </summary>
    public class DeleteGuildApplicationCommandAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}"
                    )
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildApplicationCommandAsync(applicationID, guildID, commandID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetGuildApplicationCommandPermissionsAsync"/> method.
    /// </summary>
    public class GetGuildApplicationCommandPermissionsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/permissions"
                    )
                    .WithNoContent()
                    .Respond("application/json", "[]")
            );

            var result = await api.GetGuildApplicationCommandPermissionsAsync(applicationID, guildID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetApplicationCommandPermissionsAsync"/> method.
    /// </summary>
    public class GetApplicationCommandPermissionsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}/permissions"
                    )
                    .WithNoContent()
                    .Respond
                    (
                        "application/json",
                        SampleRepository.Samples[typeof(IGuildApplicationCommandPermissions)]
                    )
            );

            var result = await api.GetApplicationCommandPermissionsAsync(applicationID, guildID, commandID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.EditApplicationCommandPermissionsAsync"/> method.
    /// </summary>
    public class EditApplicationCommandPermissionsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var guildID = DiscordSnowflake.New(1);
            var commandID = DiscordSnowflake.New(2);

            var permissions = Array.Empty<IApplicationCommandPermissions>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands/{commandID}/permissions"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o.WithProperty("permissions", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildApplicationCommandPermissions)])
            );

            var result = await api.EditApplicationCommandPermissionsAsync(applicationID, guildID, commandID, permissions);
            ResultAssert.Successful(result);
        }
    }
}
