//
//  SPDX-FileName: StatefulInteractions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Interactions;

/// <summary>
/// Handles stateful interactions.
/// </summary>
public class StatefulInteractions : InteractionGroup
{
    private readonly ILogger<ModalInteractions> _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulInteractions"/> class.
    /// </summary>
    /// <param name="log">The logging instance for this type.</param>
    public StatefulInteractions(ILogger<ModalInteractions> log)
    {
        _log = log;
    }

    /// <summary>
    /// Logs the state associated with the button.
    /// </summary>
    /// <param name="state">The state passed along with the button, if any.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Button("stateful-button")]
    public Task<Result> OnStatefulButtonClicked(string? state = null)
    {
        _log.LogInformation("Button was pressed");
        _log.LogInformation("Received state: {State}", state ?? "No state was associated with button");

        return Task.FromResult(Result.FromSuccess());
    }
}
