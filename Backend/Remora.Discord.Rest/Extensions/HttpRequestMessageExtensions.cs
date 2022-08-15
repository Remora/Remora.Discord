//
//  HttpRequestMessageExtensions.cs
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
using System.Net.Http;
using Polly;

namespace Remora.Discord.Rest.Extensions;

/// <summary>
/// Defines extensions to the <see cref="HttpRequestMessage"/> class.
/// </summary>
internal static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Modifies the policy execution context of supplied HTTP request.
    /// </summary>
    /// <remarks>
    /// Ensures that a policy execution context is set for supplied HTTP request.
    /// </remarks>
    /// <param name="request">The HTTP request.</param>
    /// <param name="modifyContext">The action that modifies the context.</param>
    /// <returns>The HTTP request that policy execution context was modified.</returns>
    public static HttpRequestMessage ModifyPolicyExecutionContext
    (
        this HttpRequestMessage request,
        Action<Context> modifyContext
    )
    {
        var context = request.GetPolicyExecutionContext();
        if (context is null)
        {
            context = new Context();
            request.SetPolicyExecutionContext(context);
        }

        modifyContext(context);

        return request;
    }
}
