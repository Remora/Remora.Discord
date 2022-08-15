//
//  DiceRollCommands.cs
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
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Samples.DiceRoller.API;
using Remora.Rest.Json.Policies;
using Remora.Results;

namespace Remora.Discord.Samples.DiceRoller.Commands;

/// <summary>
/// Contains commands for rolling dice.
/// </summary>
public class DiceRollCommands : CommandGroup
{
    private readonly HttpClient _httpClient;
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceRollCommands"/> class.
    /// </summary>
    /// <param name="httpClient">The http client used for API requests.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public DiceRollCommands(HttpClient httpClient, FeedbackService feedbackService)
    {
        _httpClient = httpClient;
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Rolls a dice using an online service.
    /// </summary>
    /// <param name="value">The command to send to the online service.</param>
    /// <returns>The result of the operation.</returns>
    [Command("roll")]
    public async Task<IResult> RollDiceAsync(string value)
    {
        var rollRequests = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (rollRequests.Length == 0)
        {
            return Result.FromSuccess();
        }

        var getRolls = await GetRollsAsync(rollRequests);
        if (!getRolls.IsSuccess)
        {
            return await ReplyWithFailureAsync();
        }

        var rollResponse = getRolls.Entity;

        return await ReplyWithRollsAsync(rollResponse);
    }

    private async Task<Result<RollResponse>> GetRollsAsync(string[] parsedRollRequests)
    {
        var requestUrl = $"http://roll.diceapi.com/json/{string.Join('/', parsedRollRequests)}";

        using var response = await _httpClient.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
        {
            return new InvalidOperationError(response.ReasonPhrase ?? "No reason given.");
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = new SnakeCaseNamingPolicy() };
        var rollResponse = await JsonSerializer.DeserializeAsync<RollResponse>(responseStream, jsonOptions);

        if (rollResponse is null)
        {
            return new InvalidOperationError("The roll response was null.");
        }

        return !rollResponse.Success
            ? new InvalidOperationError("Dice rolling failed :(")
            : rollResponse;
    }

    private async Task<Result> ReplyWithFailureAsync()
    {
        return (Result)await _feedbackService.SendContextualErrorAsync
        (
            "Dice rolling failed :(",
            ct: this.CancellationToken
        );
    }

    private async Task<Result> ReplyWithRollsAsync(RollResponse rollResponse)
    {
        var rolls = rollResponse.Dice
            .GroupBy(d => d.Type)
            .ToDictionary
            (
                g => g.Key,
                g => g.Select(d => d.Value).Aggregate((a, b) => a + b)
            );

        var fields = rolls.Select(kvp => new EmbedField(kvp.Key, kvp.Value.ToString(), true)).ToList();
        var embed = new Embed("Rolls", Fields: fields, Colour: _feedbackService.Theme.Success);

        return (Result)await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
    }
}
