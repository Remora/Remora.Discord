//
//  PaginatedAppearanceOptions.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.Pagination;

/// <summary>
/// Represents a set of appearance options for a paginated message.
/// </summary>
[PublicAPI]
public sealed record PaginatedAppearanceOptions
(
    ButtonComponent First,
    ButtonComponent Previous,
    ButtonComponent Next,
    ButtonComponent Last,
    ButtonComponent Close,
    ButtonComponent Help,
    string FooterFormat = "Page {0}/{1}",
    string HelpText = "This is a paginated message. Use the buttons to change page."
)
{
    /// <summary>
    /// Gets the default appearance instance.
    /// </summary>
    public static PaginatedAppearanceOptions Default { get; } = new
    (
        new ButtonComponent
        (
            ButtonComponentStyle.Secondary,
            Emoji: new PartialEmoji(Name: "⏮"),
            Label: nameof(First)
        ),
        new ButtonComponent
        (
            ButtonComponentStyle.Secondary,
            Emoji: new PartialEmoji(Name: "◀"),
            Label: nameof(Previous)
        ),
        new ButtonComponent
        (
            ButtonComponentStyle.Secondary,
            Emoji: new PartialEmoji(Name: "▶"),
            Label: nameof(Next)
        ),
        new ButtonComponent
        (
            ButtonComponentStyle.Secondary,
            Emoji: new PartialEmoji(Name: "⏭"),
            Label: nameof(Last)
        ),
        new ButtonComponent
        (
            ButtonComponentStyle.Secondary,
            Emoji: new PartialEmoji(Name: "\x23F9"),
            Label: nameof(Close)
        ),
        new ButtonComponent
        (
            ButtonComponentStyle.Secondary,
            Emoji: new PartialEmoji(Name: "ℹ"),
            Label: nameof(Help)
        )
    );

    /// <summary>
    /// Gets the appearance options' configured buttons as an array.
    /// </summary>
    public IReadOnlyList<ButtonComponent> Buttons { get; } = new[]
    {
        First,
        Previous,
        Next,
        Last,
        Close,
        Help
    };
}
