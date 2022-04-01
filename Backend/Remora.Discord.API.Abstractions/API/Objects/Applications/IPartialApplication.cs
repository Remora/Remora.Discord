//
//  IPartialApplication.cs
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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about an OAuth2 application.
/// </summary>
[PublicAPI]
public interface IPartialApplication
{
    /// <inheritdoc cref="IApplication.ID" />
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IApplication.Name" />
    Optional<string> Name { get; }

    /// <inheritdoc cref="IApplication.Icon" />
    Optional<IImageHash?> Icon { get; }

    /// <inheritdoc cref="IApplication.Description" />
    Optional<string> Description { get; }

    /// <inheritdoc cref="IApplication.RPCOrigins" />
    Optional<IReadOnlyList<string>> RPCOrigins { get; }

    /// <inheritdoc cref="IApplication.IsBotPublic" />
    Optional<bool> IsBotPublic { get; }

    /// <inheritdoc cref="IApplication.DoesBotRequireCodeGrant" />
    Optional<bool> DoesBotRequireCodeGrant { get; }

    /// <inheritdoc cref="IApplication.TermsOfServiceURL" />
    Optional<string> TermsOfServiceURL { get; }

    /// <inheritdoc cref="IApplication.PrivacyPolicyURL" />
    Optional<string> PrivacyPolicyURL { get; }

    /// <inheritdoc cref="IApplication.Owner" />
    Optional<IPartialUser?> Owner { get; }

    /// <inheritdoc cref="IApplication.VerifyKey" />
    Optional<string> VerifyKey { get; }

    /// <inheritdoc cref="IApplication.Team" />
    Optional<ITeam?> Team { get; }

    /// <inheritdoc cref="IApplication.GuildID" />
    Optional<Snowflake> GuildID { get; }

    /// <inheritdoc cref="IApplication.PrimarySKUID" />
    Optional<Snowflake> PrimarySKUID { get; }

    /// <inheritdoc cref="IApplication.Slug" />
    Optional<string> Slug { get; }

    /// <inheritdoc cref="IApplication.CoverImage" />
    Optional<IImageHash> CoverImage { get; }

    /// <inheritdoc cref="IApplication.Flags" />
    Optional<ApplicationFlags> Flags { get; }

    /// <inheritdoc cref="IApplication.Tags" />
    Optional<IReadOnlyList<string>> Tags { get; }

    /// <inheritdoc cref="IApplication.InstallParams" />
    Optional<IApplicationInstallParameters> InstallParams { get; }

    /// <inheritdoc cref="IApplication.CustomInstallUrl" />
    Optional<Uri> CustomInstallUrl { get; }
}
