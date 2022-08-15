//
//  SampleDataDescriptor.cs
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

using System.IO;

namespace Remora.Discord.API.Tests.Services;

/// <summary>
/// Describes a file containing sample data, for testing.
/// </summary>
public class SampleDataDescriptor
{
    private readonly string _basePath;
    private readonly string _relativePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleDataDescriptor"/> class.
    /// </summary>
    /// <param name="basePath">The value to use for the data file's base filesystem path.</param>
    /// <param name="relativePath">The value to use for the data file's relative filesystem path.</param>
    public SampleDataDescriptor(string basePath, string relativePath)
    {
        _basePath = basePath;
        _relativePath = relativePath;
    }

    /// <summary>
    /// Gets the full filesystem path of the sample data file.
    /// </summary>
    public string FullPath => Path.Combine(_basePath, _relativePath);

    /// <inheritdoc/>
    public override string ToString() => _relativePath;
}
