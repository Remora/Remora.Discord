//
//  ModalInteractions.cs
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

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Interactivity.Attributes;
using Remora.Discord.Interactivity.Bases;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Interactions;

/// <summary>
/// Handles modal interactions.
/// </summary>
public class ModalInteractions : InteractionGroup
{
    private readonly ILogger<ModalInteractions> _log;
    private readonly InteractionContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModalInteractions"/> class.
    /// </summary>
    /// <param name="log">The logging instance for this type.</param>
    /// <param name="context">The interaction context.</param>
    public ModalInteractions(ILogger<ModalInteractions> log, InteractionContext context)
    {
        _log = log;
        _context = context;
    }

    /// <summary>
    /// Logs submitted modal data.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    [Modal("modal")]
    public Task<Result> OnModalSubmitAsync()
    {
        if (!_context.Data.TryPickT2(out var data, out _))
        {
            return Task.FromResult<Result>(new InvalidOperationError("The interaction data was not modal data."));
        }

        _log.LogInformation("Received modal response");
        var actionRow = (PartialActionRowComponent)data.Components[0];
        var textInput = (PartialTextInputComponent)actionRow.Components.Value[0];
        _log.LogInformation("Received input: {Input}", textInput.Value);

        return Task.FromResult(Result.FromSuccess());
    }
}
