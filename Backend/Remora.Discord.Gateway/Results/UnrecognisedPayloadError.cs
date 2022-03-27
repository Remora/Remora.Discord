//
//  UnrecognisedPayloadError.cs
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

using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.Gateway.Results;

/// <summary>
/// Represents an error that occurs as a result of an inbound gateway payload not being recognised.
/// </summary>
/// <param name="Message">The error message.</param>
/// <param name="OpCode">The OP code of the payload.</param>
[PublicAPI]
public record UnrecognisedPayloadError(string Message, int? OpCode = null)
    : ResultError(Message);
