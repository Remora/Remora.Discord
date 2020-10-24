//
//  PositionalCollectionParameterShape.cs
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

using System.Reflection;
using Remora.Commands.Tokenization;
using static Remora.Commands.Tokenization.TokenType;

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents a single value without a name.
    /// </summary>
    public class PositionalCollectionParameterShape : PositionalParameterShape, ICollectionParameterShape
    {
        /// <inheritdoc />
        public ulong? Min { get; }

        /// <inheritdoc />
        public ulong? Max { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionalCollectionParameterShape"/> class.
        /// </summary>
        /// <param name="parameter">The underlying parameter.</param>
        /// <param name="min">The minimum number of elements.</param>
        /// <param name="max">The maximum number of elements.</param>
        public PositionalCollectionParameterShape(ParameterInfo parameter, ulong? min, ulong? max)
            : base(parameter)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <inheritdoc />
        public override bool Matches(TokenizingEnumerator tokenizer, out ulong consumedTokens)
        {
            consumedTokens = 0;

            ulong itemCount = 0;
            while (this.Max is null || itemCount < this.Max.Value)
            {
                if (!tokenizer.MoveNext())
                {
                    return false;
                }

                if (tokenizer.Current.Type != Value)
                {
                    break;
                }

                ++itemCount;
            }

            if (!(this.Min is null))
            {
                if (itemCount < this.Min.Value)
                {
                    return false;
                }
            }

            consumedTokens = itemCount;
            return true;
        }
    }
}
