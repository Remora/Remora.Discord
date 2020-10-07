//
//  DiceRollResponder.cs
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
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Json;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Samples.DiceRoller.API;
using Remora.Results;

namespace Remora.Discord.Samples.DiceRoller.Responders
{
    /// <summary>
    /// Responds to requests for dice rolls.
    /// </summary>
    public class DiceRollResponder : IResponder<IMessageCreate>, IResponder<IMessageUpdate>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscordRestChannelAPI _channelAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiceRollResponder"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The http client factory used for API requests.</param>
        /// <param name="channelAPI">The Discord channel API.</param>
        public DiceRollResponder(IHttpClientFactory httpClientFactory, IDiscordRestChannelAPI channelAPI)
        {
            _httpClient = httpClientFactory.CreateClient();
            _channelAPI = channelAPI;
        }

        /// <inheritdoc />
        public async Task<EventResponseResult> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.Content.StartsWith('!'))
            {
                return EventResponseResult.FromSuccess();
            }

            return await RollDiceAsync(gatewayEvent.Content, gatewayEvent.ChannelID);
        }

        /// <inheritdoc />
        public async Task<EventResponseResult> RespondAsync(IMessageUpdate gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.Content.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            return await RollDiceAsync(gatewayEvent.Content.Value!, gatewayEvent.ChannelID);
        }

        private async Task<EventResponseResult> RollDiceAsync(string value, Snowflake channel)
        {
            var parsedRollRequests = ParseRollRequests(value);
            if (parsedRollRequests.Length == 0)
            {
                return EventResponseResult.FromSuccess();
            }

            var getRolls = await GetRollsAsync(parsedRollRequests);
            if (!getRolls.IsSuccess)
            {
                var replyWithFailure = await ReplyWithFailureAsync(channel);

                return replyWithFailure.IsSuccess
                    ? EventResponseResult.FromError(getRolls)
                    : replyWithFailure;
            }

            var rollResponse = getRolls.Entity;

            return await ReplyWithRollsAsync(channel, rollResponse);
        }

        private async Task<RetrieveEntityResult<RollResponse>> GetRollsAsync(string[] parsedRollRequests)
        {
            var requestUrl = $"http://roll.diceapi.com/json/{string.Join('/', parsedRollRequests)}";

            using var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return RetrieveEntityResult<RollResponse>.FromError(response.ReasonPhrase);
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = new SnakeCaseNamingPolicy() };
            var rollResponse = await JsonSerializer.DeserializeAsync<RollResponse>(responseStream, jsonOptions);

            return !rollResponse.Success
                ? RetrieveEntityResult<RollResponse>.FromError("Dice rolling failed :(")
                : rollResponse;
        }

        private async Task<EventResponseResult> ReplyWithFailureAsync(Snowflake channel)
        {
            var failEmbed = new Embed(description: "Dice rolling failed :(", colour: Color.OrangeRed);

            var replyFail = await _channelAPI.CreateMessageAsync(channel, embed: failEmbed);

            return !replyFail.IsSuccess
                ? EventResponseResult.FromError(replyFail)
                : EventResponseResult.FromSuccess();
        }

        private async Task<EventResponseResult> ReplyWithRollsAsync(Snowflake channel, RollResponse rollResponse)
        {
            var rolls = rollResponse.Dice
                .GroupBy(d => d.Type)
                .ToDictionary
                (
                    g => g.Key,
                    g => g.Select(d => d.Value).Aggregate((a, b) => a + b)
                );

            var fields = rolls.Select(kvp => new EmbedField(kvp.Key, kvp.Value.ToString(), true)).ToList();
            var embed = new Embed("Rolls", fields: fields, colour: Color.LawnGreen);

            var replyRolls = await _channelAPI.CreateMessageAsync(channel, embed: embed);

            return !replyRolls.IsSuccess
                ? EventResponseResult.FromError(replyRolls)
                : EventResponseResult.FromSuccess();
        }

        private string[] ParseRollRequests(string value)
        {
            if (value.Length <= 5)
            {
                return Array.Empty<string>();
            }

            if (value[0] != '!')
            {
                return Array.Empty<string>();
            }

            if (value[1..5] != "roll")
            {
                return Array.Empty<string>();
            }

            return value[5..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
