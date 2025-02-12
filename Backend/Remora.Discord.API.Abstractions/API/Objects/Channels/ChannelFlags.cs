//
//  ChannelFlags.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various channel flags.
/// </summary>
[PublicAPI, Flags]
public enum ChannelFlags
{
    /// <summary>
    /// The thread is pinned to the top of its parent forum channel.
    /// </summary>
    Pinned = 1 << 1,

    /// <summary>
    /// The forum requires a tag to be specified when creating a thread.
    /// </summary>
    RequireTag = 1 << 4,

    /// <summary>
    /// Hides the embedded media download options. Available only for <see cref="ChannelType.GuildMedia"/>.
    /// </summary>
    HideMediaDownloadOptions = 1 << 15
}
