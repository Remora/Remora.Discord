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
using System.Reflection;
using System.Text.RegularExpressions;
using Remora.Commands.Signatures;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Extensions;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
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

        private const int MaxRootCommandsOrGroups = 100;
        private const int MaxGroupCommands = 25;
        private const int MaxChoiceValues = 25;
        private const int MaxCommandParameters = 25;
        private const int MaxGroupDepth = 1;
        private const int MaxCommandStringifiedLength = 4000;
        private const int MaxChoiceNameLength = 100;
        private const int MaxChoiceValueLength = 100;
        private const int MaxDescriptionLength = 100;

        /// <summary>
        /// Holds a regular expression that matches valid command names.
        /// </summary>
        private static readonly Regex NameRegex = new
        (
            "^[\\w-]{1,32}$",
            RegexOptions.Compiled
        );

        /// <summary>
        /// Converts the command tree to a set of Discord application commands.
        /// </summary>
        /// <param name="tree">The command tree.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        public static Remora.Results.Result<IReadOnlyList<IApplicationCommandOption>> CreateApplicationCommands(this CommandTree tree)
        {
            var createdCommands = new List<IApplicationCommandOption>();
            foreach (var child in tree.Root.Children)
            {
                var createOption = ToOption(child, out var option, out var skip);
                if (!createOption.IsSuccess)
                {
                    return Remora.Results.Result<IReadOnlyList<IApplicationCommandOption>>.FromError(createOption);
                }

                if (skip)
                {
                    continue;
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

            if (createdCommands.Any(c => GetCommandStringifiedLength(c) > MaxCommandStringifiedLength))
            {
                return new UnsupportedFeatureError
                (
                    "One or more commands is too long (combined length of name, description, and value properties), " +
                    $"max {MaxCommandStringifiedLength})."
                );
            }

            return createdCommands;
        }

        private static Result ToOption
        (
            IChildNode child,
            [NotNullWhen(true)] out IApplicationCommandOption? option,
            out bool skip,
            ulong depth = 0
        )
        {
            option = null;
            skip = false;

            switch (child)
            {
                case CommandNode command:
                {
                    if (command.GroupType.GetCustomAttribute<ExcludeFromSlashCommandsAttribute>() is not null)
                    {
                        skip = true;
                        return Result.FromSuccess();
                    }

                    if (command.CommandMethod.GetCustomAttribute<ExcludeFromSlashCommandsAttribute>() is not null)
                    {
                        skip = true;
                        return Result.FromSuccess();
                    }

                    var createParameterOptions = CreateCommandParameterOptions(command, out var parameterOptions);
                    if (!createParameterOptions.IsSuccess)
                    {
                        return createParameterOptions;
                    }

                    if (!NameRegex.IsMatch(command.Key))
                    {
                        return new UnsupportedFeatureError
                        (
                            $"\"{command.Key}\" is not a valid slash command name. " +
                            "Names must match the regex \"^[\\w-]{{1,32}}$\", and be lower-case.",
                            command
                        );
                    }

                    var descriptionLength = command.Shape.Description.Length;
                    if (descriptionLength > MaxDescriptionLength)
                    {
                        return new UnsupportedFeatureError
                        (
                            $"A command's description was too long (length {descriptionLength}, max " +
                            $"{MaxDescriptionLength}).",
                            command
                        );
                    }

                    option = new ApplicationCommandOption
                    (
                        SubCommand,
                        command.Key.ToLowerInvariant(),
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
                    if (depth > MaxGroupDepth)
                    {
                        return new UnsupportedFeatureError
                        (
                            $"A group was nested too deeply (depth {depth}, max {MaxGroupDepth}.",
                            group
                        );
                    }

                    if (!NameRegex.IsMatch(group.Key))
                    {
                        return new UnsupportedFeatureError
                        (
                            $"\"{group.Key}\" is not a valid slash command group name. " +
                            "Names must match the regex \"^[\\w-]{{1,32}}$\", and be lower-case.",
                            group
                        );
                    }

                    var groupOptions = new List<IApplicationCommandOption>();

                    // Continue down
                    foreach (var groupChild in group.Children)
                    {
                        var createNestedOption = ToOption
                        (
                            groupChild,
                            out var nestedOptions,
                            out var skipNested,
                            depth + 1
                        );

                        if (!createNestedOption.IsSuccess)
                        {
                            return createNestedOption;
                        }

                        if (skipNested)
                        {
                            continue;
                        }

                        groupOptions.Add(nestedOptions!);
                    }

                    if (groupOptions.Count == 0)
                    {
                        skip = true;
                        return Result.FromSuccess();
                    }

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

                    option = new ApplicationCommandOption
                    (
                        SubCommandGroup,
                        group.Key.ToLowerInvariant(),
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
                var descriptionLength = parameter.Description.Length;
                if (descriptionLength is > MaxDescriptionLength)
                {
                    return new UnsupportedFeatureError
                    (
                        $"A parameter's description was too long (length {descriptionLength}, max " +
                        $"{MaxDescriptionLength}).",
                        command
                    );
                }

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

                var typeHint = parameter.Parameter.GetCustomAttribute<DiscordTypeHintAttribute>();
                var discordType = typeHint is null
                    ? ToApplicationCommandOptionType(parameterType)
                    : (ApplicationCommandOptionType)typeHint.TypeHint;

                Optional<IReadOnlyList<IApplicationCommandOptionChoice>> choices = default;
                if (parameterType.IsEnum)
                {
                    var createChoices = CreateApplicationCommandOptionChoices(parameterType);
                    if (!createChoices.IsSuccess)
                    {
                        return Result.FromError(createChoices);
                    }

                    choices = new(createChoices.Entity);
                }

                var parameterOption = new ApplicationCommandOption
                (
                    discordType,
                    parameter.HintName.ToLowerInvariant(),
                    parameter.Description,
                    default,
                    !parameter.IsOmissible(),
                    choices
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

        private static Remora.Results.Result<IReadOnlyList<IApplicationCommandOptionChoice>> CreateApplicationCommandOptionChoices
        (
            Type parameterType
        )
        {
            var enumNames = Enum.GetNames(parameterType);
            if (enumNames.Any(n => n.Length > MaxChoiceValueLength))
            {
                return new UnsupportedFeatureError
                (
                    $"One or more enumeration members is too long (max {MaxChoiceValueLength})."
                );
            }

            if (enumNames.Any(n => n.Length > MaxChoiceNameLength))
            {
                return new UnsupportedFeatureError
                (
                    $"One or more enumeration members is too long (max {MaxChoiceNameLength})."
                );
            }

            return enumNames.Length <= MaxChoiceValues
                ? enumNames.Select(n => new ApplicationCommandOptionChoice(n, n)).ToList()
                : new UnsupportedFeatureError($"The enumeration contains too many members (max {MaxChoiceValues}");
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

        private static int GetCommandStringifiedLength(IApplicationCommandOption option)
        {
            var length = 0;
            length += option.Name.Length;
            length += option.Description.Length;

            if (option.Choices.HasValue)
            {
                foreach (var choice in option.Choices.Value)
                {
                    length += choice.Name.Length;
                    if (choice.Value.TryPickT0(out var choiceValue, out _))
                    {
                        length += choiceValue.Length;
                    }
                }
            }

            if (option.Options.HasValue)
            {
                foreach (var commandOption in option.Options.Value)
                {
                    length += GetCommandStringifiedLength(commandOption);
                }
            }

            return length;
        }
    }
}
