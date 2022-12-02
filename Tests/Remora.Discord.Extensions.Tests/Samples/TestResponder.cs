//
//  TestResponder.cs
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

using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Extensions.Attributes;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Extensions.Tests.Samples;

/// <summary>
/// A test responder.
/// </summary>
[Responder]
public class TestResponder : IResponder<IMessageCreate>, IResponder<IMessageDelete>
{
    /// <inheritdoc/>
    public Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
    {
        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc/>
    public Task<Result> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
    {
        return Task.FromResult(Result.FromSuccess());
    }
}
