//
//  CDNImageFormatExtensions.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.API.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="CDNImageFormat"/> enumeration.
/// </summary>
[PublicAPI]
public static class CDNImageFormatExtensions
{
    /// <summary>
    /// Maps the image format to a file extension.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>The extension.</returns>
    public static string ToFileExtension(this CDNImageFormat format)
    {
        return format switch
        {
            CDNImageFormat.JPEG => "jpeg",
            CDNImageFormat.PNG => "png",
            CDNImageFormat.WebP => "webp",
            CDNImageFormat.GIF => "gif",
            CDNImageFormat.Lottie => "json",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}
