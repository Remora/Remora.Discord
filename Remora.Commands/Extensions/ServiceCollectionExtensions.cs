//
//  ServiceCollectionExtensions.cs
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
using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Commands.Groups;
using Remora.Commands.Parsers;
using Remora.Commands.Services;
using Remora.Commands.Trees;

namespace Remora.Commands.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a command module to the available services.
        /// </summary>
        /// <typeparam name="TCommandModule">The command module to register.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection, with the configured modules.</returns>
        public static IServiceCollection AddCommandModule<TCommandModule>
        (
            this IServiceCollection serviceCollection
        )
            where TCommandModule : CommandGroup
        {
            serviceCollection.AddScoped<TCommandModule>();
            serviceCollection.Configure<CommandTreeBuilder>
            (
                builder => builder.RegisterModule<TCommandModule>()
            );

            return serviceCollection;
        }

        /// <summary>
        /// Adds the services needed by the command subsystem.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection, with the required command services.</returns>
        public static IServiceCollection AddCommands
        (
            this IServiceCollection serviceCollection
        )
        {
            serviceCollection.TryAddSingleton
            (
                services =>
                {
                    var treeBuilder = services.GetRequiredService<IOptions<CommandTreeBuilder>>();
                    return treeBuilder.Value.Build();
                }
            );

            serviceCollection.TryAddScoped
            (
                services =>
                {
                    var tree = services.GetRequiredService<CommandTree>();
                    return new CommandService(tree, services);
                }
            );

            serviceCollection
                .AddSingletonParser<char, CharParser>()
                .AddSingletonParser<bool, BooleanParser>()
                .AddSingletonParser<byte, ByteParser>()
                .AddSingletonParser<sbyte, SByteParser>()
                .AddSingletonParser<ushort, UInt16Parser>()
                .AddSingletonParser<short, Int16Parser>()
                .AddSingletonParser<uint, UInt32Parser>()
                .AddSingletonParser<int, Int32Parser>()
                .AddSingletonParser<ulong, UInt64Parser>()
                .AddSingletonParser<long, Int64Parser>()
                .AddSingletonParser<float, SingleParser>()
                .AddSingletonParser<double, DoubleParser>()
                .AddSingletonParser<decimal, DecimalParser>()
                .AddSingletonParser<BigInteger, BigIntegerParser>()
                .AddSingletonParser<string, StringParser>()
                .AddSingletonParser<DateTimeOffset, DateTimeOffsetParser>()
                .TryAddSingleton(typeof(ITypeParser<>), typeof(EnumParser<>));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a type parser as a singleton service.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="TType">The type to parse.</typeparam>
        /// <typeparam name="TParser">The type parser.</typeparam>
        /// <returns>The service collection, with the parser.</returns>
        public static IServiceCollection AddSingletonParser<TType, TParser>
        (
            this IServiceCollection serviceCollection
        )
            where TType : notnull
            where TParser : class, ITypeParser<TType>
        {
            serviceCollection.TryAddSingleton<ITypeParser<TType>, TParser>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds a type parser as a singleton service.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="TType">The type to parse.</typeparam>
        /// <typeparam name="TParser">The type parser.</typeparam>
        /// <returns>The service collection, with the parser.</returns>
        public static IServiceCollection AddParser<TType, TParser>
        (
            this IServiceCollection serviceCollection
        )
            where TType : notnull
            where TParser : class, ITypeParser<TType>
        {
            serviceCollection.TryAddScoped<ITypeParser<TType>, TParser>();
            return serviceCollection;
        }
    }
}
