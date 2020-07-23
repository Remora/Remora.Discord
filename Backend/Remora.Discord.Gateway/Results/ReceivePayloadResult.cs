//
//  ReceivePayloadResult.cs
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.Gateway.Results
{
    /// <summary>
    /// Represents an attempt to receive a payload via the gateway.
    /// </summary>
    /// <typeparam name="TEntity">The entity to create.</typeparam>
    public class ReceivePayloadResult<TEntity> : ResultBase<ReceivePayloadResult<TEntity>>
    {
        /// <summary>
        /// Holds the actual entity value.
        /// </summary>
        [MaybeNull, AllowNull]
        private readonly TEntity _entity = default!;

        /// <summary>
        /// Gets the received payload.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result did not contain a payload.</exception>
        public TEntity Entity
        {
            get
            {
                if (!this.IsSuccess || _entity is null)
                {
                    throw new InvalidOperationException("The result does not contain a valid value.");
                }

                return _entity;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivePayloadResult{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private ReceivePayloadResult(TEntity entity)
        {
            _entity = entity;
        }

        /// <inheritdoc cref="ReceivePayloadResult{TResultType}"/>
        [UsedImplicitly]
        private ReceivePayloadResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="entity">The entity that was retrieved.</param>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure]
        public static ReceivePayloadResult<TEntity> FromSuccess(TEntity entity)
        {
            return new ReceivePayloadResult<TEntity>(entity);
        }

        /// <summary>
        /// Implicitly converts a compatible value to a successful result.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The successful result.</returns>
        [PublicAPI, Pure]
        public static implicit operator ReceivePayloadResult<TEntity>(TEntity entity)
        {
            return FromSuccess(entity);
        }
    }
}
