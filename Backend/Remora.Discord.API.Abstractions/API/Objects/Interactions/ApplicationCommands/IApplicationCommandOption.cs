//
//  IApplicationCommandOption.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using OneOf;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an option in an application command.
/// </summary>
[PublicAPI]
public interface IApplicationCommandOption
{
    /// <summary>
    /// Gets the option type.
    /// </summary>
    ApplicationCommandOptionType Type { get; }

    /// <summary>
    /// Gets the name of the option.
    /// </summary>
    /// <remarks>The length of the name must be between 3 and 32 characters.</remarks>
    string Name { get; }

    /// <summary>
    /// Gets the description of the option.
    /// </summary>
    /// <remarks>The length of the description must be between 1 and 100 characters.</remarks>
    string Description { get; }

    /// <summary>
    /// Gets a value indicating whether this is the first required option that the user completes.
    /// </summary>
    /// <remarks>Only one option can be default.</remarks>
    Optional<bool> IsDefault { get; }

    /// <summary>
    /// Gets a value indicating whether the option is required.
    /// </summary>
    Optional<bool> IsRequired { get; }

    /// <summary>
    /// Gets the available options the user can pick from.
    /// </summary>
    Optional<IReadOnlyList<IApplicationCommandOptionChoice>> Choices { get; }

    /// <summary>
    /// Gets the options of the nested command or group.
    /// </summary>
    Optional<IReadOnlyList<IApplicationCommandOption>> Options { get; }

    /// <summary>
    /// Gets the channel types allowed for options.
    /// </summary>
    Optional<IReadOnlyList<ChannelType>> ChannelTypes { get; }

    /// <summary>
    /// Gets the minimum value permitted.
    /// </summary>
    /// <remarks>Only relevant for <see cref="ApplicationCommandOptionType.Integer"/> or
    /// <see cref="ApplicationCommandOptionType.Number"/> options.</remarks>
    Optional<OneOf<ulong, long, float, double>> MinValue { get; }

    /// <summary>
    /// Gets the minimum value permitted.
    /// </summary>
    /// <remarks>Only relevant for <see cref="ApplicationCommandOptionType.Integer"/> or
    /// <see cref="ApplicationCommandOptionType.Number"/> options.</remarks>
    Optional<OneOf<ulong, long, float, double>> MaxValue { get; }

    /// <summary>
    /// Gets a value indicating whether autocompletion should be enabled for this option.
    /// </summary>
    Optional<bool> EnableAutocomplete { get; }

    /// <summary>
    /// Gets the localized names of the option.
    /// </summary>
    Optional<IReadOnlyDictionary<string, string>?> NameLocalizations { get; }

    /// <summary>
    /// Gets the localized name of the option.
    /// </summary>
    /// <remarks>
    /// This field is only supplied by Discord as a response, and is not used to set the actual localized string.
    /// </remarks>
    Optional<string> NameLocalized { get; }

    /// <summary>
    /// Gets the localized descriptions of the option.
    /// </summary>
    Optional<IReadOnlyDictionary<string, string>?> DescriptionLocalizations { get; }

    /// <summary>
    /// Gets the localized description of the option.
    /// </summary>
    /// <remarks>
    /// This field is only supplied by Discord as a response, and is not used to set the actual localized string.
    /// </remarks>
    Optional<string> DescriptionLocalized { get; }

    /// <summary>
    /// Gets the minimum length of the value supplied to the parameter.
    /// </summary>
    /// <remarks>Only valid for <see cref="ApplicationCommandOptionType.String"/>.</remarks>
    Optional<uint> MinLength { get; }

    /// <summary>
    /// Gets the maximum length of the value supplied to the parameter.
    /// </summary>
    /// <remarks>Only valid for <see cref="ApplicationCommandOptionType.String"/>. Minimum 1.</remarks>
    Optional<uint> MaxLength { get; }
}
