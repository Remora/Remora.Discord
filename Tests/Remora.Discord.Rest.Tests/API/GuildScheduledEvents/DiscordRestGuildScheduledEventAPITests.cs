//
//  DiscordRestGuildScheduledEventAPITests.cs
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

namespace Remora.Discord.Rest.Tests.API.GuildScheduledEvents;

/// <summary>
/// Tests the <see cref="DiscordRestGuildScheduledEventAPI"/> class.
/// </summary>
public class DiscordRestGuildScheduledEventAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestGuildScheduledEventAPI.ListScheduledEventsForGuildAsync"/> method.
    /// </summary>
    public class ListScheduledEventsForGuildAsync : RestAPITestBase<IDiscordRestGuildScheduledEventAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(1);
            var withUserCount = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildID}/scheduled-events")
                    .WithQueryString("with_user_count", withUserCount.ToString())
                    .Respond("application/json", "[]")
            );

            var result = await api.ListScheduledEventsForGuildAsync(guildID, withUserCount);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildScheduledEventAPI.CreateGuildScheduledEventAsync"/> method.
    /// </summary>
    public class CreateGuildScheduledEventAsync : RestAPITestBase<IDiscordRestGuildScheduledEventAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            image.Position = 0;

            var guildID = DiscordSnowflake.New(1);
            var channelID = DiscordSnowflake.New(2);
            var entityMetadata = new GuildScheduledEventEntityMetadata();
            var name = "wooga";
            var privacyLevel = GuildScheduledEventPrivacyLevel.GuildOnly;
            var scheduledStartTime = DateTimeOffset.UtcNow.AddDays(1);
            var scheduledEndTime = scheduledStartTime.AddHours(1);
            var description = "booga";
            var entityType = GuildScheduledEventEntityType.StageInstance;
            var reason = "aaa";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildID}/scheduled-events")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("channel_id", p => p.Is(channelID.ToString()))
                                .WithProperty("entity_metadata", p => p.IsObject())
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("privacy_level", p => p.Is((int)privacyLevel))

                                // DateTimeOffset comparisons "work", but do not exactly replicate the number of ticks
                                // in the structure, causing a failure here
                                .WithProperty("scheduled_start_time", p => p.IsString())
                                .WithProperty("scheduled_end_time", p => p.IsString())

                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("entity_type", p => p.Is((int)entityType))
                                .WithProperty("image", p => p.IsString())
                        )
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildScheduledEvent)])
            );

            var result = await api.CreateGuildScheduledEventAsync
            (
                guildID,
                channelID,
                entityMetadata,
                name,
                privacyLevel,
                scheduledStartTime,
                scheduledEndTime,
                description,
                entityType,
                image,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildScheduledEventAPI.GetGuildScheduledEventAsync"/> method.
    /// </summary>
    public class GetGuildScheduledEventAsync : RestAPITestBase<IDiscordRestGuildScheduledEventAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(1);
            var eventID = DiscordSnowflake.New(2);
            var withCounts = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildID}/scheduled-events/{eventID}")
                    .WithQueryString("with_user_count", withCounts.ToString())
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildScheduledEvent)])
            );

            var result = await api.GetGuildScheduledEventAsync(guildID, eventID, withCounts);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildScheduledEventAPI.ModifyGuildScheduledEventAsync"/> method.
    /// </summary>
    public class ModifyGuildScheduledEventAsync : RestAPITestBase<IDiscordRestGuildScheduledEventAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            image.Position = 0;

            var guildID = DiscordSnowflake.New(1);
            var eventID = DiscordSnowflake.New(2);
            var channelID = DiscordSnowflake.New(3);
            var entityMetadata = new GuildScheduledEventEntityMetadata();
            var name = "wooga";
            var privacyLevel = GuildScheduledEventPrivacyLevel.GuildOnly;
            var scheduledStartTime = DateTimeOffset.UtcNow.AddDays(1);
            var scheduledEndTime = scheduledStartTime.AddHours(1);
            var description = "booga";
            var entityType = GuildScheduledEventEntityType.StageInstance;
            var status = GuildScheduledEventStatus.Completed;
            var reason = "aaa";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildID}/scheduled-events/{eventID}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("channel_id", p => p.Is(channelID.ToString()))
                                .WithProperty("entity_metadata", p => p.IsObject())
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("privacy_level", p => p.Is((int)privacyLevel))

                                // DateTimeOffset comparisons "work", but do not exactly replicate the number of ticks
                                // in the structure, causing a failure here
                                .WithProperty("scheduled_start_time", p => p.IsString())
                                .WithProperty("scheduled_end_time", p => p.IsString())

                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("entity_type", p => p.Is((int)entityType))
                                .WithProperty("status", p => p.Is((int)status))
                                .WithProperty("image", p => p.IsString())
                        )
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildScheduledEvent)])
            );

            var result = await api.ModifyGuildScheduledEventAsync
            (
                guildID,
                eventID,
                channelID,
                entityMetadata,
                name,
                privacyLevel,
                scheduledStartTime,
                scheduledEndTime,
                description,
                entityType,
                status,
                image,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildScheduledEventAPI.DeleteGuildScheduledEventAsync"/> method.
    /// </summary>
    public class DeleteGuildScheduledEventAsync : RestAPITestBase<IDiscordRestGuildScheduledEventAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(1);
            var eventID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildID}/scheduled-events/{eventID}")
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildScheduledEventAsync(guildID, eventID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildScheduledEventAPI.GetGuildScheduledEventUsersAsync"/> method.
    /// </summary>
    public class GetGuildScheduledEventUsersAsync : RestAPITestBase<IDiscordRestGuildScheduledEventAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(1);
            var eventID = DiscordSnowflake.New(2);
            var limit = 10;
            var withMember = true;
            var before = DiscordSnowflake.New(3);
            var after = DiscordSnowflake.New(4);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildID}/scheduled-events/{eventID}/users")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("with_member", withMember.ToString()),
                            new KeyValuePair<string, string>("before", before.ToString()),
                            new KeyValuePair<string, string>("after", after.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildScheduledEventUsersAsync(guildID, eventID, limit, withMember, before, after);
            ResultAssert.Successful(result);
        }
    }
}
