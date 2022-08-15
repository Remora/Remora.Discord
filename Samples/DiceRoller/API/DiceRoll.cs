//
//  DiceRoll.cs
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
