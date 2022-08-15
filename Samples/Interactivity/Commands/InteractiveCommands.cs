//
//  InteractiveCommands.cs
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

using System.Linq;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Discord.Pagination.Extensions;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Commands;

/// <summary>
/// Defines commands with various interactivity types.
/// </summary>
[Group("interactive")]
public class InteractiveCommands : CommandGroup
{
    private readonly ICommandContext _context;
    private readonly FeedbackService _feedback;
    private readonly IDiscordRestInteractionAPI _interactionAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractiveCommands"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="feedback">The feedback service.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    public InteractiveCommands
    (
        ICommandContext context,
        FeedbackService feedback,
        IDiscordRestInteractionAPI interactionAPI
    )
    {
        _context = context;
        _feedback = feedback;
        _interactionAPI = interactionAPI;
    }

    /// <summary>
    /// Sends a paginated embed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("pages")]
    public async Task<IResult> SendPagesAsync()
    {
        // Create some pages
        var imageCaptions = new[]
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            "Morbi sollicitudin velit eu nisi sagittis condimentum.",
            "Donec molestie ex ac mollis vestibulum.",
            "Nam mollis felis at accumsan consectetur."
        };

        var images = new[]
        {
            "https://picsum.photos/800",
            "https://picsum.photos/801",
            "https://picsum.photos/802",
            "https://picsum.photos/803"
        };

        var pages = imageCaptions.Zip
        (
            images,
            (c, i) => new Embed(Image: new EmbedImage(i), Description: c)
        ).ToList();

        return await _feedback.SendContextualPaginatedMessageAsync
        (
            _context.User.ID,
            pages,
            ct: this.CancellationToken
        );
    }

    /// <summary>
    /// Sends an embed with a dropdown.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("dropdown")]
    public async Task<IResult> SendDropdownAsync()
    {
        var embed = new Embed(Description: "Select a colour below.");
        var options = new FeedbackMessageOptions(MessageComponents: new IMessageComponent[]
        {
            new ActionRowComponent(new[]
            {
                new SelectMenuComponent
                (
                    CustomIDHelpers.CreateSelectMenuID("colour-dropdown"),
                    new ISelectOption[]
                    {
                        new SelectOption("Red", "#FF0000"),
                        new SelectOption("Green", "#00FF00"),
                        new SelectOption("Blue", "#0000FF"),
                        new SelectOption("Cyan", "#00FFFF"),
                        new SelectOption("Magenta", "#FF00FF"),
                        new SelectOption("Yellow", "#FFFF00"),
                        new SelectOption("Black", "#000000"),
                        new SelectOption("White", "#FFFFFF")
                    },
                    "Colours...",
                    1,
                    1
                )
            })
        });

        return await _feedback.SendContextualEmbedAsync(embed, options, this.CancellationToken);
    }

    /// <summary>
    /// Sends an embed with a dropdown.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("typed-dropdown")]
    public async Task<IResult> SendTypedDropdownAsync()
    {
        var embed = new Embed(Description: "Select an emoji below.");
        var options = new FeedbackMessageOptions(MessageComponents: new IMessageComponent[]
        {
            new ActionRowComponent(new[]
            {
                new SelectMenuComponent
                (
                    CustomIDHelpers.CreateSelectMenuID("typed-dropdown"),
                    new ISelectOption[]
                    {
                        new SelectOption("Sweden", "🇸🇪", Emoji: new PartialEmoji(Name: "🇸🇪")),
                        new SelectOption("Robot", "🤖", Emoji: new PartialEmoji(Name: "🤖")),
                        new SelectOption("Shark", "🦈", Emoji: new PartialEmoji(Name: "🦈"))
                    },
                    "Emojis...",
                    1,
                    1
                )
            })
        });

        return await _feedback.SendContextualEmbedAsync(embed, options, this.CancellationToken);
    }

    /// <summary>
    /// Shows a modal.
    /// </summary>
    /// <returns>A result, indicating if the modal was sent successfully.</returns>
    [Command("modal")]
    [SuppressInteractionResponse(true)]
    public async Task<Result> ShowModalAsync()
    {
        if (_context is not InteractionContext interactionContext)
        {
            return (Result)await _feedback.SendContextualWarningAsync
            (
                "This command can only be used with slash commands.",
                _context.User.ID,
                new FeedbackMessageOptions(MessageFlags: MessageFlags.Ephemeral)
            );
        }

        var response = new InteractionResponse
        (
            InteractionCallbackType.Modal,
            new
            (
                new InteractionModalCallbackData
                (
                    CustomIDHelpers.CreateModalID("modal"),
                    "Test Modal",
                    new[]
                    {
                        new ActionRowComponent
                        (
                            new[]
                            {
                                new TextInputComponent
                                (
                                    "modal-text-input",
                                    TextInputStyle.Short,
                                    "Short Text",
                                    1,
                                    32,
                                    true,
                                    string.Empty,
                                    "Short Text here"
                                )
                            }
                        )
                    }
                )
            )
        );

        var result = await _interactionAPI.CreateInteractionResponseAsync
        (
            interactionContext.ID,
            interactionContext.Token,
            response,
            ct: this.CancellationToken
        );

        return result;
    }
}
