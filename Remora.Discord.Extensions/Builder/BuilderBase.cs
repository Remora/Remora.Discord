//
//  BuilderBase.cs
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Remora.Discord.Extensions.Errors;
using Remora.Results;

namespace Remora.Discord.Extensions.Builder
{
    /// <inheritdoc />
    public abstract class BuilderBase<TEntity> : IBuilder<TEntity>
    {
        /// <inheritdoc />
        public abstract Result<TEntity> Build();

        /// <inheritdoc />
        public Result Validate()
        {
            var context = new ValidationContext(this);
            var errorResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(this, context, errorResults, true))
            {
                return Result.FromSuccess();
            }
            else
            {
                var errors = new List<IResult>();
                foreach (var errorResult in errorResults)
                {
                    foreach (var member in errorResult.MemberNames)
                    {
                        errors.Add(Result.FromError(new ValidationError(member, errorResult.ErrorMessage ?? string.Empty)));
                    }
                }

                // If there's only one error, return that error directly.
                if (errors.Count == 1)
                {
                    return (Result)errors[0];
                }

                return new AggregateError(errors.AsReadOnly());
            }
        }
    }
}
