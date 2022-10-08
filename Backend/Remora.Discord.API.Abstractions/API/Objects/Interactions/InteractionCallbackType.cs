//
//  InteractionCallbackType.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various response types.
/// </summary>
[PublicAPI]
public enum InteractionCallbackType
{
    /// <summary>
    /// Acknowledge a <see cref="InteractionType.Ping"/>.
    /// </summary>
    Pong = 1,

    /// <summary>
    /// Respond with a message, showing the user input.
    /// </summary>
    ChannelMessageWithSource = 4,

    /// <summary>
    /// Acknowledge a command without sending a message, showing the user input.
    /// </summary>
    DeferredChannelMessageWithSource = 5,

    /// <summary>
    /// Acknowledge an interaction and edit the message later; the user does not see a loading state.
    /// </summary>
    /// <remarks>
    /// Only relevant for component-based interactions.
    /// </remarks>
    DeferredUpdateMessage = 6,

    /// <summary>
    /// Respond by editing the message.
    /// </summary>
    /// <remarks>
    /// Only relevant for component-based interactions.
    /// </remarks>
    UpdateMessage = 7,

    /// <summary>
    /// Respond to an autocomplete request with suggested choices.
    /// </summary>
    ApplicationCommandAutocompleteResult = 8,

    /// <summary>
    /// Respond to an interaction with a modal.
    /// </summary>
    /// <remarks>
    /// Only relevant for component-based interactions and application commands.
    /// </remarks>
    Modal = 9
}
