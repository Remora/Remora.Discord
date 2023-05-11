//
//  SPDX-FileName: ModalInteractions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Interactions;

/// <summary>
/// Handles modal interactions.
/// </summary>
public class ModalInteractions : InteractionGroup
{
    private readonly ILogger<ModalInteractions> _log;
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModalInteractions"/> class.
    /// </summary>
    /// <param name="log">The logging instance for this type.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public ModalInteractions(ILogger<ModalInteractions> log, FeedbackService feedbackService)
    {
        _log = log;
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Logs submitted modal data.
    /// </summary>
    /// <param name="modalTextInput">The value of the modal text input component.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Modal("modal")]
    public Task<Result> OnModalSubmitAsync(string modalTextInput)
    {
        _log.LogInformation("Received modal response");
        _log.LogInformation("Received input: {Input}", modalTextInput);

        return Task.FromResult(Result.FromSuccess());
    }

    /// <summary>
    /// Logs submitted modal data.
    /// </summary>
    /// <param name="modalTextInput">The value of the modal text input component.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Modal("ephemeral-modal")]
    [Ephemeral]
    public async Task<Result> OnEphemeralModalSubmitAsync(string modalTextInput)
    {
        _log.LogInformation("Received modal response");
        _log.LogInformation("Received input: {Input}", modalTextInput);

        return (Result)await _feedbackService.SendContextualSuccessAsync(modalTextInput);
    }
}
