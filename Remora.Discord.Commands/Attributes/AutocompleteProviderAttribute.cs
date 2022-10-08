//
//  AutocompleteProviderAttribute.cs
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

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Marks a parameter as having an associated autocomplete provider, which will dynamically suggest values as the
/// user is typing.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class AutocompleteProviderAttribute : AutocompleteAttribute
{
    /// <summary>
    /// Gets the desired autocomplete provider's identity.
    /// </summary>
    public string ProviderIdentity { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocompleteProviderAttribute"/> class.
    /// </summary>
    /// <param name="providerIdentity">The identity string of the provider.</param>
    public AutocompleteProviderAttribute(string providerIdentity)
    {
        this.ProviderIdentity = providerIdentity;
    }
}
