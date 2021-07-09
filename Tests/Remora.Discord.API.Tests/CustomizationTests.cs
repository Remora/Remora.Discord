//
//  CustomizationTests.cs
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Extensions;
using Xunit;

namespace Remora.Discord.API.Tests
{
    /// <summary>
    /// Tests various available user customizations of API types, such as overriding the implementation of a data type
    /// or adding an entirely new one.
    /// </summary>
    public class CustomizationTests
    {
        /// <summary>
        /// Represents the public interface of an existing data model.
        /// </summary>
        public interface IExisting
        {
            /// <summary>
            /// Gets some existing value.
            /// </summary>
            string ExistingValue { get; }
        }

        /// <summary>
        /// Represents the implementation of an existing data model.
        /// </summary>
        /// <param name="ExistingValue">Some existing value.</param>
        public record Existing(string ExistingValue) : IExisting;

        /// <summary>
        /// Represents the customized implementation of an existing data model.
        /// </summary>
        /// <param name="ExistingValue">Some existing value.</param>
        /// <param name="AdditionalValue">Some additional value.</param>
        public record Customized(string ExistingValue, string AdditionalValue) : Existing(ExistingValue);

        /// <summary>
        /// Tests whether an existing data type can be overridden.
        /// </summary>
        [Fact]
        public void CanOverrideExistingDataType()
        {
            var serviceCollection = new ServiceCollection()
                .AddDiscordApi()
                .Configure<JsonSerializerOptions>
                (
                    options =>
                    {
                        // Add the existing type
                        options.AddDataObjectConverter<IExisting, Existing>();
                    }
                );

            serviceCollection.Configure<JsonSerializerOptions>
            (
                options =>
                {
                    // Override the existing type
                    options.AddDataObjectConverter<IExisting, Customized>();
                }
            );

            var services = serviceCollection.BuildServiceProvider();

            var json = @"
            {
                ""existing_value"": ""some-value"",
                ""additional_value"": ""some-other-value""
            }";

            var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
            var value = JsonSerializer.Deserialize<IExisting>(json, jsonOptions);

            Assert.NotNull(value);
            Assert.IsType<Customized>(value);
            Assert.Equal("some-other-value", ((Customized)value!).AdditionalValue);
        }
    }
}
