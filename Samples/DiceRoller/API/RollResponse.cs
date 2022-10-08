//
//  SPDX-FileName: RollResponse.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System.Collections.Generic;

namespace Remora.Discord.Samples.DiceRoller.API;

/// <summary>
/// Represents a roll response.
/// </summary>
public class RollResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the roll was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the rolled dice.
    /// </summary>
    public IReadOnlyList<DiceRoll> Dice { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RollResponse"/> class.
    /// </summary>
    public RollResponse()
    {
        this.Dice = new List<DiceRoll>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RollResponse"/> class.
    /// </summary>
    /// <param name="success">Whether the roll was successful.</param>
    /// <param name="dice">The rolled dice.</param>
    public RollResponse(bool success, IReadOnlyList<DiceRoll> dice)
    {
        this.Success = success;
        this.Dice = dice;
    }
}
