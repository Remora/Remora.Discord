//
//  IRestError.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.API.Abstractions.Results;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an error reported by the REST API.
/// </summary>
[PublicAPI]
public interface IRestError
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    DiscordError Code { get; }

    /// <summary>
    /// Gets the per-property error details.
    /// </summary>
    Optional<IReadOnlyDictionary<string, OneOf<IPropertyErrorDetails, IReadOnlyList<IErrorDetails>>>> Errors { get; }

    /// <summary>
    /// Gets a descriptive error message.
    /// </summary>
    string Message { get; }
}
