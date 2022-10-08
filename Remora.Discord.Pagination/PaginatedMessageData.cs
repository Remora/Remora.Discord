//
//  PaginatedMessageData.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Interactivity;
using Remora.Rest.Core;

namespace Remora.Discord.Pagination;

/// <summary>
/// Represents in-memory persistent data for a paginated message.
/// </summary>
internal sealed class PaginatedMessageData
{
    private readonly IReadOnlyList<Embed> _pages;
    private int _currentPage;

    /// <summary>
    /// Gets a value indicating whether the paginated message was created as part of an interaction.
    /// </summary>
    public bool IsInteractionDriven { get; }

    /// <summary>
    /// Gets the appearance options for the message.
    /// </summary>
    public PaginatedAppearanceOptions Appearance { get; }

    /// <summary>
    /// Gets the ID of the source user.
    /// </summary>
    public Snowflake SourceUserID { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedMessageData"/> class.
    /// </summary>
    /// <param name="isInteractionDriven">
    /// Indicates whether the paginated message was created as part of an interaction.
    /// </param>
    /// <param name="sourceUserID">The ID of the source user.</param>
    /// <param name="pages">The pages in the paginated message.</param>
    /// <param name="appearance">The appearance options.</param>
    public PaginatedMessageData
    (
        bool isInteractionDriven,
        Snowflake sourceUserID,
        IReadOnlyList<Embed> pages,
        PaginatedAppearanceOptions? appearance = null
    )
    {
        appearance ??= PaginatedAppearanceOptions.Default;

        _pages = pages;
        this.IsInteractionDriven = isInteractionDriven;
        _currentPage = 0;

        this.SourceUserID = sourceUserID;
        this.Appearance = appearance;
    }

    /// <summary>
    /// Moves the paginated message to the next page.
    /// </summary>
    /// <returns>True if the page changed.</returns>
    public bool MoveNext()
    {
        if (_currentPage >= _pages.Count - 1)
        {
            return false;
        }

        _currentPage += 1;
        return true;
    }

    /// <summary>
    /// Moves the paginated message to the previous page.
    /// </summary>
    /// <returns>True if the page changed.</returns>
    public bool MovePrevious()
    {
        if (_currentPage <= 0)
        {
            return false;
        }

        _currentPage -= 1;
        return true;
    }

    /// <summary>
    /// Moves the paginated message to the first page.
    /// </summary>
    /// <returns>True if the page changed.</returns>
    public bool MoveFirst()
    {
        if (_currentPage == 0)
        {
            return false;
        }

        _currentPage = 0;
        return true;
    }

    /// <summary>
    /// Moves the paginated message to the last page.
    /// </summary>
    /// <returns>True if the page changed.</returns>
    public bool MoveLast()
    {
        if (_currentPage == _pages.Count - 1)
        {
            return false;
        }

        _currentPage = _pages.Count - 1;
        return true;
    }

    /// <summary>
    /// Gets the current page.
    /// </summary>
    /// <returns>The page.</returns>
    public Embed GetCurrentPage()
    {
        return _pages[_currentPage] with
        {
            Footer = new EmbedFooter(string.Format(this.Appearance.FooterFormat, _currentPage + 1, _pages.Count))
        };
    }

    /// <summary>
    /// Gets the current set of components that the message should have.
    /// </summary>
    /// <returns>The buttons.</returns>
    public IReadOnlyList<IMessageComponent> GetCurrentComponents()
    {
        return new[]
        {
            new ActionRowComponent
            (
                new[]
                {
                    this.Appearance.First with
                    {
                        CustomID = CustomIDHelpers.CreateButtonID("first"),
                        IsDisabled = _currentPage == 0
                    },
                    this.Appearance.Previous with
                    {
                        CustomID = CustomIDHelpers.CreateButtonID("previous"),
                        IsDisabled = _currentPage == 0
                    },
                    this.Appearance.Next with
                    {
                        CustomID = CustomIDHelpers.CreateButtonID("next"),
                        IsDisabled = _currentPage == _pages.Count - 1
                    },
                    this.Appearance.Last with
                    {
                        CustomID = CustomIDHelpers.CreateButtonID("last"),
                        IsDisabled = _currentPage == _pages.Count - 1
                    }
                }
            ),
            new ActionRowComponent
            (
                new[]
                {
                    this.Appearance.Close with { CustomID = CustomIDHelpers.CreateButtonID("close") },
                    this.Appearance.Help with { CustomID = CustomIDHelpers.CreateButtonID("help") }
                }
            )
        };
    }
}
