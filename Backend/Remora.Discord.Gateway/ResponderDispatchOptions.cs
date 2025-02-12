//
//  ResponderDispatchOptions.cs
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

using JetBrains.Annotations;
using Remora.Discord.Gateway.Services;

namespace Remora.Discord.Gateway;

/// <summary>
///  Represents options related to <see cref="IResponderDispatchService"/>.
/// </summary>
/// <param name="MaxItems">How many items can be queued for dispatch at any given time.</param>
[PublicAPI]
public record ResponderDispatchOptions(uint MaxItems = 100);
