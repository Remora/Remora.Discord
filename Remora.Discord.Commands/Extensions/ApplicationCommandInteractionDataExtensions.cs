//
//  ApplicationCommandInteractionDataExtensions.cs
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
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Extensions
{
    /// <summary>
    /// Defines extensions for the <see cref="IApplicationCommandInteractionData"/> interface.
    /// </summary>
    public static class ApplicationCommandInteractionDataExtensions
    {
        /// <summary>
        /// Unpacks an interaction into a command name string and a set of parameters.
        /// </summary>
        /// <param name="commandData">The interaction to unpack.</param>
        /// <param name="commandName">The command name.</param>
        /// <param name="parameters">The parameters supplied to the command.</param>
        public static void UnpackInteraction
        (
            this IApplicationCommandInteractionData commandData,
            out string commandName,
            out IReadOnlyDictionary<string, IReadOnlyList<string>> parameters
        )
        {
            if (!commandData.Options.HasValue)
            {
                commandName = commandData.Name;
                parameters = new Dictionary<string, IReadOnlyList<string>>();

                return;
            }

            UnpackInteractionOptions(commandData.Options.Value!, out var nestedCommandName, out var nestedParameters);

            commandName = nestedCommandName is not null
                ? $"{commandData.Name} {nestedCommandName}"
                : commandData.Name;

            parameters = nestedParameters ?? new Dictionary<string, IReadOnlyList<string>>();
        }

        private static void UnpackInteractionOptions
        (
            IReadOnlyCollection<IApplicationCommandInteractionDataOption> options,
            out string? commandName,
            out IReadOnlyDictionary<string, IReadOnlyList<string>>? parameters
        )
        {
            commandName = null;
            parameters = null;

            if (options.Count > 1)
            {
                // multiple parameters
                var unpackedParameters = new Dictionary<string, IReadOnlyList<string>>();
                foreach (var option in options)
                {
                    var (name, values) = UnpackInteractionParameter(option);
                    unpackedParameters.Add(name, values);
                }

                parameters = unpackedParameters;
                return;
            }

            var singleOption = options.SingleOrDefault();
            if (singleOption is null)
            {
                return;
            }

            if (singleOption.Value.HasValue)
            {
                // A single parameter
                var (name, values) = UnpackInteractionParameter(singleOption);
                parameters = new Dictionary<string, IReadOnlyList<string>>
                {
                    { name, values }
                };
            }
            else if (singleOption.Options.HasValue)
            {
                // A nested group
                var nestedOptions = singleOption.Options.Value!;

                UnpackInteractionOptions(nestedOptions, out var nestedCommandName, out parameters);

                commandName = nestedCommandName is not null
                    ? $"{singleOption.Name} {nestedCommandName}"
                    : singleOption.Name;
            }
            else
            {
                // A parameterless command
                commandName = singleOption.Name;
            }
        }

        private static (string Name, IReadOnlyList<string> Values) UnpackInteractionParameter
        (
            IApplicationCommandInteractionDataOption option
        )
        {
            if (!option.Value.HasValue)
            {
                throw new InvalidOperationException();
            }

            var values = new List<string>();
            var optionValue = option.Value.Value!;
            if (optionValue.Value is ICollection collection)
            {
                values.AddRange(collection.Cast<object>().Select(o => o.ToString()));
            }
            else
            {
                values.Add(optionValue.Value.ToString());
            }

            return (option.Name, values);
        }
    }
}
