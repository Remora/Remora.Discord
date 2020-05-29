//
//  APITypeTestBase.cs
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

using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Remora.Discord.Gateway.API;
using Remora.Discord.Gateway.API.Events;
using Remora.Discord.Gateway.API.Json.ContractResolvers;
using Remora.Discord.Gateway.Services;
using Remora.Discord.Gateway.Tests.Services;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Gateway.Tests.TestBases
{
    /// <summary>
    /// Acts as a base class for testing API types in the Discord API. This class contains common baseline tests for all
    /// types.
    /// </summary>
    /// <typeparam name="TType">The type under test.</typeparam>
    public abstract class APITypeTestBase<TType>
    {
        /// <summary>
        /// Gets the sample data service.
        /// </summary>
        protected SampleDataService SampleData { get; }

        /// <summary>
        /// Gets the JSON service.
        /// </summary>
        protected DiscordJsonService DiscordJsonService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="APITypeTestBase{TType}"/> class.
        /// </summary>
        public APITypeTestBase()
        {
            this.SampleData = new SampleDataService();
            this.DiscordJsonService = new DiscordJsonService();
        }

        /// <summary>
        /// Gets some sample data for the given type.
        /// </summary>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        protected abstract RetrieveEntityResult<Stream> GetSampleData();

        /// <summary>
        /// Tests whether the type can be deserialized from a JSON object.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [SkippableFact]
        public async Task CanDeserialize()
        {
            var getSampleData = GetSampleData();
            if (!getSampleData.IsSuccess)
            {
                throw new SkipException(getSampleData.ErrorReason);
            }

            await using var sampleData = getSampleData.Entity;
            var payload = this.DiscordJsonService.Serializer.Deserialize<IPayload>
            (
                new JsonTextReader(new StreamReader(sampleData))
            );

            Assert.NotNull(payload);
        }

        /// <summary>
        /// Tests whether the type can be serialized to a JSON object.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [SkippableFact]
        public async Task CanSerialize()
        {
            var getSampleData = GetSampleData();
            if (!getSampleData.IsSuccess)
            {
                throw new SkipException(getSampleData.ErrorReason);
            }

            await using var sampleData = getSampleData.Entity;
            var payload = this.DiscordJsonService.Serializer.Deserialize<IPayload>
            (
                new JsonTextReader(new StreamReader(sampleData))
            );

            using var writer = new JsonTextWriter(new StreamWriter(new MemoryStream()));
            this.DiscordJsonService.Serializer.Serialize(writer, payload);

            Assert.NotNull(payload);
        }

        /// <summary>
        /// Tests whether data survives being round-tripped by the type - that is, it can be deserialized and then
        /// serialized again without data loss.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [SkippableFact]
        public async Task SurvivesRoundTrip()
        {
            var getSampleData = GetSampleData();
            if (!getSampleData.IsSuccess)
            {
                throw new SkipException(getSampleData.ErrorReason);
            }

            await using var sampleData = getSampleData.Entity;
            var deserialized = this.DiscordJsonService.Serializer.Deserialize<IPayload>
            (
                new JsonTextReader(new StreamReader(sampleData))
            );

            await using var ms = new MemoryStream();
            using var writer = new JsonTextWriter(new StreamWriter(ms));

            this.DiscordJsonService.Serializer.Serialize(writer, deserialized);

            ms.Seek(0, SeekOrigin.Begin);
            sampleData.Seek(0, SeekOrigin.Begin);

            for (var i = 0; i < ms.Length; i++)
            {
                Assert.Equal(ms.ReadByte(), sampleData.ReadByte());
            }
        }
    }
}
