//
//  PositionalParameterShape.cs
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
using System.Reflection;
using Remora.Commands.Tokenization;
using static Remora.Commands.Tokenization.TokenType;

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents a single value without a name.
    /// </summary>
    public class PositionalParameterShape : IParameterShape
    {
        /// <inheritdoc />
        public IReadOnlyList<TokenType> ValidTypes => new[] { Value };

        /// <inheritdoc />
        public ParameterInfo Parameter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionalParameterShape"/> class.
        /// </summary>
        /// <param name="parameter">The underlying parameter.</param>
        public PositionalParameterShape(ParameterInfo parameter)
        {
            this.Parameter = parameter;
        }

        /// <inheritdoc />
        public virtual bool Matches(TokenizingEnumerator tokenizer, out ulong consumedTokens)
        {
            consumedTokens = 0;

            if (!tokenizer.MoveNext())
            {
                return false;
            }

            if (tokenizer.Current.Type != Value)
            {
                return false;
            }

            consumedTokens = 1;
            return true;
        }
    }
}
