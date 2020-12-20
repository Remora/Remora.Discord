//
//  FileData.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Core
{
    /// <summary>
    /// Represents a file.
    /// </summary>
    [PublicAPI]
    public class FileData
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        public Stream Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileData"/> class.
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <param name="content">The file content.</param>
        public FileData(string name, Stream content)
        {
            Name = name;
            Content = content;
        }
    }
}
