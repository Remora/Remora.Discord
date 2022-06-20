//
//  ColourDropdownEntity.cs
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

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Results;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Interactivity;
using Remora.Results;

namespace Remora.Discord.Samples.Interactivity.Entities;

/// <summary>
/// Defines an interactive entity that changes the embed colour of its attached message based on a dropdown.
/// </summary>
public class ColourDropdownEntity : ISelectMenuInteractiveEntity
{
    private readonly InteractionContext _context;
    private readonly IDiscordRestChannelAPI _channelAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColourDropdownEntity"/> class.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    /// <param name="channelAPI">The channel API.</param>
    public ColourDropdownEntity(InteractionContext context, IDiscordRestChannelAPI channelAPI)
    {
        _context = context;
        _channelAPI = channelAPI;
    }

    /// <inheritdoc />
    public Task<Result<bool>> IsInterestedAsync
    (
        ComponentType? componentType,
        string customID,
        CancellationToken ct = default
    )
    {
        return componentType is not ComponentType.SelectMenu
            ? Task.FromResult<Result<bool>>(false)
            : Task.FromResult<Result<bool>>(customID is "colour-dropdown");
    }

    /// <inheritdoc />
    public async Task<Result> HandleInteractionAsync
    (
        IUser user,
        string customID,
        IReadOnlyList<string> values,
        CancellationToken ct = default
    )
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
            ct: ct
        );
    }
}
