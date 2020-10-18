//
//  UnitTimeSpanConverter.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to and from a specified time unit in JSON.
    /// </summary>
    /// <remarks>
    /// This converter does not take fractions into account, and only serializes whole integers.
    /// </remarks>
    [PublicAPI]
    public class UnitTimeSpanConverter : JsonConverter<TimeSpan>
    {
        private readonly TimeUnit _unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTimeSpanConverter"/> class.
        /// </summary>
        /// <param name="unit">The unit to convert to and from.</param>
        public UnitTimeSpanConverter(TimeUnit unit)
        {
            _unit = unit;
        }

        /// <inheritdoc />
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!reader.TryGetDouble(out var value))
            {
                throw new JsonException();
            }

            switch (_unit)
            {
                case TimeUnit.Days:
                {
                    return TimeSpan.FromDays(value);
                }
                case TimeUnit.Hours:
                {
                    return TimeSpan.FromHours(value);
                }
                case TimeUnit.Minutes:
                {
                    return TimeSpan.FromMinutes(value);
                }
                case TimeUnit.Seconds:
                {
                    return TimeSpan.FromSeconds(value);
                }
                case TimeUnit.Milliseconds:
                {
                    return TimeSpan.FromMilliseconds(value);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            switch (_unit)
            {
                case TimeUnit.Days:
                {
                    writer.WriteNumberValue((int)value.TotalDays);
                    break;
                }
                case TimeUnit.Hours:
                {
                    writer.WriteNumberValue((int)value.TotalHours);
                    break;
                }
                case TimeUnit.Minutes:
                {
                    writer.WriteNumberValue((int)value.TotalMinutes);
                    break;
                }
                case TimeUnit.Seconds:
                {
                    writer.WriteNumberValue((int)value.TotalSeconds);
                    break;
                }
                case TimeUnit.Milliseconds:
                {
                    writer.WriteNumberValue((int)value.TotalMilliseconds);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
