//
//  OneOfParser.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OneOf;
using Remora.Commands.Parsers;
using Remora.Commands.Services;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses various instances of the <see cref="OneOf{T}"/> type.
/// </summary>
[PublicAPI]
public class OneOfParser : AbstractTypeParser
{
    private static readonly IReadOnlyList<Type> _oneOfTypes = new List<Type>
    {
        typeof(OneOf<>),
        typeof(OneOf<,>),
        typeof(OneOf<,,>),
        typeof(OneOf<,,,>),
        typeof(OneOf<,,,,>),
        typeof(OneOf<,,,,,>),
        typeof(OneOf<,,,,,,>),
        typeof(OneOf<,,,,,,,>),
        typeof(OneOf<,,,,,,,,>)
    };

    private readonly TypeParserService _typeParserService;
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfParser"/> class.
    /// </summary>
    /// <param name="typeParserService">The type parser service.</param>
    /// <param name="services">The available services.</param>
    public OneOfParser(TypeParserService typeParserService, IServiceProvider services)
    {
        _typeParserService = typeParserService;
        _services = services;
    }

    /// <inheritdoc/>
    public override bool CanParse(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericType = type.GetGenericTypeDefinition();
        return _oneOfTypes.Contains(genericType);
    }

    /// <inheritdoc/>
    public override async ValueTask<Result<object?>> TryParseAsync
    (
        string token,
        Type type,
        CancellationToken ct = default
    )
    {
        var unionTypes = type.GetGenericArguments();

        var errors = new List<IResult>();
        for (var i = 0; i < unionTypes.Length; i++)
        {
            var unionType = unionTypes[i];
            var methodName = $"FromT{i}";

            var tryParse = await _typeParserService.TryParseAsync(_services, token, unionType, ct);
            if (!tryParse.IsSuccess)
            {
                errors.Add(tryParse);
                continue;
            }

            var method = type.GetMethod(methodName) ?? throw new MissingMethodException();

            var value = tryParse.Entity;
            return method.Invoke(null, new[] { value });
        }

        return new AggregateError(errors, $"\"{token}\" could not be parsed as any of the OneOf members.");
    }
}
