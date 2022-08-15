//
//  IResponderTypeRepository.cs
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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;

namespace Remora.Discord.Gateway.Services;

/// <summary>
/// Represents a type that can serve lists of registered responder types for gateway events.
/// </summary>
[PublicAPI]
public interface IResponderTypeRepository
{
    /// <summary>
    /// Gets all responder types that are relevant for the given event, and should run before any other responders.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The event type.</typeparam>
    /// <returns>A list of responder types.</returns>
    IReadOnlyList<Type> GetEarlyResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent;

    /// <summary>
    /// Gets all responder types that are relevant for the given event.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The event type.</typeparam>
    /// <returns>A list of responder types.</returns>
    IReadOnlyList<Type> GetResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent;

    /// <summary>
    /// Gets all responder types that are relevant for the given event, and should run after any other responders.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The event type.</typeparam>
    /// <returns>A list of responder types.</returns>
    IReadOnlyList<Type> GetLateResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent;
}
