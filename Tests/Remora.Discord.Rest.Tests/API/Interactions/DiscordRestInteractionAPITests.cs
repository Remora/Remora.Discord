//
//  DiscordRestInteractionAPITests.cs
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

namespace Remora.Discord.Rest.Tests.API.Interactions
{
    /// <summary>
    /// Tests the <see cref="DiscordRestInteractionAPI"/> class.
    /// </summary>
    public class DiscordRestInteractionAPITests
    {
        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.CreateInteractionResponseAsync"/> method.
        /// </summary>
        public class CreateInteractionResponseAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var interactionID = new Snowflake(0);
                var token = "aaaa";
                var interactionResponse = new InteractionResponse(InteractionResponseType.DeferredChannelMessageWithSource);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}interactions/{interactionID}/{token}/callback")
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o.WithProperty("type", p => p.Is(5))
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IInteractionResponse)])
                );

                var result = await api.CreateInteractionResponseAsync
                (
                    interactionID,
                    token,
                    interactionResponse
                );

                ResultAssert.Successful(result);
            }
        }
    }
}
