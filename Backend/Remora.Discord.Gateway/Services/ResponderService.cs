//
//  ResponderService.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;

namespace Remora.Discord.Gateway.Services
{
    /// <summary>
    /// Handles introspection of registered responders.
    /// </summary>
    public class ResponderService : IResponderTypeRepository
    {
        private readonly Dictionary<Type, List<Type>> _registeredResponderTypes = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Adds a responder to the service.
        /// </summary>
        /// <typeparam name="TResponder">The responder type.</typeparam>
        public void RegisterResponderType<TResponder>() where TResponder : IResponder
        {
            var responderTypeInterfaces = typeof(TResponder).GetInterfaces();
            var responderInterfaces = responderTypeInterfaces.Where
            (
                r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IResponder<>)
            );

            foreach (var responderInterface in responderInterfaces)
            {
                if (!_registeredResponderTypes.TryGetValue(responderInterface, out var responderTypeList))
                {
                    responderTypeList = new List<Type>();
                    _registeredResponderTypes.Add(responderInterface, responderTypeList);
                }

                if (responderTypeList.Contains(typeof(TResponder)))
                {
                    continue;
                }

                responderTypeList.Add(typeof(TResponder));
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetResponderTypes<TGatewayEvent>() where TGatewayEvent : IGatewayEvent
        {
            var typeKey = typeof(IResponder<TGatewayEvent>);
            if (!_registeredResponderTypes.TryGetValue(typeKey, out var responderTypes))
            {
                return Array.Empty<Type>();
            }

            return responderTypes;
        }
    }
}
