//
//  SnakeCaseNamingPolicy.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Represents a snake_case naming policy.
    /// </summary>
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        /// <inheritdoc />
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var builder = new StringBuilder();

            var wordBoundaries = new List<int>();

            char? previous = null;
            for (var index = 0; index < name.Length; index++)
            {
                var c = name[index];

                if (previous.HasValue && char.IsUpper(previous.Value) && char.IsLower(c))
                {
                    wordBoundaries.Add(index - 1);
                }

                if (previous.HasValue && char.IsLower(previous.Value) && char.IsUpper(c))
                {
                    wordBoundaries.Add(index);
                }

                previous = c;
            }

            for (var index = 0; index < name.Length; index++)
            {
                var c = name[index];
                if (wordBoundaries.Contains(index) && index != 0)
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(c));
            }

            return builder.ToString();
        }
    }
}
