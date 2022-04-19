//
// ModalReceiver.cs
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

using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Samples.Modal;

/// <inheritdoc />
public class ModalReceiver : IModalInteractiveEntity
{
    private readonly ILogger<ModalReceiver> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModalReceiver"/> class.
    /// </summary>
    /// <param name="logger">Logger used to log the received information.</param>
    public ModalReceiver(ILogger<ModalReceiver> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<Result<bool>> IsInterestedAsync(ComponentType? componentType, string customID, CancellationToken ct = default)
    {
        return Task.FromResult(Result<bool>.FromSuccess(customID == "test-modal"));
    }

    /// <inheritdoc />
    public Task<Result> HandleInteractionAsync(IUser user, string customID, IReadOnlyList<IPartialMessageComponent> components, CancellationToken ct = default)
    {
        _logger.LogInformation("Received modal response");
        var actionRow = (PartialActionRowComponent)components[0];
        var textInput = (PartialTextInputComponent)actionRow.Components.Value[0];
        _logger.LogInformation("Received input: {0}", textInput.Value);
        return Task.FromResult(Result.FromSuccess());
    }
}
