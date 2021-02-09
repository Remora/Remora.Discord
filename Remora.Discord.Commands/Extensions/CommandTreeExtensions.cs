//
//  CommandTreeExtensions.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remora.Commands.Signatures;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Extensions;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Results;
using Remora.Discord.Core;
using Remora.Results;
using static Remora.Discord.API.Abstractions.Objects.ApplicationCommandOptionType;

namespace Remora.Discord.Commands.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="CommandTree"/> class.
    /// </summary>
    public static class CommandTreeExtensions
    {
        /*
         * Various Discord-imposed limits.
         */

        private const int MaxRootCommandsOrGroups = 50;
        private const int MaxGroupCommands = 10;
        private const int MaxChoiceValues = 10;
        private const int MaxCommandParameters = 10;
        private const int MaxGroupDepth = 2;

        /// <summary>
        /// Converts the command tree to a set of Discord application commands.
        /// </summary>
        /// <param name="tree">The command tree.</param>
        /// <param name="commands">The created commands, if any.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        public static Result CreateApplicationCommands
        (
            this CommandTree tree,
            [NotNullWhen(true)] out IReadOnlyList<IApplicationCommandOption>? commands
        )
        {
            commands = null;

            var createdCommands = new List<IApplicationCommandOption>();
            foreach (var child in tree.Root.Children)
            {
                var createOption = ToOption(child, out var option);
                if (!createOption.IsSuccess)
                {
                    return createOption;
                }

                createdCommands.Add(option!);
            }

            if (createdCommands.Count > MaxRootCommandsOrGroups)
            {
                return new UnsupportedFeatureError
                (
                    $"Too many root-level commands or groups (had {createdCommands.Count}, max " +
                    $"{MaxRootCommandsOrGroups})."
                );
            }

            if (createdCommands.GroupBy(c => c.Name).Any(g => g.Count() > 1))
            {
                return new UnsupportedFeatureError("Overloads are not supported.");
            }

            commands = createdCommands;
            return Result.FromSuccess();
        }

        private static Result ToOption
        (
            IChildNode child,
            [NotNullWhen(true)] out IApplicationCommandOption? option,
            ulong depth = 0
        )
        {
            option = null;

            switch (child)
            {
                case CommandNode command:
                {
                    var createParameterOptions = CreateCommandParameterOptions(command, out var parameterOptions);
                    if (!createParameterOptions.IsSuccess)
                    {
                        return createParameterOptions;
                    }

                    option = new ApplicationCommandOption
                    (
                        SubCommand,
                        command.Key,
                        command.Shape.Description,
                        default,
                        default,
                        default,
                        new Optional<IReadOnlyList<IApplicationCommandOption>>(parameterOptions)
                    );

                    return Result.FromSuccess();
                }
                case GroupNode group:
                {
                    if (depth >= MaxGroupDepth)
                    {
                        return new UnsupportedFeatureError
                        (
                            $"A group was nested too deeply (depth {depth}, max {MaxGroupDepth}.",
                            group
                        );
                    }

                    var groupOptions = new List<IApplicationCommandOption>();

                    // Continue down
                    foreach (var groupChild in group.Children)
                    {
                        var createNestedOption = ToOption(groupChild, out var nestedOptions, depth + 1);
                        if (!createNestedOption.IsSuccess)
                        {
                            return createNestedOption;
                        }

                        groupOptions.Add(nestedOptions!);

                        var subcommandCount = groupOptions.Count(o => o.Type == SubCommand);
                        if (subcommandCount > MaxGroupCommands)
                        {
                            return new UnsupportedFeatureError
                            (
                                $"Too many commands under a group ({subcommandCount}, max {MaxGroupCommands}).",
                                group
                            );
                        }

                        if (groupOptions.GroupBy(c => c.Name).Any(g => g.Count() > 1))
                        {
                            return new UnsupportedFeatureError("Overloads are not supported.", group);
                        }
                    }

                    option = new ApplicationCommandOption
                    (
                        SubCommandGroup,
                        group.Key,
                        group.Description,
                        default,
                        default,
                        default,
                        groupOptions
                    );

                    return Result.FromSuccess();
                }
                default:
                {
                    throw new InvalidOperationException
                    (
                        "An unknown node type was encountered; operation cannot continue."
                    );
                }
            }
        }

        private static Result CreateCommandParameterOptions
        (
            CommandNode command,
            [NotNullWhen(true)] out IReadOnlyList<IApplicationCommandOption>? parameters
        )
        {
            parameters = null;
            var parameterOptions = new List<IApplicationCommandOption>();

            // Create a set of parameter options
            foreach (var parameter in command.Shape.Parameters)
            {
                switch (parameter)
                {
                    case SwitchParameterShape:
                    {
                        return new UnsupportedParameterFeatureError
                        (
                            "Switch parameters are not supported.",
                            command,
                            parameter
                        );
                    }
                    case NamedCollectionParameterShape or PositionalCollectionParameterShape:
                    {
                        return new UnsupportedParameterFeatureError
                        (
                            "Collection parameters are not supported.",
                            command,
                            parameter
                        );
                    }
                }

                var parameterType = parameter.Parameter.ParameterType;
                var discordType = ToApplicationCommandOptionType(parameterType);
                var choices = CreateApplicationCommandOptionChoices(parameterType);

                var parameterOption = new ApplicationCommandOption
                (
                    discordType,
                    parameter.HintName,
                    parameter.Description,
                    default,
                    !parameter.IsOmissible(),
                    choices,
                    default
                );

                parameterOptions.Add(parameterOption);
            }

            if (parameterOptions.Count > MaxCommandParameters)
            {
                return new UnsupportedFeatureError
                (
                    $"Too many parameters in a command (had {parameterOptions.Count}, max {MaxCommandParameters}).",
                    command
                );
            }

            parameters = parameterOptions;
            return Result.FromSuccess();
        }

        private static Optional<IReadOnlyList<IApplicationCommandOptionChoice>> CreateApplicationCommandOptionChoices
        (
            Type parameterType
        )
        {
            if (!parameterType.IsEnum)
            {
                return default;
            }

            var enumNames = Enum.GetNames(parameterType);
            return enumNames.Length <= MaxChoiceValues
                ? enumNames.Select(n => new ApplicationCommandOptionChoice(n, n)).ToList()
                : default(Optional<IReadOnlyList<IApplicationCommandOptionChoice>>);
        }

        private static ApplicationCommandOptionType ToApplicationCommandOptionType(Type parameterType)
        {
            var discordType = parameterType switch
            {
                var t when t == typeof(bool) => ApplicationCommandOptionType.Boolean,
                var t when t == typeof(IRole) => ApplicationCommandOptionType.Role,
                var t when t == typeof(IUser) => ApplicationCommandOptionType.User,
                var t when t == typeof(IGuildMember) => ApplicationCommandOptionType.User,
                var t when t == typeof(IChannel) => ApplicationCommandOptionType.Channel,
                var t when t.IsInteger() => Integer,
                _ => ApplicationCommandOptionType.String
            };
            return discordType;
        }
    }
}
