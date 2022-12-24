//
//  ResponderAttribute.cs
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
using Remora.Discord.Gateway.Responders;

namespace Remora.Discord.Extensions.Attributes;

/// <summary>
/// Indicates that a responder type or method should be registered as the specified group.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ResponderAttribute : Attribute
{
    /// <summary>
    /// Gets the group to register the responder as.
    /// </summary>
    public ResponderGroup Group { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponderAttribute"/> class.
    /// </summary>
    /// <param name="group">
    /// The responder group the responder should be added to, defaulting to <see cref="ResponderGroup.Normal"/>.
    /// </param>
    public ResponderAttribute(ResponderGroup group = ResponderGroup.Normal)
    {
        this.Group = group;
    }
}
