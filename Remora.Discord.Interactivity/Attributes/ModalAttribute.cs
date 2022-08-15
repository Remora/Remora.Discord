//
//  ModalAttribute.cs
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

using System.Linq;
using JetBrains.Annotations;
using Remora.Commands.Attributes;

namespace Remora.Discord.Interactivity;

/// <summary>
/// Marks a method in an interaction group as a handler for modal interactions.
/// </summary>
[PublicAPI]
[MeansImplicitUse(ImplicitUseKindFlags.Access)]
public class ModalAttribute : CommandAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModalAttribute"/> class.
    /// </summary>
    /// <param name="name">The modal's custom ID, excluding Remora's prefixed metadata.</param>
    /// <param name="aliases">The modal's custom ID aliases, excluding Remora's prefixed metadata.</param>
    public ModalAttribute(string name, params string[] aliases)
        : base($"modal::{name}", aliases.Select(a => $"modal::{a}").ToArray())
    {
    }
}
