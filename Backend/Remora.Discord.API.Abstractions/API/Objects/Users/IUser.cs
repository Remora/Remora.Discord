//
//  IUser.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
/// Represents a Discord user.
/// </summary>
[PublicAPI]
public interface IUser : IPartialUser
{
    /// <summary>
    /// Gets the ID of the user.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the username of the user. This is not a unique value.
    /// </summary>
    new string Username { get; }

    /// <summary>
    /// Gets the user's 4-digit discord tag.
    /// </summary>
    new ushort Discriminator { get; }

    /// <summary>
    /// Gets the user's avatar hash.
    /// </summary>
    new IImageHash? Avatar { get; }

    /// <summary>
    /// Gets a value indicating whether the user is a bot, belonging to an OAuth2 application.
    /// </summary>
    new Optional<bool> IsBot { get; }

    /// <summary>
    /// Gets a value indicating whether the user is an official Discord system user (part of the urgent message
    /// system).
    /// </summary>
    new Optional<bool> IsSystem { get; }

    /// <summary>
    /// Gets a value indicating whether the user has multi-factor authentication enabled on their account.
    /// </summary>
    new Optional<bool> IsMFAEnabled { get; }

    /// <summary>
    /// Gets the user's banner.
    /// </summary>
    new Optional<IImageHash?> Banner { get; }

    /// <summary>
    /// Gets the user's banner colour.
    /// </summary>
    new Optional<Color?> AccentColour { get; }

    /// <summary>
    /// Gets the user's chosen language option.
    /// </summary>
    new Optional<string> Locale { get; }

    /// <summary>
    /// Gets a value indicating whether the email on the account has been verified.
    /// </summary>
    new Optional<bool> IsVerified { get; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    new Optional<string?> Email { get; }

    /// <summary>
    /// Gets the flags on the user's account.
    /// </summary>
    new Optional<UserFlags> Flags { get; }

    /// <summary>
    /// Gets the user's premium status.
    /// </summary>
    new Optional<PremiumType> PremiumType { get; }

    /// <summary>
    /// Gets the flags on a user's account.
    /// </summary>
    new Optional<UserFlags> PublicFlags { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialUser.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialUser.Username => this.Username;

    /// <inheritdoc/>
    Optional<ushort> IPartialUser.Discriminator => this.Discriminator;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialUser.Avatar => new(this.Avatar);

    /// <inheritdoc/>
    Optional<bool> IPartialUser.IsBot => this.IsBot;

    /// <inheritdoc/>
    Optional<bool> IPartialUser.IsSystem => this.IsSystem;

    /// <inheritdoc/>
    Optional<bool> IPartialUser.IsMFAEnabled => this.IsMFAEnabled;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialUser.Banner => this.Banner;

    /// <inheritdoc/>
    Optional<Color?> IPartialUser.AccentColour => this.AccentColour;

    /// <inheritdoc/>
    Optional<string> IPartialUser.Locale => this.Locale;

    /// <inheritdoc/>
    Optional<bool> IPartialUser.IsVerified => this.IsVerified;

    /// <inheritdoc/>
    Optional<string?> IPartialUser.Email => this.Email;

    /// <inheritdoc/>
    Optional<UserFlags> IPartialUser.Flags => this.Flags;

    /// <inheritdoc/>
    Optional<PremiumType> IPartialUser.PremiumType => this.PremiumType;

    /// <inheritdoc/>
    Optional<UserFlags> IPartialUser.PublicFlags => this.PublicFlags;
}
