//
//  JsonSerializerOptionsExtensions.cs
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

using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Discord.API.Json;

namespace Remora.Discord.API.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="JsonSerializerOptions"/> class.
    /// </summary>
    [PublicAPI]
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Adds a data object converter to the given json options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TActual">The actual type.</typeparam>
        /// <returns>The added converter.</returns>
        public static DataObjectConverter<TInterface, TActual> AddDataObjectConverter<TInterface, TActual>
        (
            this JsonSerializerOptions options
        ) where TActual : TInterface
        {
            var converter = new DataObjectConverter<TInterface, TActual>();
            options.Converters.Add(converter);

            return converter;
        }

        /// <summary>
        /// Adds a JSON converter to the given json options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <typeparam name="TConverter">The converter type.</typeparam>
        /// <returns>The added converter.</returns>
        public static JsonSerializerOptions AddConverter<TConverter>(this JsonSerializerOptions options)
            where TConverter : JsonConverter, new()
        {
            options.Converters.Add(new TConverter());

            return options;
        }

        /// <summary>
        /// Clones the given serializer options, creating a new independent object with the same settings. The
        /// converters in the original options are copied to a new list in the new options.
        /// </summary>
        /// <param name="options">The options to clone.</param>
        /// <returns>The cloned options.</returns>
        public static JsonSerializerOptions Clone(this JsonSerializerOptions options)
        {
            var newOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = options.AllowTrailingCommas,
                DefaultBufferSize = options.DefaultBufferSize,
                DictionaryKeyPolicy = options.DictionaryKeyPolicy,
                Encoder = options.Encoder,
                IgnoreNullValues = options.IgnoreNullValues,
                IgnoreReadOnlyProperties = options.IgnoreReadOnlyProperties,
                MaxDepth = options.MaxDepth,
                PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive,
                PropertyNamingPolicy = options.PropertyNamingPolicy,
                ReadCommentHandling = options.ReadCommentHandling,
                WriteIndented = options.WriteIndented
            };

            foreach (var converter in options.Converters)
            {
                newOptions.Converters.Add(converter);
            }

            return newOptions;
        }
    }
}
