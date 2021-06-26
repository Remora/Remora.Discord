//
//  JsonAssertOptions.cs
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
using System.Text.Json;

namespace Remora.Discord.Tests
{
    /// <summary>
    /// Contains various options for the json assertions.
    /// </summary>
    public class JsonAssertOptions
    {
        /// <summary>
        /// Gets a list of property names that are allowed to be missing from the serialized result.
        /// </summary>
        public IReadOnlyCollection<string> AllowMissing { get; init; }

        /// <summary>
        /// Gets a function that inspects a property and determines if it's allowed to be missing in the serialized
        /// result.
        /// </summary>
        public Func<JsonProperty, bool> AllowMissingBy { get; init; }

        /// <summary>
        /// Gets a function that inspects an element and determines if validation of it should be skipped.
        /// </summary>
        public Func<JsonElement, bool> AllowSkip { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAssertOptions"/> class.
        /// </summary>
        /// <param name="allowMissing">The names that are allowed to be missing.</param>
        /// <param name="allowMissingBy">A function that determines if a property is allowed to be missing.</param>
        /// <param name="allowSkip">A function that determines if validation of an element should be skipped.</param>
        public JsonAssertOptions
        (
            IReadOnlyCollection<string>? allowMissing = default,
            Func<JsonProperty, bool>? allowMissingBy = default,
            Func<JsonElement, bool>? allowSkip = default
        )
        {
            this.AllowMissing = allowMissing ?? new List<string>();
            this.AllowMissingBy = allowMissingBy ?? (_ => false);
            this.AllowSkip = allowSkip ?? (_ => false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAssertOptions"/> class.
        /// </summary>
        /// <param name="original">An existing <see cref="JsonAssertOptions"/> object to be cloned..</param>
        public JsonAssertOptions(JsonAssertOptions original)
        {
            AllowMissing = original.AllowMissing;
            AllowMissingBy = original.AllowMissingBy;
            AllowSkip = original.AllowSkip;
        }

        /// <summary>
        /// Gets a default instance of the assertion options. This default option set allows underscore-prefixed fields
        /// to be missing.
        /// </summary>
        public static JsonAssertOptions Default { get; } = new
        (
            allowMissingBy: p => p.Name.StartsWith("_")
        );
    }
}
