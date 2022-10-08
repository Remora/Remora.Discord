//
//  RequireContextAttribute.cs
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
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Extensions;

namespace Remora.Discord.Commands.Conditions;

/// <summary>
/// Marks a command as requiring execution within a particular context.
/// </summary>
[PublicAPI]
public class RequireContextAttribute : ConditionAttribute
{
    /// <summary>
    /// Gets the channel types command execution is permitted in.
    /// </summary>
    public ChannelType[] ChannelTypes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireContextAttribute"/> class.
    /// </summary>
    /// <param name="channelContexts">The grouped channel contexts.</param>
    public RequireContextAttribute(params ChannelContext[] channelContexts)
    {
        this.ChannelTypes = channelContexts.SelectMany(x => x.ToChannelTypes()).ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireContextAttribute"/> class.
    /// </summary>
    /// <param name="channelType">The individual channel types.</param>
    public RequireContextAttribute(params ChannelType[] channelType)
    {
        this.ChannelTypes = channelType;
    }
}
