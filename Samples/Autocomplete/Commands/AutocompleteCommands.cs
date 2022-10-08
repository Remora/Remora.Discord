//
//  SPDX-FileName: AutocompleteCommands.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System.Drawing;
using System.Threading.Tasks;
using Humanizer;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace Remora.Discord.Samples.SlashCommands.Commands;

/// <summary>
/// Responds to various commands with autocompletion support.
/// </summary>
public class AutocompleteCommands : CommandGroup
{
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocompleteCommands"/> class.
    /// </summary>
    /// <param name="feedbackService">The feedback service.</param>
    public AutocompleteCommands(FeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Displays an embed with a predefined colour.
    /// </summary>
    /// <param name="colour">The colour.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("display-colour")]
    public async Task<Result> DisplayColour(PredefinedColours colour)
    {
        var embed = new Embed
        (
            Description: $"This embed is {colour.Humanize().ToLowerInvariant()}.",
            Colour: Color.FromArgb((int)(0xFF000000 | (int)colour))
        );

        return (Result)await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
    }

    /// <summary>
    /// Displays an embed with a user-supplied word.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("display-word")]
    public async Task<Result> DisplayWord([AutocompleteProvider("autocomplete::dictionary")] string word)
    {
        return (Result)await _feedbackService.SendContextualNeutralAsync
        (
            $"Your word is \"{word}\".",
            ct: this.CancellationToken
        );
    }
}
