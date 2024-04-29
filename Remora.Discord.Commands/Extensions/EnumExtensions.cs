//
//  EnumExtensions.cs
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
using Remora.Discord.Commands.Services;

// long strings
#pragma warning disable SA1118

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for enumerations.
/// </summary>
internal static class EnumExtensions
{
    private const int _maxChoiceNameLength = 100;
    private const int _maxChoiceValueLength = 100;

    private static readonly ConcurrentDictionary<Type, IReadOnlyList<IApplicationCommandOptionChoice>> _choiceCache
        = new();

    /// <summary>
    /// Gets the Discord choices that the given enumeration is composed of.
    /// </summary>
    /// <remarks>
    /// This method is relatively expensive on the first call, after which the results will be cached. This method is
    /// thread-safe.
    /// </remarks>
    /// <param name="localizationProvider">The localization provider.</param>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>The choices.</returns>
    public static IReadOnlyList<IApplicationCommandOptionChoice> GetEnumChoices<TEnum>
    (
        ILocalizationProvider localizationProvider
    )
    where TEnum : struct, Enum
        => GetEnumChoices(typeof(TEnum), localizationProvider);

    /// <summary>
    /// Gets the Discord choices that the given enumeration is composed of.
    /// </summary>
    /// <remarks>
    /// This method is relatively expensive on the first call, after which the results will be cached. This method is
    /// thread-safe.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the enum has more than 25 members, or if any of its members translated names or values exceeds one of
    /// <see cref="_maxChoiceNameLength"/> or <see cref="_maxChoiceValueLength"/>.
    /// </exception>
    /// <param name="enumType">The enumeration type.</param>
    /// <param name="localizationProvider">The localization provider.</param>
    /// <returns>The choices.</returns>
    public static IReadOnlyList<IApplicationCommandOptionChoice> GetEnumChoices
    (
        Type enumType,
        ILocalizationProvider localizationProvider
    )
    {
        return _choiceCache.GetOrAdd
        (
            enumType,
            static (type, provider) =>
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
                    var localizedDisplayNames = provider.GetStrings(displayString);
                    foreach (var (locale, localizedDisplayName) in localizedDisplayNames)
                    {
                        if (localizedDisplayName.Length > _maxChoiceNameLength)
                        {
                            throw new ArgumentOutOfRangeException
                            (
                                nameof(enumType),
                                $"The localized display name for the locale {locale} of the enumeration member "
                                + $"{type.Name}::{enumName} is too long (max {_maxChoiceNameLength})."
                            );
                        }
                    }

                    var valueString = enumName;
                    if (valueString.Length <= _maxChoiceValueLength)
                    {
                        choices.Add
                        (
                            new ApplicationCommandOptionChoice
                            (
                                displayString,
                                valueString,
                                localizedDisplayNames.Count > 0 ? new(localizedDisplayNames) : default
                            )
                        );

                        continue;
                    }

                    // Try converting the enum's value representation
                    valueString = value.ToString() ?? throw new InvalidOperationException();
                    if (valueString.Length > _maxChoiceValueLength)
                    {
                        throw new ArgumentOutOfRangeException
                        (
                            nameof(enumType),
                            $"The length of the enumeration member {type.Name}::{enumName} value is too long " +
                            $"(max {_maxChoiceValueLength})."
                        );
                    }

                    choices.Add
                    (
                        new ApplicationCommandOptionChoice
                        (
                            displayString,
                            valueString,
                            localizedDisplayNames.Count > 0 ? new(localizedDisplayNames) : default
                        )
                    );
                }

                return choices;
            },
            localizationProvider
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
