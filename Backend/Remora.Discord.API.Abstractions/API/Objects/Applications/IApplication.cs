//
//  IApplication.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about an OAuth2 application.
/// </summary>
[PublicAPI]
public interface IApplication : IPartialApplication
{
    /// <summary>
    /// Gets the application ID.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    new string Name { get; }

    /// <summary>
    /// Gets the icon hash of the application.
    /// </summary>
    new IImageHash? Icon { get; }

    /// <summary>
    /// Gets the description of the application.
    /// </summary>
    new string Description { get; }

    /// <summary>
    /// Gets a list of RPC origin URLs.
    /// </summary>
    new Optional<IReadOnlyList<string>> RPCOrigins { get; }

    /// <summary>
    /// Gets a value indicating whether the bot is a public bot.
    /// </summary>
    new bool IsBotPublic { get; }

    /// <summary>
    /// Gets a value indicating whether the bot will only join upon completion of a full OAuth2 flow.
    /// </summary>
    new bool DoesBotRequireCodeGrant { get; }

    /// <summary>
    /// Gets the URL to the application's terms of service.
    /// </summary>
    new Optional<string> TermsOfServiceURL { get; }

    /// <summary>
    /// Gets the URL to the application's privacy policy.
    /// </summary>
    new Optional<string> PrivacyPolicyURL { get; }

    /// <summary>
    /// Gets the user information of the application owner.
    /// </summary>
    new IPartialUser? Owner { get; }

    /// <summary>
    /// Gets the hex-encoded key for GameSDK's GetTicket function.
    /// </summary>
    new string VerifyKey { get; }

    /// <summary>
    /// Gets the team the application belongs to, if any.
    /// </summary>
    new ITeam? Team { get; }

    /// <summary>
    /// Gets the guild the game is linked to, if the application is a game sold on the Discord storefront.
    /// </summary>
    new Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the primary SKU ID of the game, if the application is a game sold on the Discord storefront.
    /// </summary>
    new Optional<Snowflake> PrimarySKUID { get; }

    /// <summary>
    /// Gets the URL slug that links to the store page, if the application is a game sold on the Discord storefront.
    /// </summary>
    new Optional<string> Slug { get; }

    /// <summary>
    /// Gets the cover image, if the application is a game sold on the Discord storefront.
    /// </summary>
    new Optional<IImageHash> CoverImage { get; }

    /// <summary>
    /// Gets the application's public flags.
    /// </summary>
    new Optional<ApplicationFlags> Flags { get; }

    /// <summary>
    /// Gets up to 5 tags describing the content and functionality of the application.
    /// </summary>
    new Optional<IReadOnlyList<string>> Tags { get; }

    /// <summary>
    /// Gets the settings for the application's default in-app authorization link.
    /// </summary>
    new Optional<IApplicationInstallParameters> InstallParams { get; }

    /// <summary>
    /// Gets the application's default custom authorization link.
    /// </summary>
    new Optional<Uri> CustomInstallUrl { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialApplication.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialApplication.Name => this.Name;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialApplication.Icon => new(this.Icon);

    /// <inheritdoc/>
    Optional<string> IPartialApplication.Description => this.Description;

    /// <inheritdoc/>
    Optional<IReadOnlyList<string>> IPartialApplication.RPCOrigins => this.RPCOrigins;

    /// <inheritdoc/>
    Optional<bool> IPartialApplication.IsBotPublic => this.IsBotPublic;

    /// <inheritdoc/>
    Optional<bool> IPartialApplication.DoesBotRequireCodeGrant => this.DoesBotRequireCodeGrant;

    /// <inheritdoc/>
    Optional<string> IPartialApplication.TermsOfServiceURL => this.TermsOfServiceURL;

    /// <inheritdoc/>
    Optional<string> IPartialApplication.PrivacyPolicyURL => this.PrivacyPolicyURL;

    /// <inheritdoc/>
    Optional<IPartialUser?> IPartialApplication.Owner => new(this.Owner);

    /// <inheritdoc/>
    Optional<string> IPartialApplication.VerifyKey => this.VerifyKey;

    /// <inheritdoc/>
    Optional<ITeam?> IPartialApplication.Team => new(this.Team);

    /// <inheritdoc/>
    Optional<Snowflake> IPartialApplication.GuildID => this.GuildID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialApplication.PrimarySKUID => this.PrimarySKUID;

    /// <inheritdoc/>
    Optional<string> IPartialApplication.Slug => this.Slug;

    /// <inheritdoc/>
    Optional<IImageHash> IPartialApplication.CoverImage => this.CoverImage;

    /// <inheritdoc/>
    Optional<ApplicationFlags> IPartialApplication.Flags => this.Flags;

    /// <inheritdoc/>
    Optional<IReadOnlyList<string>> IPartialApplication.Tags => throw new NotImplementedException();

    /// <inheritdoc/>
    Optional<IApplicationInstallParameters> IPartialApplication.InstallParams => throw new NotImplementedException();

    /// <inheritdoc/>
    Optional<Uri> IPartialApplication.CustomInstallUrl => throw new NotImplementedException();
}
