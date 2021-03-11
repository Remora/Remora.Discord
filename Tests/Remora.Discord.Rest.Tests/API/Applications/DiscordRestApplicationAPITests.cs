//
//  DiscordRestApplicationAPITests.cs
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Applications
{
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
                var applicationID = new Snowflake(0);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}applications/{applicationID}/commands")
                        .WithNoContent()
                        .Respond("application/json", "[ ]")
                );

                var result = await api.GetGlobalApplicationCommandsAsync
                (
                    applicationID
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
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
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

                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the API method returns a client-side error if a failure condition is met.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task ReturnsUnsuccessfulIfNameIsTooShort()
            {
                var applicationID = new Snowflake(0);
                var name = string.Empty;
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
            public async Task ReturnsUnsuccessfulIfNameIsTooLong()
            {
                var applicationID = new Snowflake(0);
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
                var applicationID = new Snowflake(0);
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
                var applicationID = new Snowflake(0);
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
        /// Tests the <see cref="DiscordRestApplicationAPI.CreateGlobalApplicationCommandsAsync"/> method.
        /// </summary>
        public class CreateGlobalApplicationCommandsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var name = "aaa";
                var description = "wwww";
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/commands")
                        .WithJson
                        (
                            json => json.IsArray
                            (
                                a => a.WithSingleElement
                                (
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(name))
                                            .WithProperty("description", p => p.Is(description))
                                            .WithProperty("options", p => p.IsArray())
                                    )
                                )
                            )
                        )
                        .Respond("application/json", "[]")
                );

                var result = await api.CreateGlobalApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var name = string.Empty;
                var description = "wwww";
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGlobalApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var name = new string('a', 33);
                var description = "wwww";
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGlobalApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var name = "aaa";
                var description = string.Empty;
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGlobalApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var name = "aaa";
                var description = new string('a', 101);
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGlobalApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var commandID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Get,
                            $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands"
                        )
                        .WithNoContent()
                        .Respond("application/json", "[ ]")
                );

                var result = await api.GetGuildApplicationCommandsAsync
                (
                    applicationID,
                    guildID
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
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);

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

                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the API method returns a client-side error if a failure condition is met.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task ReturnsUnsuccessfulIfNameIsTooShort()
            {
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);

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
        /// Tests the <see cref="DiscordRestApplicationAPI.CreateGuildApplicationCommandsAsync"/> method.
        /// </summary>
        public class CreateGuildApplicationCommandsAsync : RestAPITestBase<IDiscordRestApplicationAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var name = "aaa";
                var description = "wwww";
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                        .WithJson
                        (
                            json => json.IsArray
                            (
                                a => a.WithSingleElement
                                (
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("name", p => p.Is(name))
                                            .WithProperty("description", p => p.Is(description))
                                            .WithProperty("options", p => p.IsArray())
                                    )
                                )
                            )
                        )
                        .Respond("application/json", "[]")
                );

                var result = await api.CreateGuildApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var name = string.Empty;
                var description = "wwww";
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGuildApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var name = new string('a', 33);
                var description = "wwww";
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGuildApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var name = "aaa";
                var description = string.Empty;
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGuildApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var name = "aaa";
                var description = new string('a', 101);
                Optional<IReadOnlyList<IApplicationCommandOption>> options = new List<ApplicationCommandOption>();

                var commands = new[] { (name, description, options) };

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Put, $"{Constants.BaseURL}applications/{applicationID}/guilds/{guildID}/commands")
                        .Respond("application/json", SampleRepository.Samples[typeof(IApplicationCommand)])
                );

                var result = await api.CreateGuildApplicationCommandsAsync
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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
                var applicationID = new Snowflake(0);
                var guildID = new Snowflake(1);
                var commandID = new Snowflake(2);

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
    }
}
