//
//  IBuilder.cs
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
using Remora.Results;

namespace Remora.Discord.Extensions.Builder;

/// <summary>
/// Represents an object responsible for constructing and validating a model.
/// </summary>
/// <typeparam name="TEntity">The type of model to build.</typeparam>
[PublicAPI]
public interface IBuilder<TEntity>
{
    /// <summary>
    /// Validate the model within specifications described by the model.
    /// </summary>
    /// <returns>Returns a <see cref="Result"/> indicating the result of validation.</returns>
    Result Validate();

    /// <summary>
    /// Validates and then builds the model.
    /// </summary>
    /// <returns>Returns a <see cref="Result{TEntity}"/> containing the result of the build or the reason for failure.</returns>
    Result<TEntity> Build();
}
