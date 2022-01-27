//
//  SampleDataDescriptor.cs
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

using System.IO;

namespace Remora.Discord.API.Tests.Services;

/// <summary>
/// Describes a file containing sample data, for testing.
/// </summary>
public class SampleDataDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleDataDescriptor"/> class.
    /// </summary>
    /// <param name="basePath">The value to use for <see cref="BasePath"/>.</param>
    /// <param name="relativePath">The value to use for <see cref="RelativePath"/>.</param>
    public SampleDataDescriptor(string basePath, string relativePath)
    {
        this.BasePath = basePath;
        this.RelativePath = relativePath;
    }

    /// <summary>
    /// Gets the base path at which the sample data file may be found.
    /// </summary>
    public string BasePath { get; }

    /// <summary>
    /// Gets the full filesystem path of the sample data file.
    /// </summary>
    public string FullPath => Path.Combine(this.BasePath, this.RelativePath);

    /// <summary>
    /// Gets the path to the sample data file, relative to other sample data files.
    /// </summary>
    public string RelativePath { get; }

    /// <inheritdoc/>
    public override string ToString() => this.RelativePath;
}
