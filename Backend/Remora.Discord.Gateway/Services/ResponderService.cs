//
//  ResponderService.cs
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
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Responders;

namespace Remora.Discord.Gateway.Services;

/// <summary>
/// Handles introspection of registered responders.
/// </summary>
[PublicAPI]
public class ResponderService : IResponderTypeRepository
{
    private readonly Dictionary<Type, List<Type>> _registeredEarlyResponderTypes = new();
    private readonly Dictionary<Type, List<Type>> _registeredResponderTypes = new();
    private readonly Dictionary<Type, List<Type>> _registeredLateResponderTypes = new();

    /// <summary>
    /// Adds a responder to the service.
    /// </summary>
    /// <typeparam name="TResponder">The responder type.</typeparam>
    /// <param name="group">The group the responder belongs to.</param>
    internal void RegisterResponderType<TResponder>(ResponderGroup group) where TResponder : IResponder
    {
        RegisterResponderType(typeof(TResponder), group);
    }

    /// <summary>
    /// Adds a responder to the service.
    /// </summary>
    /// <param name="responderType">The responder type.</param>
    /// <param name="group">The group the responder belongs to.</param>
    internal void RegisterResponderType(Type responderType, ResponderGroup group)
    {
        if (!responderType.IsResponder())
        {
            throw new ArgumentException(
                $"{nameof(responderType)} should implement {nameof(IResponder)}.",
                nameof(responderType));
        }

        var responderTypeInterfaces = responderType.GetInterfaces();
        var responderInterfaces = responderTypeInterfaces.Where
        (
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IResponder<>)
        );

        foreach (var responderInterface in responderInterfaces)
        {
            var registeredTypes = group switch
            {
                ResponderGroup.Early => _registeredEarlyResponderTypes,
                ResponderGroup.Normal => _registeredResponderTypes,
                ResponderGroup.Late => _registeredLateResponderTypes,
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };

            if (!registeredTypes.TryGetValue(responderInterface, out var responderTypeList))
            {
                responderTypeList = new List<Type>();
                registeredTypes.Add(responderInterface, responderTypeList);
            }

            if (responderTypeList.Contains(responderType))
            {
                continue;
            }

            responderTypeList.Add(responderType);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> GetEarlyResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent
        => GetResponderTypes<TGatewayEvent>(_registeredEarlyResponderTypes);

    /// <inheritdoc />
    public IReadOnlyList<Type> GetResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent
        => GetResponderTypes<TGatewayEvent>(_registeredResponderTypes);

    /// <inheritdoc />
    public IReadOnlyList<Type> GetLateResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent
        => GetResponderTypes<TGatewayEvent>(_registeredLateResponderTypes);

    /// <summary>
    /// Gets all responder types that are relevant for the given event.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The event type.</typeparam>
    /// <param name="responderGroup">The responder group that responders should be retrieved from.</param>
    /// <returns>A list of responder types.</returns>
    public static IReadOnlyList<Type> GetResponderTypes<TGatewayEvent>
    (
        IReadOnlyDictionary<Type, List<Type>> responderGroup
    )
        where TGatewayEvent : IGatewayEvent
    {
        var typeKey = typeof(IResponder<TGatewayEvent>);

        // Fetch any responders which want all events
        if (!responderGroup.TryGetValue(typeof(IResponder<IGatewayEvent>), out var anyResponderTypes))
        {
            anyResponderTypes = new List<Type>();
        }

        // And add on the ones wanting this event type in particular
        return responderGroup.TryGetValue(typeKey, out var typedResponderTypes)
            ? anyResponderTypes.Concat(typedResponderTypes).ToList()
            : anyResponderTypes;
    }
}
