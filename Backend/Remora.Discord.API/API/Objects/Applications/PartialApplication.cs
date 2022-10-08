//
//  PartialApplication.cs
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
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IApplication" />
[PublicAPI]
public record PartialApplication
(
    Optional<Snowflake> ID = default,
    Optional<string> Name = default,
    Optional<IImageHash?> Icon = default,
    Optional<string> Description = default,
    Optional<IReadOnlyList<string>> RPCOrigins = default,
    Optional<bool> IsBotPublic = default,
    Optional<bool> DoesBotRequireCodeGrant = default,
    Optional<string> TermsOfServiceURL = default,
    Optional<string> PrivacyPolicyURL = default,
    Optional<IPartialUser?> Owner = default,
    Optional<string> VerifyKey = default,
    Optional<ITeam?> Team = default,
    Optional<Snowflake> GuildID = default,
    Optional<Snowflake> PrimarySKUID = default,
    Optional<string> Slug = default,
    Optional<IImageHash> CoverImage = default,
    Optional<ApplicationFlags> Flags = default,
    Optional<IReadOnlyList<string>> Tags = default,
    Optional<IApplicationInstallParameters> InstallParams = default,
    Optional<Uri> CustomInstallUrl = default
) : IPartialApplication;
