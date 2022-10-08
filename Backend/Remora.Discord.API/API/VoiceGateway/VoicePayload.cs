//
//  VoicePayload.cs
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
using Remora.Discord.API.Abstractions.VoiceGateway;

namespace Remora.Discord.API.VoiceGateway;

/// <summary>
/// Represents a payload of data to be sent to, or received from, the Discord voice gateway.
/// </summary>
/// <typeparam name="TData">The data type encapsulated in the payload.</typeparam>
/// <param name="OperationCode">The operation code of the payload.</param>
/// <param name="Data">The data encapsulated in the payload.</param>
[PublicAPI]
public record VoicePayload<TData>
(
    VoiceOperationCode OperationCode,
    TData Data
) : IVoicePayload<TData> where TData : IVoiceGatewayPayloadData;
