//
//  EnumExtensions.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Humanizer;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Results;
using Remora.Results;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for enumerations.
/// </summary>
internal static class EnumExtensions
{
    private const int MaxChoiceNameLength = 100;
    private const int MaxChoiceValueLength = 100;

    private static readonly ConcurrentDictionary<Type, Result<IReadOnlyList<IApplicationCommandOptionChoice>>> ChoiceCache
        = new();

    /// <summary>
    /// Gets the Discord choices that the given enumeration is composed of.
    /// </summary>
    /// <remarks>
    /// This method is relatively expensive on the first call, after which the results will be cached. This method is
    /// thread-safe.
    /// </remarks>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>The choices.</returns>
    public static Result<IReadOnlyList<IApplicationCommandOptionChoice>> GetEnumChoices<TEnum>()
        where TEnum : struct, Enum
        => GetEnumChoices(typeof(TEnum));

    /// <summary>
    /// Gets the Discord choices that the given enumeration is composed of.
    /// </summary>
    /// <remarks>
    /// This method is relatively expensive on the first call, after which the results will be cached. This method is
    /// thread-safe.
    /// </remarks>
    /// <param name="enumType">The enumeration type.</param>
    /// <returns>The choices.</returns>
    public static Result<IReadOnlyList<IApplicationCommandOptionChoice>> GetEnumChoices(Type enumType)
    {
        return ChoiceCache.GetOrAdd
        (
            enumType,
            type =>
            {
                var values = Enum.GetValues(type);
                var choices = new List<IApplicationCommandOptionChoice>();

                foreach (var value in values)
                {
                    var enumName = Enum.GetName(type, value) ?? throw new InvalidOperationException();
                    var member = type.GetMember(enumName).Single();

                    if (member.GetCustomAttribute<ExcludeFromChoicesAttribute>() is not null)
                    {
                        continue;
                    }

                    var displayString = GetDisplayString(type, value);
                    if (displayString.Length > MaxChoiceNameLength)
                    {
                        return new UnsupportedFeatureError
                        (
                            $"The display name of the enumeration member {type.Name}::{enumName} is too long " +
                            $"(max {MaxChoiceNameLength}). Either configure a shorter name with " +
                            "[Description] or [Display], or rename the member."
                        );
                    }

                    var valueString = enumName;
                    if (valueString.Length <= MaxChoiceValueLength)
                    {
                        choices.Add(new ApplicationCommandOptionChoice(displayString, valueString));
                        continue;
                    }

                    // Try converting the enum's value representation
                    valueString = value.ToString() ?? throw new InvalidOperationException();
                    if (valueString.Length > MaxChoiceValueLength)
                    {
                        return new UnsupportedFeatureError
                        (
                            $"The length of the enumeration member {type.Name}::{enumName} value is too long " +
                            $"(max {MaxChoiceValueLength})."
                        );
                    }

                    choices.Add(new ApplicationCommandOptionChoice(displayString, valueString));
                }

                return choices;
            }
        );
    }

    /// <summary>
    /// Gets a string that should be displayed for the given enumeration value in user-facing interfaces.
    /// </summary>
    /// <param name="enumType">The type of the enumeration.</param>
    /// <param name="enumValue">The enumeration value.</param>
    /// <returns>The display string.</returns>
    private static string GetDisplayString(Type enumType, object enumValue)
    {
        var enumName = Enum.GetName(enumType, enumValue) ?? throw new InvalidOperationException();
        var enumMember = enumType.GetMember(enumName).Single();

        var descriptionAttribute = enumMember.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttribute is not null)
        {
            return descriptionAttribute.Description;
        }

        var displayAttribute = enumMember.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute is null)
        {
            return enumName.Humanize().Transform(To.TitleCase);
        }

        if (displayAttribute.Description is not null)
        {
            return displayAttribute.Description;
        }

        return displayAttribute.Name ?? enumName.Humanize().Transform(To.TitleCase);
    }
}
