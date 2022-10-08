//
//  FileData.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents a file with its associated information.
/// </summary>
/// <param name="Name">The name of the file.</param>
/// <param name="Content">The contents of the file.</param>
/// <param name="Description">The file description.</param>
[PublicAPI]
public record FileData(string Name, Stream Content, string Description = "No description set.");
