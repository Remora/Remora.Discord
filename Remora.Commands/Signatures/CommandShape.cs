//
//  CommandShape.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remora.Commands.Attributes;

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents the general "shape" of a command. This type is used to determine whether a sequence of tokens could
    /// fit the associated command, provided all other things hold true.
    /// </summary>
    public class CommandShape
    {
        /// <summary>
        /// Gets the parameters for this command shape.
        /// </summary>
        public IReadOnlyList<IParameterShape> Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandShape"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public CommandShape(IReadOnlyList<IParameterShape> parameters)
        {
            this.Parameters = parameters;
        }

        /// <summary>
        /// Creates a new <see cref="CommandShape"/> from the given method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The command shape.</returns>
        public static CommandShape FromMethod(MethodInfo method)
        {
            var positionalParameters = new List<IParameterShape>();
            var namedParameters = new List<IParameterShape>();

            foreach (var parameter in method.GetParameters())
            {
                var optionAttribute = parameter.GetCustomAttribute<OptionAttribute>();
                var rangeAttribute = parameter.GetCustomAttribute<RangeAttribute>();

                if (optionAttribute is null)
                {
                    var newPositionalParameter = CreatePositionalParameterShape
                    (
                        rangeAttribute,
                        parameter
                    );

                    positionalParameters.Add(newPositionalParameter);
                }
                else
                {
                    var newNamedParameter = CreateNamedParameterShape
                    (
                        optionAttribute,
                        rangeAttribute,
                        parameter
                    );

                    namedParameters.Add(newNamedParameter);
                }
            }

            return new CommandShape(positionalParameters.Concat(namedParameters).ToList());
        }

        private static IParameterShape CreateNamedParameterShape
        (
            OptionAttribute optionAttribute,
            RangeAttribute? rangeAttribute,
            ParameterInfo parameter
        )
        {
            var isCollection = typeof(IEnumerable).IsAssignableFrom(parameter.ParameterType);

            IParameterShape newNamedParameter;
            if (rangeAttribute is null || !isCollection)
            {
                newNamedParameter = CreateNamedSingleValueParameterShape(optionAttribute, parameter);
            }
            else if (parameter.ParameterType == typeof(bool))
            {
                newNamedParameter = CreateNamedSwitchParameterShape(optionAttribute, parameter);
            }
            else
            {
                newNamedParameter = CreateNamedCollectionParameterShape(optionAttribute, rangeAttribute, parameter);
            }

            return newNamedParameter;
        }

        private static IParameterShape CreateNamedCollectionParameterShape
        (
            OptionAttribute optionAttribute,
            RangeAttribute rangeAttribute,
            ParameterInfo parameter
        )
        {
            IParameterShape newNamedParameter;
            if (optionAttribute.ShortName is null)
            {
                newNamedParameter = new NamedCollectionParameterShape
                (
                    parameter,
                    optionAttribute.LongName ?? throw new InvalidOperationException(),
                    rangeAttribute.Min,
                    rangeAttribute.Max
                );
            }
            else if (optionAttribute.LongName is null)
            {
                newNamedParameter = new NamedCollectionParameterShape
                (
                    parameter,
                    optionAttribute.ShortName ?? throw new InvalidOperationException(),
                    rangeAttribute.Min,
                    rangeAttribute.Max
                );
            }
            else
            {
                newNamedParameter = new NamedCollectionParameterShape
                (
                    parameter,
                    optionAttribute.ShortName ?? throw new InvalidOperationException(),
                    optionAttribute.LongName ?? throw new InvalidOperationException(),
                    rangeAttribute.Min,
                    rangeAttribute.Max
                );
            }

            return newNamedParameter;
        }

        private static IParameterShape CreateNamedSwitchParameterShape
        (
            OptionAttribute optionAttribute,
            ParameterInfo parameter
        )
        {
            if (!parameter.IsOptional)
            {
                throw new InvalidOperationException("Switches must have a default value.");
            }

            IParameterShape newNamedParameter;
            if (optionAttribute.ShortName is null)
            {
                newNamedParameter = new SwitchParameterShape
                (
                    parameter,
                    optionAttribute.LongName ?? throw new InvalidOperationException()
                );
            }
            else if (optionAttribute.LongName is null)
            {
                newNamedParameter = new SwitchParameterShape
                (
                    parameter,
                    optionAttribute.ShortName ?? throw new InvalidOperationException()
                );
            }
            else
            {
                newNamedParameter = new SwitchParameterShape
                (
                    parameter,
                    optionAttribute.ShortName ?? throw new InvalidOperationException(),
                    optionAttribute.LongName ?? throw new InvalidOperationException()
                );
            }

            return newNamedParameter;
        }

        private static IParameterShape CreateNamedSingleValueParameterShape
        (
            OptionAttribute optionAttribute,
            ParameterInfo parameter
        )
        {
            IParameterShape newNamedParameter;
            if (optionAttribute.ShortName is null)
            {
                newNamedParameter = new NamedParameterShape
                (
                    parameter,
                    optionAttribute.LongName ?? throw new InvalidOperationException()
                );
            }
            else if (optionAttribute.LongName is null)
            {
                newNamedParameter = new NamedParameterShape
                (
                    parameter,
                    optionAttribute.ShortName ?? throw new InvalidOperationException()
                );
            }
            else
            {
                newNamedParameter = new NamedParameterShape
                (
                    parameter,
                    optionAttribute.ShortName ?? throw new InvalidOperationException(),
                    optionAttribute.LongName ?? throw new InvalidOperationException()
                );
            }

            return newNamedParameter;
        }

        private static IParameterShape CreatePositionalParameterShape
        (
            RangeAttribute? rangeAttribute,
            ParameterInfo parameter
        )
        {
            var isCollection = typeof(IEnumerable).IsAssignableFrom(parameter.ParameterType);

            IParameterShape newPositionalParameter;
            if (rangeAttribute is null || !isCollection)
            {
                newPositionalParameter = new PositionalParameterShape(parameter);
            }
            else
            {
                newPositionalParameter = new PositionalCollectionParameterShape
                (
                    parameter,
                    rangeAttribute.Min,
                    rangeAttribute.Max
                );
            }

            return newPositionalParameter;
        }
    }
}
