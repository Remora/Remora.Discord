//
//  DiscordInstallContextAttribute.cs
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

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Specifies that the command is valid to install in the given contexts.
/// </summary>
/// <param name="installTypes">The contexts that the command can be installed into.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DiscordInstallContextAttribute(params ApplicationIntegrationType[] installTypes) : Attribute
{
    /// <summary>
    /// Gets a value specifying the contexts that the command can be installed into.
    /// </summary>
    public IReadOnlyList<ApplicationIntegrationType> InstallTypes { get; } = installTypes;
}
