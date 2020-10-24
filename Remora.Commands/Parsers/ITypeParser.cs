//
//  ITypeParser.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;

#pragma warning disable CS1591, SA1402

namespace Remora.Commands.Parsers
{
    /// <summary>
    /// Represents the public API of a specialized type parser.
    /// </summary>
    /// <typeparam name="TType">The type to parse.</typeparam>
    [PublicAPI]
    public interface ITypeParser<TType> where TType : notnull
    {
        /// <summary>
        /// Attempts to parse the given string into an instance of <see cref="TType"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        ValueTask<RetrieveEntityResult<TType>> TryParse(string value, CancellationToken ct);
    }

    /// <summary>
    /// Represents the internal API of a general type parser.
    /// </summary>
    internal interface ITypeParser
    {
        /// <summary>
        /// Attempts to parse the given string into a CLR object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        ValueTask<RetrieveEntityResult<object>> TryParseAsync(string value, CancellationToken ct);
    }
}
