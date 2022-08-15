//
//  ModalInteractions.cs
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

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Interactions;

/// <summary>
/// Handles modal interactions.
/// </summary>
public class ModalInteractions : InteractionGroup
{
    private readonly ILogger<ModalInteractions> _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModalInteractions"/> class.
    /// </summary>
    /// <param name="log">The logging instance for this type.</param>
    public ModalInteractions(ILogger<ModalInteractions> log)
    {
        _log = log;
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
}
