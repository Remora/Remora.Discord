//
//  IUpdatePresence.cs
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
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Abstractions.Gateway.Commands;

/// <summary>
/// Represents a command to update the presence of a user.
/// </summary>
[PublicAPI]
public interface IUpdatePresence : IGatewayCommand
{
    /// <summary>
    /// Gets the unix time in milliseconds of when the client went idle, or null if the client is not idle.
    /// </summary>
    DateTimeOffset? Since { get; }

    /// <summary>
    /// Gets the user's new activities.
    /// </summary>
    IReadOnlyList<IActivity> Activities { get; }

    /// <summary>
    /// Gets the user's status.
    /// </summary>
    ClientStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the user is AFK.
    /// </summary>
    bool IsAFK { get; }
}
