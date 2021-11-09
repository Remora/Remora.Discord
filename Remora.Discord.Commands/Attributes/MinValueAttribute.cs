//
//  MinValueAttribute.cs
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
using OneOf;
using Remora.Discord.Core;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Defines an allowed range for a marked parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class MinValueAttribute : Attribute
{
    /// <summary>
    /// Gets the minimum allowed value.
    /// </summary>
    public Optional<OneOf<ulong, long, float, double>> Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinValueAttribute"/> class.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    public MinValueAttribute(long? minValue = default)
    {
        this.Value = minValue is null
            ? default
            : new Optional<OneOf<ulong, long, float, double>>(minValue.Value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinValueAttribute"/> class.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    public MinValueAttribute(ulong? minValue = default)
    {
        this.Value = minValue is null
            ? default
            : new Optional<OneOf<ulong, long, float, double>>(minValue.Value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinValueAttribute"/> class.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    public MinValueAttribute(float? minValue = default)
    {
        this.Value = minValue is null
            ? default
            : new Optional<OneOf<ulong, long, float, double>>(minValue.Value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinValueAttribute"/> class.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    public MinValueAttribute(double? minValue = default)
    {
        this.Value = minValue is null
            ? default
            : new Optional<OneOf<ulong, long, float, double>>(minValue.Value);
    }
}
