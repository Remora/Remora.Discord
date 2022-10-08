//
//  IPartialUser.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a partial Discord user.
/// </summary>
[PublicAPI]
public interface IPartialUser
{
    /// <inheritdoc cref="IUser.ID" />
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IUser.Username" />
    Optional<string> Username { get; }

    /// <inheritdoc cref="IUser.Discriminator" />
    Optional<ushort> Discriminator { get; }

    /// <inheritdoc cref="IUser.Avatar" />
    Optional<IImageHash?> Avatar { get; }

    /// <inheritdoc cref="IUser.IsBot" />
    Optional<bool> IsBot { get; }

    /// <inheritdoc cref="IUser.IsSystem" />
    Optional<bool> IsSystem { get; }

    /// <inheritdoc cref="IUser.IsMFAEnabled" />
    Optional<bool> IsMFAEnabled { get; }

    /// <inheritdoc cref="IUser.Banner" />
    Optional<IImageHash?> Banner { get; }

    /// <inheritdoc cref="IUser.AccentColour" />
    Optional<Color?> AccentColour { get; }

    /// <inheritdoc cref="IUser.Locale" />
    Optional<string> Locale { get; }

    /// <inheritdoc cref="IUser.IsVerified" />
    Optional<bool> IsVerified { get; }

    /// <inheritdoc cref="IUser.Email" />
    Optional<string?> Email { get; }

    /// <inheritdoc cref="IUser.Flags" />
    Optional<UserFlags> Flags { get; }

    /// <inheritdoc cref="IUser.PremiumType" />
    Optional<PremiumType> PremiumType { get; }

    /// <inheritdoc cref="IUser.PublicFlags" />
    Optional<UserFlags> PublicFlags { get; }
}
