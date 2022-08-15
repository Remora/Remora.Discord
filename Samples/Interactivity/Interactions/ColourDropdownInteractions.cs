//
//  ColourDropdownInteractions.cs
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

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Remora.Commands.Results;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Interactions;

/// <summary>
/// Handles colour dropdown interactions.
/// </summary>
public class ColourDropdownInteractions : InteractionGroup
{
    private readonly InteractionContext _context;
    private readonly IDiscordRestChannelAPI _channelAPI;
    private readonly FeedbackService _feedback;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColourDropdownInteractions"/> class.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    /// <param name="channelAPI">The channel API.</param>
    /// <param name="feedback">The feedback service.</param>
    public ColourDropdownInteractions
    (
        InteractionContext context,
        IDiscordRestChannelAPI channelAPI,
        FeedbackService feedback
    )
    {
        _context = context;
        _channelAPI = channelAPI;
        _feedback = feedback;
    }

    /// <summary>
    /// Sets the colour of the associated embed.
    /// </summary>
    /// <param name="values">The selected values.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    [SelectMenu("colour-dropdown")]
    public async Task<Result> SetEmbedColourAsync(IReadOnlyList<string> values)
    {
        if (!_context.Message.IsDefined(out var message))
        {
            return new InvalidOperationError("Interaction without a message?");
        }

        if (values.Count != 1)
        {
            return new InvalidOperationError("Only one element may be selected at any one time.");
        }

        var colourCodeRaw = values.Single();
        if (!Regex.IsMatch(colourCodeRaw, "^#[0-9A-Fa-f]{6}$"))
        {
            return new ParsingError<Color>(colourCodeRaw);
        }

        var r = int.Parse(colourCodeRaw.Substring(1, 2), NumberStyles.HexNumber);
        var g = int.Parse(colourCodeRaw.Substring(3, 2), NumberStyles.HexNumber);
        var b = int.Parse(colourCodeRaw.Substring(5, 2), NumberStyles.HexNumber);

        var colour = Color.FromArgb(r, g, b);

        var embed = new Embed
        (
            Colour: colour,
            Description: $"This embed is the colour {colourCodeRaw}.",
            Footer: new EmbedFooter("Want another colour? Select one below")
        );

        var components = new List<IMessageComponent>(message.Components.Value);
        var dropdown = (ISelectMenuComponent)((IActionRowComponent)components[0]).Components[0];

        var selected = dropdown.Options.Single(o => o.Value == colourCodeRaw);
        var newComponents = new List<IMessageComponent>
        {
            new ActionRowComponent(new[]
            {
                new SelectMenuComponent
                (
                    dropdown.CustomID,
                    dropdown.Options,
                    selected.Label,
                    dropdown.MinValues,
                    dropdown.MaxValues,
                    dropdown.IsDisabled
                )
            })
        };

        return (Result)await _channelAPI.EditMessageAsync
        (
            _context.ChannelID,
            message.ID,
            embeds: new[] { embed },
            components: newComponents,
            ct: this.CancellationToken
        );
    }

    /// <summary>
    /// Explains a selected emoji.
    /// </summary>
    /// <param name="values">The selected values.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    [SelectMenu("typed-dropdown")]
    public async Task<Result> ExplainEmojiAsync(IReadOnlyList<IEmoji> values)
    {
        if (values.Count != 1)
        {
            return new InvalidOperationError("Only one element may be selected at any one time.");
        }

        var message = values.Single().Name switch
        {
            "🇸🇪" => "Someplace that gets cold in the winters.",
            "🤖" => "A machine that wants a heart.",
            "🦈" => "A misunderstood fish.",
            _ => "No clue."
        };

        return (Result)await _feedback.SendContextualNeutralAsync
        (
            message,
            _context.User.ID,
            options: new FeedbackMessageOptions(MessageFlags: MessageFlags.Ephemeral),
            ct: this.CancellationToken
        );
    }
}
