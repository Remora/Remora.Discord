//
//  NamedCollectionParameterShape.cs
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
using System.Reflection;
using Remora.Commands.Tokenization;

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents a named parameter with a single value.
    /// </summary>
    public class NamedCollectionParameterShape : NamedParameterShape, ICollectionParameterShape
    {
        /// <inheritdoc />
        public ulong? Min { get; }

        /// <inheritdoc />
        public ulong? Max { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollectionParameterShape"/> class.
        /// </summary>
        /// <param name="parameter">The underlying parameter.</param>
        /// <param name="shortName">The short name.</param>
        /// <param name="longName">The long name.</param>
        /// <param name="min">The minimum number of items in the collection.</param>
        /// <param name="max">The maximum number of items in the collection.</param>
        public NamedCollectionParameterShape
        (
            ParameterInfo parameter,
            char shortName,
            string longName,
            ulong? min,
            ulong? max
        )
            : base(parameter, shortName, longName)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollectionParameterShape"/> class.
        /// </summary>
        /// <param name="parameter">The underlying parameter.</param>
        /// <param name="shortName">The short name.</param>
        /// <param name="min">The minimum number of items in the collection.</param>
        /// <param name="max">The maximum number of items in the collection.</param>
        public NamedCollectionParameterShape
        (
            ParameterInfo parameter,
            char shortName,
            ulong? min,
            ulong? max
        )
            : base(parameter, shortName)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollectionParameterShape"/> class.
        /// </summary>
        /// <param name="parameter">The underlying parameter.</param>
        /// <param name="longName">The long name.</param>
        /// <param name="min">The minimum number of items in the collection.</param>
        /// <param name="max">The maximum number of items in the collection.</param>
        public NamedCollectionParameterShape
        (
            ParameterInfo parameter,
            string longName,
            ulong? min,
            ulong? max
        )
            : base(parameter, longName)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <inheritdoc/>
        public override bool Matches(TokenizingEnumerator tokenizer, out ulong consumedTokens)
        {
            consumedTokens = 0;

            if (!tokenizer.MoveNext())
            {
                return false;
            }

            switch (tokenizer.Current.Type)
            {
                case TokenType.LongName:
                {
                    if (this.LongName is null)
                    {
                        return false;
                    }

                    if (!tokenizer.Current.Value.Equals(this.LongName, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    break;
                }
                case TokenType.ShortName:
                {
                    if (this.ShortName is null)
                    {
                        return false;
                    }

                    if (tokenizer.Current.Value.Length != 1)
                    {
                        return false;
                    }

                    if (tokenizer.Current.Value[0] != this.ShortName.Value)
                    {
                        return false;
                    }

                    break;
                }
                case TokenType.Value:
                {
                    return false;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            ulong itemCount = 0;
            while (this.Max is null || itemCount < this.Max.Value)
            {
                if (!tokenizer.MoveNext())
                {
                    return false;
                }

                if (tokenizer.Current.Type != TokenType.Value)
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

            consumedTokens = itemCount + 1;
            return true;
        }
    }
}
