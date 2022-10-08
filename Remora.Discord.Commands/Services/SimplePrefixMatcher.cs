//
//  SimplePrefixMatcher.cs
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remora.Discord.Commands.Responders;
using Remora.Results;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Provides simple static prefix matching.
/// </summary>
public class SimplePrefixMatcher : ICommandPrefixMatcher
{
    private readonly CommandResponderOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimplePrefixMatcher"/> class.
    /// </summary>
    /// <param name="options">The responder options.</param>
    public SimplePrefixMatcher(IOptions<CommandResponderOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public ValueTask<Result<(bool Matches, int ContentStartIndex)>> MatchesPrefixAsync
    (
        string content,
        CancellationToken ct = default
    )
    {
        if (_options.Prefix is null)
        {
            return new((true, 0));
        }

        if (!content.StartsWith(_options.Prefix))
        {
            return new((false, -1));
        }

        var index = content.IndexOf(_options.Prefix, StringComparison.Ordinal) + _options.Prefix.Length;
        return new((true, index));
    }
}
