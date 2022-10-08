//
//  ApplicationCommandDataExtensions.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extensions for the <see cref="IApplicationCommandData"/> interface.
/// </summary>
[PublicAPI]
public static class ApplicationCommandDataExtensions
{
    /// <summary>
    /// Unpacks an interaction into a command name string and a set of parameters.
    /// </summary>
    /// <param name="commandData">The interaction to unpack.</param>
    /// <param name="commandPath">
    /// The command path, that is, the sequential components of the full command name.
    /// </param>
    /// <param name="parameters">The parameters supplied to the command.</param>
    public static void UnpackInteraction
    (
        this IApplicationCommandData commandData,
        out IReadOnlyList<string> commandPath,
        out IReadOnlyDictionary<string, IReadOnlyList<string>> parameters
    )
    {
        if (commandData.Type is ApplicationCommandType.User or ApplicationCommandType.Message)
        {
            var parameterName = commandData.Type.AsParameterName();

            commandPath = new[] { commandData.Name };

            if (commandData.TargetID.IsDefined(out var targetId))
            {
                parameters = new Dictionary<string, IReadOnlyList<string>>
                {
                    { parameterName, new[] { targetId.ToString() } }
                };
            }
            else
            {
                parameters = new Dictionary<string, IReadOnlyList<string>>();
            }

            return;
        }

        if (!commandData.Options.IsDefined(out var options))
        {
            commandPath = new[] { commandData.Name };
            parameters = new Dictionary<string, IReadOnlyList<string>>();

            return;
        }

        UnpackInteractionOptions(options, out var nestedCommandPath, out var nestedParameters);

        var path = new List<string> { commandData.Name };
        path.AddRange(nestedCommandPath);

        commandPath = path;
        parameters = nestedParameters ?? new Dictionary<string, IReadOnlyList<string>>();
    }

    private static void UnpackInteractionOptions
    (
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> options,
        out IReadOnlyList<string> commandPath,
        out IReadOnlyDictionary<string, IReadOnlyList<string>>? parameters
    )
    {
        commandPath = Array.Empty<string>();
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
        else if (singleOption.Options.IsDefined(out var nestedOptions))
        {
            // A nested group
            UnpackInteractionOptions(nestedOptions, out var nestedCommandPath, out parameters);

            var path = new List<string> { singleOption.Name };
            path.AddRange(nestedCommandPath);

            commandPath = path;
        }
        else
        {
            // A parameterless command
            commandPath = new[] { singleOption.Name };
        }
    }

    private static (string Name, IReadOnlyList<string> Values) UnpackInteractionParameter
    (
        IApplicationCommandInteractionDataOption option
    )
    {
        if (!option.Value.IsDefined(out var optionValue))
        {
            throw new InvalidOperationException();
        }

        var values = new List<string>();
        if (optionValue.Value is ICollection collection)
        {
            values.AddRange(collection.Cast<object>().Select(o => o.ToString() ?? string.Empty));
        }
        else
        {
            values.Add(optionValue.Value.ToString() ?? string.Empty);
        }

        return (option.Name, values);
    }
}
