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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
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
        /// Initializes a new instance of the <see cref="GetGlobalApplicationCommandsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGlobalApplicationCommandsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
                    .WithExactQueryString("with_localizations", withLocalizations.ToString())
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
        /// Initializes a new instance of the <see cref="CreateGlobalApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGlobalApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
            var nsfw = true;

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
                                .WithProperty("nsfw", p => p.Is(nsfw))
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
                type: type,
                isNsfw: nsfw
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
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                options: options,
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
                                .WithProperty("options", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
            );

            var result = await api.CreateGlobalApplicationCommandAsync
            (
                applicationID,
                name,
                options: options,
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
        /// Initializes a new instance of the <see cref="BulkOverwriteGlobalApplicationCommandsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public BulkOverwriteGlobalApplicationCommandsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
                                            .WithProperty("options", p => p.IsArray(ar => ar.WithCount(0)))
                                            .WithProperty
                                            (
                                                "default_member_permissions",
                                                p => p.Is(commands[0].DefaultMemberPermissions!.Value.ToString())
                                            )
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
                                            .WithProperty("options", p => p.IsArray(ar => ar.WithCount(0)))
                                            .WithProperty
                                            (
                                                "default_member_permissions",
                                                p => p.Is(commands[1].DefaultMemberPermissions!.Value.ToString())
                                            )
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
        /// Initializes a new instance of the <see cref="GetGlobalApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGlobalApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
        /// Initializes a new instance of the <see cref="EditGlobalApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EditGlobalApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
            var nsfw = true;

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
                                .WithProperty("nsfw", p => p.Is(nsfw))
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
                options,
                isNsfw: nsfw
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
        /// Initializes a new instance of the <see cref="DeleteGlobalApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteGlobalApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
        /// Initializes a new instance of the <see cref="GetGuildApplicationCommandsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildApplicationCommandsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
                    .WithExactQueryString("with_localizations", withLocalizations.ToString())
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
        /// Initializes a new instance of the <see cref="CreateGuildApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGuildApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
            var nsfw = true;

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
                                .WithProperty("nsfw", p => p.Is(nsfw))
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
                type: type,
                isNsfw: nsfw
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
                options: options,
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
                options: options,
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
        /// Initializes a new instance of the <see cref="BulkOverwriteGuildApplicationCommandsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public BulkOverwriteGuildApplicationCommandsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
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
                                            .WithProperty("options", p => p.IsArray(ar => ar.WithCount(0)))
                                            .WithProperty
                                            (
                                                "default_member_permissions",
                                                p => p.Is(commands[0].DefaultMemberPermissions!.Value.ToString())
                                            )
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
                                            .WithProperty("options", p => p.IsArray(ar => ar.WithCount(0)))
                                            .WithProperty
                                            (
                                                "default_member_permissions",
                                                p => p.Is(commands[1].DefaultMemberPermissions!.Value.ToString())
                                            )
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
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
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
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
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
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
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
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                    )
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
        /// Initializes a new instance of the <see cref="GetGuildApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
        /// Initializes a new instance of the <see cref="EditGuildApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EditGuildApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
            var nsfw = true;

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
                                .WithProperty("nsfw", p => p.Is(nsfw))
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
                options,
                isNsfw: nsfw
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
        /// Initializes a new instance of the <see cref="DeleteGuildApplicationCommandAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteGuildApplicationCommandAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
        /// Initializes a new instance of the <see cref="GetGuildApplicationCommandPermissionsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildApplicationCommandPermissionsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
        /// Initializes a new instance of the <see cref="GetApplicationCommandPermissionsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetApplicationCommandPermissionsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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
        /// Initializes a new instance of the <see cref="EditApplicationCommandPermissionsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EditApplicationCommandPermissionsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

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

            var result = await api.EditApplicationCommandPermissionsAsync
            (
                applicationID,
                guildID,
                commandID,
                permissions
            );
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetApplicationRoleConnectionMetadataRecordsAsync"/> method.
    /// </summary>
    public class GetApplicationRoleConnectionMetadataRecordsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetApplicationRoleConnectionMetadataRecordsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetApplicationRoleConnectionMetadataRecordsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}applications/{applicationID}/role-connections/metadata"
                    )
                    .WithNoContent()
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetApplicationRoleConnectionMetadataRecordsAsync
            (
                applicationID
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.UpdateApplicationRoleConnectionMetadataRecordsAsync"/> method.
    /// </summary>
    public class UpdateApplicationRoleConnectionMetadataRecordsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateApplicationRoleConnectionMetadataRecordsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public UpdateApplicationRoleConnectionMetadataRecordsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var records = new[]
            {
                new ApplicationRoleConnectionMetadata
                (
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    ApplicationRoleConnectionMetadataType.IntegerLessThanOrEqual
                ),
                new ApplicationRoleConnectionMetadata
                (
                    new string('d', 50),
                    new string('e', 100),
                    new string('f', 200),
                    ApplicationRoleConnectionMetadataType.IntegerGreaterThanOrEqual
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/role-connections/metadata"
                    )
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
                                            .WithProperty("key", p => p.Is(records[0].Key))
                                            .WithProperty("name", p => p.Is(records[0].Name))
                                            .WithProperty("description", p => p.Is(records[0].Description))
                                            .WithProperty("type", p => p.Is((int)records[0].Type))
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("key", p => p.Is(records[1].Key))
                                            .WithProperty("name", p => p.Is(records[1].Name))
                                            .WithProperty("description", p => p.Is(records[1].Description))
                                            .WithProperty("type", p => p.Is((int)records[1].Type))
                                    )
                                )
                        )
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.UpdateApplicationRoleConnectionMetadataRecordsAsync
            (
                applicationID,
                records
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method returns a client-side error if a failure condition is met.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsUnsuccessfulIfKeyIsTooLong()
        {
            var applicationID = DiscordSnowflake.New(0);
            var records = new[]
            {
                new ApplicationRoleConnectionMetadata
                (
                    new string('a', 51),
                    string.Empty,
                    string.Empty,
                    ApplicationRoleConnectionMetadataType.IntegerLessThanOrEqual
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/role-connections/metadata"
                    )
                    .Respond
                    (
                        "application/json",
                        $"[ {SampleRepository.Samples[typeof(IApplicationRoleConnectionMetadata)]} ]"
                    )
            );

            var result = await api.UpdateApplicationRoleConnectionMetadataRecordsAsync
            (
                applicationID,
                records
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
            var records = new[]
            {
                new ApplicationRoleConnectionMetadata
                (
                    string.Empty,
                    new string('b', 101),
                    string.Empty,
                    ApplicationRoleConnectionMetadataType.IntegerLessThanOrEqual
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/role-connections/metadata"
                    )
                    .Respond
                    (
                        "application/json",
                        $"[ {SampleRepository.Samples[typeof(IApplicationRoleConnectionMetadata)]} ]"
                    )
            );

            var result = await api.UpdateApplicationRoleConnectionMetadataRecordsAsync
            (
                applicationID,
                records
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
            var records = new[]
            {
                new ApplicationRoleConnectionMetadata
                (
                    string.Empty,
                    string.Empty,
                    new string('c', 201),
                    ApplicationRoleConnectionMetadataType.IntegerLessThanOrEqual
                )
            };

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}applications/{applicationID}/role-connections/metadata"
                    )
                    .Respond
                    (
                        "application/json",
                        $"[ {SampleRepository.Samples[typeof(IApplicationRoleConnectionMetadata)]} ]"
                    )
            );

            var result = await api.UpdateApplicationRoleConnectionMetadataRecordsAsync
            (
                applicationID,
                records
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.GetCurrentApplicationAsync"/> method.
    /// </summary>
    public class GetCurrentApplicationAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetCurrentApplicationAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetCurrentApplicationAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}applications/@me"
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplication)])
            );

            var result = await api.GetCurrentApplicationAsync();

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestApplicationAPI.EditCurrentApplicationAsync"/> method.
    /// </summary>
    public class EditCurrentApplicationAsync : RestAPITestBase<IDiscordRestApplicationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCurrentApplicationAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EditCurrentApplicationAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var customInstallUrl = new Uri("https://example.org/install");
            var description = "aaa";
            var roleConnectionsVerificationUrl = new Uri("https://example.org/verify");
            var installParams = new ApplicationInstallParameters
            (
                Array.Empty<string>(),
                new DiscordPermissionSet(DiscordPermission.Administrator)
            );
            var flags = ApplicationFlags.ApplicationCommandBadge;

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            // Create a dummy PNG image
            await using var cover = new MemoryStream();
            await using var coverBinaryWriter = new BinaryWriter(cover);
            coverBinaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            cover.Position = 0;

            var interactionsEndpointUrl = new Uri("https://example.org/interact");
            var tags = new[] { "ooga", "booga" };

            var integrationTypes = new[]
            {
                ApplicationIntegrationType.UserInstallable,
                ApplicationIntegrationType.GuildInstallable
            };

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}applications/@me"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("custom_install_url", p => p.Is(customInstallUrl.ToString()))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty
                                (
                                    "role_connections_verification_url",
                                    p => p.Is(roleConnectionsVerificationUrl.ToString())
                                )
                                .WithProperty("install_params", p => p.IsObject())
                                .WithProperty("flags", p => p.Is((int)flags))
                                .WithProperty("icon", p => p.Is("data:image/png;base64,iVBORw0KGgo="))
                                .WithProperty("cover_image", p => p.Is("data:image/png;base64,iVBORw0KGgo="))
                                .WithProperty
                                (
                                    "interactions_endpoint_url",
                                    p => p.Is(interactionsEndpointUrl.ToString())
                                )
                                .WithProperty
                                (
                                    "tags",
                                    p => p.IsArray
                                    (
                                        a => a
                                            .WithElement(0, e => e.Is("ooga"))
                                            .WithElement(1, e => e.Is("booga"))
                                    )
                                )
                                .WithProperty
                                (
                                    "integration_types_config",
                                    p => p.IsObject
                                    (
                                        j => j.WithProperty("0", e => e.IsObject())
                                            .WithProperty("1", e => e.IsObject())
                                    )
                                )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplication)])
            );

            var result = await api.EditCurrentApplicationAsync
            (
                customInstallUrl,
                description,
                roleConnectionsVerificationUrl,
                installParams,
                flags,
                icon,
                cover,
                interactionsEndpointUrl,
                tags,
                integrationTypes
            );

            ResultAssert.Successful(result);
        }
    }
}
