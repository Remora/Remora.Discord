//
//  DeleteRestEntityResult.cs
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
using System.Net;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Results;

// ReSharper disable SA1402
#pragma warning disable SA1402

namespace Remora.Discord.Rest.Results
{
    /// <summary>
    /// Represents an attempt to create an entity via the REST API.
    /// </summary>
    public class DeleteRestEntityResult :
        AbstractRestResult<DeleteRestEntityResult>,
        IDeleteRestEntityResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRestEntityResult"/> class.
        /// </summary>
        private DeleteRestEntityResult()
        {
        }

        /// <inheritdoc cref="DeleteRestEntityResult"/>
        [UsedImplicitly]
        private DeleteRestEntityResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <inheritdoc cref="DeleteRestEntityResult"/>
        [UsedImplicitly]
        private DeleteRestEntityResult
        (
            string? errorReason,
            IRestError? discordError = null
        )
            : base(errorReason, discordError)
        {
        }

        /// <inheritdoc cref="DeleteRestEntityResult"/>
        [UsedImplicitly]
        private DeleteRestEntityResult
        (
            string? errorReason,
            HttpStatusCode? statusCode = null
        )
            : base(errorReason, statusCode)
        {
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure]
        public static DeleteRestEntityResult FromSuccess()
        {
            return new DeleteRestEntityResult();
        }
    }

    /// <summary>
    /// Represents an attempt to delete an entity via the REST API.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to Delete.</typeparam>
    public class DeleteRestEntityResult<TEntity> :
        AbstractRestResult<DeleteRestEntityResult<TEntity>>,
        IDeleteRestEntityResult<TEntity>
    {
        /// <summary>
        /// Holds the actual entity value.
        /// </summary>
        [MaybeNull, AllowNull]
        private readonly TEntity _entity = default!;

        /// <inheritdoc />
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
        /// Initializes a new instance of the <see cref="DeleteRestEntityResult{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private DeleteRestEntityResult(TEntity entity)
        {
            _entity = entity;
        }

        /// <inheritdoc cref="DeleteRestEntityResult{TResultType}"/>
        [UsedImplicitly]
        private DeleteRestEntityResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <inheritdoc cref="DeleteRestEntityResult{TResultType}"/>
        [UsedImplicitly]
        private DeleteRestEntityResult
        (
            string? errorReason,
            IRestError? discordError = null
        )
            : base(errorReason, discordError)
        {
        }

        /// <inheritdoc cref="DeleteRestEntityResult{TResultType}"/>
        [UsedImplicitly]
        private DeleteRestEntityResult
        (
            string? errorReason,
            HttpStatusCode? statusCode = null
        )
            : base(errorReason, statusCode)
        {
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure]
        public static DeleteRestEntityResult<TEntity> FromSuccess(TEntity entity)
        {
            return new DeleteRestEntityResult<TEntity>(entity);
        }

        /// <summary>
        /// Implicitly converts a compatible value to a successful result.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <returns>The successful result.</returns>
        [PublicAPI, Pure]
        public static implicit operator DeleteRestEntityResult<TEntity>(TEntity entity)
        {
            return FromSuccess(entity);
        }
    }
}
