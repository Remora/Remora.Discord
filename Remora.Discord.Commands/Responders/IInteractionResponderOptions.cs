//
//  IInteractionResponderOptions.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;

namespace Remora.Discord.Commands.Responders;

/// <summary>
/// Represents a read-only view of a <see cref="InteractionResponderOptions"/> object.
/// </summary>
[PublicAPI]
public interface IInteractionResponderOptions
{
    /// <summary>
    /// Gets a value indicating whether <see cref="InteractionResponder"/> should automatically issue a
    /// <see cref="InteractionCallbackType.DeferredChannelMessageWithSource"/> response to interactions, before
    /// attempting to identify and invoke the command, or whether all interaction responses should be handled by the
    /// consumer.
    /// </summary>
    public bool SuppressAutomaticResponses { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="InteractionResponder"/> should automatically respond to
    /// interactions with the <see cref="MessageFlags.Ephemeral"/> flag.
    /// Ephemeral responses can still be explicitly disabled for a given command/group through use of
    /// <see cref="EphemeralAttribute"/>.
    /// </summary>
    public bool UseEphemeralResponses { get; }
}
