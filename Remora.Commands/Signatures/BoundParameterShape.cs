//
//  BoundParameterShape.cs
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
using Remora.Commands.Tokenization;

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents a parameter shape that has been bound to a sequence of matching tokens.
    /// </summary>
    public class BoundParameterShape
    {
        /// <summary>
        /// Gets the parameter shape.
        /// </summary>
        public IParameterShape ParameterShape { get; }

        /// <summary>
        /// Gets the tokens bound to the parameter.
        /// </summary>
        public IReadOnlyList<(TokenType Type, string Value)> Tokens { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundParameterShape"/> class.
        /// </summary>
        /// <param name="parameterShape">The parameter shape.</param>
        /// <param name="tokens">The bound tokens.</param>
        public BoundParameterShape(IParameterShape parameterShape, IReadOnlyList<(TokenType Type, string Value)> tokens)
        {
            this.ParameterShape = parameterShape;
            this.Tokens = tokens;
        }
    }
}
