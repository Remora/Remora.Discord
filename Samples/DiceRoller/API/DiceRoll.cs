//
//  SPDX-FileName: DiceRoll.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

namespace Remora.Discord.Samples.DiceRoller.API;

/// <summary>
/// Represents a single roll of a dice.
/// </summary>
public class DiceRoll
{
    /// <summary>
    /// Gets or sets the rolled value.
    /// </summary>
    public ulong Value { get; set; }

    /// <summary>
    /// Gets or sets the dice type.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceRoll"/> class.
    /// </summary>
    public DiceRoll()
    {
        this.Type = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceRoll"/> class.
    /// </summary>
    /// <param name="value">The rolled value.</param>
    /// <param name="type">The dice type.</param>
    public DiceRoll(ulong value, string type)
    {
        this.Value = value;
        this.Type = type;
    }
}
