//
//  ProgramOptions.cs
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
using CommandLine;
using JetBrains.Annotations;

namespace Remora.Discord.SensitiveDataScrubber
{
    /// <summary>
    /// Represents the command-line options of the program.
    /// </summary>
    public class ProgramOptions
    {
        /// <summary>
        /// Gets the files to scrub.
        /// </summary>
        [Option('i', "input-files", Min = 1, Required = true, HelpText = "The files to scrub.")]
        public IReadOnlyList<string> InputFiles { get; }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        [Option('o', "output-directory", HelpText = "The output directory.")]
        public string OutputDirectory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramOptions"/> class.
        /// </summary>
        /// <param name="inputFiles">The files to scrub.</param>
        /// <param name="outputDirectory">The output directory.</param>
        public ProgramOptions(IReadOnlyList<string> inputFiles, string? outputDirectory)
        {
            this.InputFiles = inputFiles;
            this.OutputDirectory = outputDirectory ?? Environment.CurrentDirectory;
        }
    }
}
