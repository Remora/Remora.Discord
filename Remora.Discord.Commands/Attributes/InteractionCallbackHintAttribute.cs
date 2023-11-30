//
//  InteractionCallbackHintAttribute.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Represents a hint to Discord for how to handle an interaction callback.
/// </summary>
/// <param name="allowedCallbackTypes">The allowed callback types.</param>
/// <param name="ephemerality">Represents a hint as to whether the command can/will respond ephemerally.</param>
/// <param name="requiredPermissions">If <paramref name="allowedCallbackTypes"/> specifies a type that creates or updates a message, the required permissions.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class InteractionCallbackHintAttribute
(
    IReadOnlyList<InteractionCallbackType> allowedCallbackTypes,
    InteractionCallbackEphemerality? ephemerality = default,
    IReadOnlyList<DiscordPermission>? requiredPermissions = default
) : Attribute
{
    /// <summary>
    /// Gets a value specifying the allowed callback types.
    /// </summary>
    public IReadOnlyList<InteractionCallbackType> AllowedCallbackTypes { get; } = allowedCallbackTypes;

    /// <summary>
    /// Gets a value specifying the ephemeral nature of the callback, if required.
    /// </summary>
    public Optional<InteractionCallbackEphemerality> Ephemerality { get; } = ephemerality.AsOptional();

    /// <summary>
    /// Gets a value specifying the required permissions for the callback, if required.
    /// </summary>
    public Optional<IDiscordPermissionSet> RequiredPermissions { get; } = requiredPermissions
                                                                          .AsOptional()
                                                                          .Map(t => (IDiscordPermissionSet)new DiscordPermissionSet(t.ToArray()));
}
