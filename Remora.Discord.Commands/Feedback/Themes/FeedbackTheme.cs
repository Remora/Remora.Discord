//
//  FeedbackTheme.cs
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
/// Represents a custom feedback theme.
/// </summary>
[PublicAPI]
public record FeedbackTheme
(
    Color Background,
    Color Text,
    Color TextSecondary,
    Color TextSecondaryDisabled,
    Color Primary,
    Color PrimaryDisabled,
    Color Secondary,
    Color SecondaryDisabled,
    Color Success,
    Color SuccessDisabled,
    Color Warning,
    Color WarningDisabled,
    Color FaultOrDanger,
    Color FaultOrDangerDisabled
) : IFeedbackTheme
{
    /// <summary>
    /// Gets an instance that contains colours appropriate for Discord's dark theme.
    /// </summary>
    public static IFeedbackTheme DiscordDark { get; } = new FeedbackTheme
    (
        Color.FromArgb(54, 57, 63),
        Color.FromArgb(244, 245, 245),
        Color.FromArgb(255, 255, 255),
        Color.FromArgb(94, 97, 101),
        Color.FromArgb(88, 101, 242),
        Color.FromArgb(61, 66, 99),
        Color.FromArgb(79, 84, 92),
        Color.FromArgb(59, 62, 69),
        Color.FromArgb(67, 181, 129),
        Color.FromArgb(57, 82, 76),
        Color.FromArgb(251, 185, 72),
        Color.FromArgb(91, 75, 60),
        Color.FromArgb(240, 71, 71),
        Color.FromArgb(91, 60, 65)
    );

    /// <summary>
    /// Gets an instance that contains colours appropriate for Discord's light theme.
    /// </summary>
    public static IFeedbackTheme DiscordLight { get; } = new FeedbackTheme
    (
        Color.FromArgb(255, 255, 255),
        Color.FromArgb(255, 255, 255),
        Color.FromArgb(255, 255, 255),
        Color.FromArgb(255, 255, 255),
        Color.FromArgb(88, 101, 242),
        Color.FromArgb(222, 224, 252),
        Color.FromArgb(116, 127, 141),
        Color.FromArgb(227, 229, 232),
        Color.FromArgb(67, 181, 129),
        Color.FromArgb(217, 240, 230),
        Color.FromArgb(251, 185, 72),
        Color.FromArgb(252, 240, 218),
        Color.FromArgb(240, 71, 71),
        Color.FromArgb(252, 218, 218)
    );
}
