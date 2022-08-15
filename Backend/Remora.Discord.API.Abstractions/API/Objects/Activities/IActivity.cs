//
//  IActivity.cs
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
using JetBrains.Annotations;
using OneOf;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about an activity Discord is aware of.
/// </summary>
[PublicAPI]
public interface IActivity
{
    /// <summary>
    /// Gets the name of the activity.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of the activity.
    /// </summary>
    ActivityType Type { get; }

    /// <summary>
    /// Gets the stream URL. Used when <see cref="Type"/> is <see cref="ActivityType.Streaming"/>.
    /// </summary>
    Optional<Uri?> Url { get; }

    /// <summary>
    /// Gets the time when the activity was added to the user's session.
    /// </summary>
    /// <remarks>
    /// This field is always present on activities received from Discord; it is optional because it should never be
    /// sent by a bot. Discord's documentation is somewhat vague at first glance, but because of the above it's
    /// implemented as an optional field.
    /// </remarks>
    Optional<DateTimeOffset> CreatedAt { get; }

    /// <summary>
    /// Gets the time at which the activity started and/or ended.
    /// </summary>
    Optional<IActivityTimestamps> Timestamps { get; }

    /// <summary>
    /// Gets the ID of the application the activity is associated with.
    /// </summary>
    Optional<Snowflake> ApplicationID { get; }

    /// <summary>
    /// Gets a detail string about what the user is currently doing.
    /// </summary>
    Optional<string?> Details { get; }

    /// <summary>
    /// Gets the user's current party status.
    /// </summary>
    Optional<string?> State { get; }

    /// <summary>
    /// Gets the emoji used for a custom status.
    /// </summary>
    Optional<IActivityEmoji?> Emoji { get; }

    /// <summary>
    /// Gets information about the user's current party.
    /// </summary>
    Optional<IActivityParty> Party { get; }

    /// <summary>
    /// Gets information about the assets associated with the user's current activity.
    /// </summary>
    Optional<IActivityAssets> Assets { get; }

    /// <summary>
    /// Gets the secrets associated with the user's current activity.
    /// </summary>
    Optional<IActivitySecrets> Secrets { get; }

    /// <summary>
    /// Gets a value indicating whether the activity is in an instanced session.
    /// </summary>
    Optional<bool> Instance { get; }

    /// <summary>
    /// Gets a set of descriptive flags that detail what the payload includes.
    /// </summary>
    Optional<ActivityFlags> Flags { get; }

    /// <summary>
    /// Gets the custom buttons show in Rich Presence.
    /// </summary>
    Optional<OneOf<IReadOnlyList<string>, IReadOnlyList<IActivityButton>>> Buttons { get; }
}
