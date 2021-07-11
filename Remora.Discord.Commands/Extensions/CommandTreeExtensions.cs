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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
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
    [PublicAPI]
    public static class CommandTreeExtensions
    {
        /*
         * Various Discord-imposed limits.
         */

        private const int MaxRootCommandsOrGroups = 100;
        private const int MaxGroupCommands = 25;
        private const int MaxChoiceValues = 25;
        private const int MaxCommandParameters = 25;
        private const int MaxCommandStringifiedLength = 4000;
        private const int MaxChoiceNameLength = 100;
        private const int MaxChoiceValueLength = 100;
        private const int MaxDescriptionLength = 100;
        private const int MaxTreeDepth = 3; // Top level is a depth of 1

        private const string NameRegexPattern = "^[a-z0-9_-]{1,32}$";

        /// <summary>
        /// Holds a regular expression that matches valid command names.
        /// </summary>
        private static readonly Regex NameRegex = new
        (
            NameRegexPattern,
            RegexOptions.Compiled
        );

        /// <summary>
        /// Converts the command tree to a set of Discord application commands.
        /// </summary>
        /// <param name="tree">The command tree.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        public static Result<IReadOnlyList<IBulkApplicationCommandData>> CreateApplicationCommands
        (
            this CommandTree tree
        )
        {
            var commands = new List<IBulkApplicationCommandData>();
            var commandNames = new HashSet<string>();
            foreach (var node in tree.Root.Children)
            {
                // Using the TryTranslateCommandNode() method here for the sake of code simplicity, even though it
                // returns an "option" object, which isn't truly what we want.
                var translationResult = TryTranslateCommandNode(node, 1);
                if (!translationResult.IsSuccess)
                {
                    return Result<IReadOnlyList<IBulkApplicationCommandData>>.FromError(translationResult.Unwrap());
                }

                if (translationResult.Entity is null)
                {
                    continue;
                }

                if (!commandNames.Add(translationResult.Entity.Name))
                {
                    return new UnsupportedFeatureError("Overloads are not supported.");
                }

                if (GetCommandStringifiedLength(translationResult.Entity) > MaxCommandStringifiedLength)
                {
                    return new UnsupportedFeatureError
                    (
                        "One or more commands is too long (combined length of name, description, and value " +
                        $"properties), max {MaxCommandStringifiedLength}).",
                        node
                    );
                }

                commands.Add
                (
                    new BulkApplicationCommandData
                    (
                        translationResult.Entity.Name,
                        translationResult.Entity.Description,
                        translationResult.Entity.Options
                    )
                );
            }

            if (commands.Count > MaxRootCommandsOrGroups)
            {
                return new UnsupportedFeatureError
                (
                    $"Too many root-level commands or groups (had {commands.Count}, max {MaxRootCommandsOrGroups})."
                );
            }

            return commands;
        }

        private static Result<IApplicationCommandOption?> TryTranslateCommandNode(IChildNode node, int treeDepth)
        {
            if (treeDepth > MaxTreeDepth)
            {
                return new UnsupportedFeatureError
                (
                    $"A sub-command or group was nested too deeply (depth {treeDepth}, max {MaxTreeDepth}.",
                    node
                );
            }

            switch (node)
            {
                case CommandNode command:
                {
                    if (command.GroupType.GetCustomAttribute<ExcludeFromSlashCommandsAttribute>() is not null)
                    {
                        return Result<IApplicationCommandOption?>.FromSuccess(null);
                    }

                    if (command.CommandMethod.GetCustomAttribute<ExcludeFromSlashCommandsAttribute>() is not null)
                    {
                        return Result<IApplicationCommandOption?>.FromSuccess(null);
                    }

                    var validateNameResult = ValidateCommandName(command.Key, command);
                    if (!validateNameResult.IsSuccess)
                    {
                        return Result<IApplicationCommandOption?>.FromError(validateNameResult.Unwrap());
                    }

                    var validateDescriptionResult = ValidateCommandDescription(command.Shape.Description, command);
                    if (!validateDescriptionResult.IsSuccess)
                    {
                        return Result<IApplicationCommandOption?>.FromError(validateDescriptionResult.Unwrap());
                    }

                    var buildOptionsResult = CreateCommandParameterOptions(command);
                    if (!buildOptionsResult.IsSuccess)
                    {
                        return Result<IApplicationCommandOption?>.FromError(buildOptionsResult.Unwrap());
                    }

                    return new ApplicationCommandOption
                    (
                        SubCommand, // Might not actually be a sub-command, but the caller will handle that
                        command.Key.ToLowerInvariant(),
                        command.Shape.Description,
                        Options: new(buildOptionsResult.Entity)
                    );
                }
                case GroupNode group:
                {
                    var validateNameResult = ValidateCommandName(group.Key, group);
                    if (!validateNameResult.IsSuccess)
                    {
                        return Result<IApplicationCommandOption?>.FromError(validateNameResult.Unwrap());
                    }

                    var validateDescriptionResult = ValidateCommandDescription(group.Description, group);
                    if (!validateDescriptionResult.IsSuccess)
                    {
                        return Result<IApplicationCommandOption?>.FromError(validateDescriptionResult.Unwrap());
                    }

                    var groupOptions = new List<IApplicationCommandOption>();
                    var groupOptionNames = new HashSet<string>();
                    var subCommandCount = 0;
                    foreach (var childNode in group.Children)
                    {
                        var translateChildNodeResult = TryTranslateCommandNode(childNode, treeDepth + 1);
                        if (!translateChildNodeResult.IsSuccess)
                        {
                            return Result<IApplicationCommandOption?>.FromError(translateChildNodeResult.Unwrap());
                        }

                        if (translateChildNodeResult.Entity is null)
                        {
                            // Skipped
                            continue;
                        }

                        if (translateChildNodeResult.Entity.Type is SubCommand)
                        {
                            ++subCommandCount;
                        }

                        if (!groupOptionNames.Add(translateChildNodeResult.Entity.Name))
                        {
                            return new UnsupportedFeatureError("Overloads are not supported.", group);
                        }

                        groupOptions.Add(translateChildNodeResult.Entity);
                    }

                    if (groupOptions.Count == 0)
                    {
                        return Result<IApplicationCommandOption?>.FromSuccess(null);
                    }

                    if (subCommandCount > MaxGroupCommands)
                    {
                        return new UnsupportedFeatureError
                        (
                            $"Too many commands under a group ({subCommandCount}, max {MaxGroupCommands}).",
                            group
                        );
                    }

                    return new ApplicationCommandOption
                    (
                        SubCommandGroup,
                        group.Key.ToLowerInvariant(),
                        group.Description,
                        Options: new(groupOptions)
                    );
                }
                default:
                {
                    throw new InvalidOperationException
                    (
                        $"Unable to translate node of type {node.GetType().FullName} into an application command"
                    );
                }
            }
        }

        private static Result<IReadOnlyList<IApplicationCommandOption>> CreateCommandParameterOptions
        (
            CommandNode command
        )
        {
            var parameterOptions = new List<IApplicationCommandOption>();
            foreach (var parameter in command.Shape.Parameters)
            {
                var validateDescriptionResult = ValidateCommandDescription(parameter.Description, command);
                if (!validateDescriptionResult.IsSuccess)
                {
                    return Result<IReadOnlyList<IApplicationCommandOption>>.FromError
                    (
                        validateDescriptionResult.Unwrap()
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
                        return Result<IReadOnlyList<IApplicationCommandOption>>.FromError(createChoices);
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

            return Result<IReadOnlyList<IApplicationCommandOption>>.FromSuccess(parameterOptions);
        }

        private static Result<IReadOnlyList<IApplicationCommandOptionChoice>> CreateApplicationCommandOptionChoices
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
                length += option.Options.Value.Sum(GetCommandStringifiedLength);
            }

            return length;
        }

        private static Result ValidateCommandName(string name, IChildNode node)
        {
            return NameRegex.IsMatch(name)
                ? Result.FromSuccess()
                : new UnsupportedFeatureError
                (
                    $"\"{name}\" is not a valid slash command or group name. " +
                    "Names must match the regex \"^[\\w-]{{1,32}}$\", and be lower-case.",
                    node
                );
        }

        private static Result ValidateCommandDescription(string description, IChildNode node)
        {
            return description.Length <= MaxDescriptionLength
                ? Result.FromSuccess()
                : new UnsupportedFeatureError
                (
                    $"A command, group, or parameter description was too long (length {description.Length}, max " +
                    $"{MaxDescriptionLength}).",
                    node
                );
        }
    }
}
