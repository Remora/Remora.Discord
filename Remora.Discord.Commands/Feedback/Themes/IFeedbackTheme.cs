//
//  IFeedbackTheme.cs
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

using System.Drawing;
using JetBrains.Annotations;

namespace Remora.Discord.Commands.Feedback.Themes;

/// <summary>
/// Represents the public API of a colour theme for feedback messages.
/// </summary>
[PublicAPI]
public interface IFeedbackTheme
{
    /// <summary>
    /// Gets a colour used as a background for things interposed with each other; image backgrounds, text
    /// background, etc.
    /// </summary>
    Color Background { get; }

    /// <summary>
    /// Gets a colour used for primary text elements superimposed on the <see cref="Background"/> colour.
    /// </summary>
    Color Text { get; }

    /// <summary>
    /// Gets a colour used for secondary text elements superimposed on an element, such as <see cref="Primary"/> or
    /// <see cref="Secondary"/>.
    /// </summary>
    Color TextSecondary { get; }

    /// <summary>
    /// Gets a variant of <see cref="TextSecondary"/>, which is used to indicate the associated element is currently
    /// non-interactive.
    /// </summary>
    Color TextSecondaryDisabled { get; }

    /// <summary>
    /// Gets a colour used as a primary element colour; buttons, links, etc.
    /// </summary>
    Color Primary { get; }

    /// <summary>
    /// Gets a variant of <see cref="Primary"/>, which is used to indicate the associated element is currently
    /// non-interactive.
    /// </summary>
    Color PrimaryDisabled { get; }

    /// <summary>
    /// Gets a colour used as a secondary element colour; less important buttons, bulk elements, etc.
    /// </summary>
    Color Secondary { get; }

    /// <summary>
    /// Gets a variant of <see cref="Secondary"/>, which is used to indicate the associated element is currently
    /// non-interactive.
    /// </summary>
    Color SecondaryDisabled { get; }

    /// <summary>
    /// Gets a colour used to indicate success; positive actions, completed requests, etc.
    /// </summary>
    Color Success { get; }

    /// <summary>
    /// Gets a variant of <see cref="Success"/>, which is used to indicate the associated element is currently
    /// non-interactive.
    /// </summary>
    Color SuccessDisabled { get; }

    /// <summary>
    /// Gets a colour used to indicate a warning; actions that partially succeeded, something a user has to take
    /// note of, etc.
    /// </summary>
    Color Warning { get; }

    /// <summary>
    /// Gets a variant of <see cref="Warning"/>, which is used to indicate the associated element is currently
    /// non-interactive.
    /// </summary>
    Color WarningDisabled { get; }

    /// <summary>
    /// Gets a colour used to indicate a fault or a dangerous action; a failed request, interacting with the element
    /// will be destructive, etc.
    /// </summary>
    Color FaultOrDanger { get; }

    /// <summary>
    /// Gets a variant of <see cref="FaultOrDanger"/>, which is used to indicate the associated element is currently
    /// non-interactive.
    /// </summary>
    Color FaultOrDangerDisabled { get; }
}
