//
//  RollResponse.cs
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
