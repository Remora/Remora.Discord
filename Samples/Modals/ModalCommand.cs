//
//  ModalCommand.cs
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

using System.ComponentModel;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Responders;
using Remora.Results;

namespace Remora.Discord.Samples.Modal;

/// <inheritdoc />
public class ModalCommand : CommandGroup
{
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly InteractionContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModalCommand"/> class.
    /// </summary>
    /// <param name="interactionApi">Discord interaction Api.</param>
    /// <param name="context">The Interaction's context.</param>
    public ModalCommand(IDiscordRestInteractionAPI interactionApi, InteractionContext context)
    {
        _interactionApi = interactionApi;
        _context = context;
    }

    /// <summary>
    /// Shows a modal.
    /// </summary>
    /// <returns>A result, indicating if the modal was sent successfully.</returns>
    [Command("modal")]
    [Description("Shows a modal")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [SuppressInteractionResponse(true)]
    public async Task<Result> OnModal()
    {
        var response = new InteractionResponse(
                                               InteractionCallbackType.Modal,
                                               new(
                                                   new InteractionModalCallbackData("test-modal", "Test Modal", new[]
                                                           {
                                                               new ActionRowComponent(new[]
                                                               {
                                                                   new TextInputComponent(
                                                                    "modal-text-input",
                                                                    TextInputStyle.Short,
                                                                    "Short Text",
                                                                    1,
                                                                    32,
                                                                    true,
                                                                    string.Empty,
                                                                    "Short Text here")
                                                               })
                                                           }
                                                       )));

        var result =
            await _interactionApi.CreateInteractionResponseAsync(_context.ID, _context.Token, response, ct: CancellationToken);
        return result;
    }
}
