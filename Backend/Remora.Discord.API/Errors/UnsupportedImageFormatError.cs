//
//  UnsupportedImageFormatError.cs
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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.API.Errors;

/// <summary>
/// Represents an error produced by requesting an unsupported image format.
/// </summary>
[PublicAPI]
public record UnsupportedImageFormatError(IReadOnlyList<CDNImageFormat> SupportedFormats) : ResultError
(
    "Unsupported image format. The endpoint supports the following formats: " +
    $"{string.Join(", ", SupportedFormats.Select(f => f.ToString()))}"
);
