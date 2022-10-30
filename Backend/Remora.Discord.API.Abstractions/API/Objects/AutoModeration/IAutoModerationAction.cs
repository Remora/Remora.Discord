//
//  IAutoModerationAction.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// An action which will execute whenever a rule is triggered.
/// </summary>
[PublicAPI]
public interface IAutoModerationAction
{
    /// <summary>
    /// Gets the type of the action.
    /// </summary>
    AutoModerationActionType Type { get; }

    /// <summary>
    /// Gets additional metadata needed during execution for this specific action type.
    /// </summary>
    /// <remarks>
    /// This property can be omitted based on the <see cref="Type"/>. See the associated action type in
    /// <see cref="IAutoModerationActionMetadata"/> to understand which <see cref="Type"/> values require metadata
    /// to be set.
    /// </remarks>
    Optional<IAutoModerationActionMetadata> Metadata { get; }
}
