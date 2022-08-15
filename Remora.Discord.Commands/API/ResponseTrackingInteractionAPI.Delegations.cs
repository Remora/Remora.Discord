//
//  ResponseTrackingInteractionAPI.Delegations.cs
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
using Remora.Rest;

namespace Remora.Discord.Commands.API;

internal partial class ResponseTrackingInteractionAPI
{
    /// <inheritdoc/>
    public RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            // TODO: not ideal...
            throw new NotImplementedException("The decorated API type is not customizable.");
        }

        return customizable.WithCustomization(requestCustomizer);
    }

    /// <inheritdoc/>
    void IRestCustomizable.RemoveCustomization(RestRequestCustomization customization)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            return;
        }

        customizable.RemoveCustomization(customization);
    }
}
