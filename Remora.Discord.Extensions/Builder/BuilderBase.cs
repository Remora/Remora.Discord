//
//  BuilderBase.cs
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
using Remora.Discord.Extensions.Errors;
using Remora.Results;

namespace Remora.Discord.Extensions.Builder;

/// <inheritdoc />
public abstract class BuilderBase<TEntity> : IBuilder<TEntity>
{
    /// <inheritdoc />
    public abstract Result<TEntity> Build();

    /// <inheritdoc />
    public abstract Result Validate();

    /// <summary>
    /// Validates a URL to ensure it is a valid URL.
    /// </summary>
    /// <param name="propertyName">The name of the property you are testing.</param>
    /// <param name="url">The text of the url.</param>
    /// <param name="allowNull">If true, a null url will return a successful result.</param>
    /// <returns>Returns a successful result if the url is valid; otherwise, a failed result.</returns>
    internal static Result ValidateUrl(string propertyName, string? url, bool allowNull)
    {
        if (url is null)
        {
            return allowNull
                ? Result.FromSuccess()
                : new ValidationError(propertyName, "The provided url is null but null values are not allowed.");
        }

        if (url.Length == 0)
        {
            return new ValidationError(propertyName, $"The {propertyName} cannot be an empty string.");
        }

        if
        (
            Uri.IsWellFormedUriString(url, UriKind.Absolute) &&
            Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            uri is { Scheme: "http" or "https" or "attachment" }
        )
        {
            return Result.FromSuccess();
        }

        return new ValidationError(propertyName, "Url is not in a valid format.");
    }

    /// <summary>
    /// Ensures that the length of the provided text is valid.
    /// </summary>
    /// <param name="propertyName">The name of the property you are testing.</param>
    /// <param name="text">The text.</param>
    /// <param name="upperBound">The maximum length of the value.</param>
    /// <param name="allowNull">If true, a null field will return a successful result.</param>
    /// <returns>Returns a successful result if the text is valid; otherwise, a failed result.</returns>
    internal static Result ValidateLength(string propertyName, string? text, int upperBound, bool allowNull)
    {
        if (text is null)
        {
            return allowNull
                ? Result.FromSuccess()
                : new ValidationError(propertyName, "The provided text is null but null values are not allowed.");
        }

        if (text.Length == 0)
        {
            return new ValidationError(propertyName, $"The {propertyName} cannot be an empty string.");
        }

        return text.Length > upperBound
            ? new ValidationError(propertyName, $"The {propertyName} is too long. Expected: shorter than {upperBound}. Actual: {text.Length}")
            : Result.FromSuccess();
    }
}
