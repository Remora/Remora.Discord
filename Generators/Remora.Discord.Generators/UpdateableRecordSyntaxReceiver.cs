//
//  UpdateableRecordSyntaxReceiver.cs
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remora.Discord.Generators.Support;

namespace Remora.Discord.Generators
{
    /// <summary>
    /// Caches and analyzers candidates for updateable record generation.
    /// </summary>
    public class UpdateableRecordSyntaxReceiver : ISyntaxReceiver
    {
        private readonly List<RecordDeclarationSyntax> _updatableRecords = new();
        private readonly List<Diagnostic> _diagnostics = new();

        /// <summary>
        /// Gets the valid record declarations that supporting code should be generated for.
        /// </summary>
        public IReadOnlyList<RecordDeclarationSyntax> UpdatableRecords => _updatableRecords;

        /// <summary>
        /// Gets any diagnostics raised during candidate analysis.
        /// </summary>
        public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

        /// <inheritdoc />
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not RecordDeclarationSyntax recordDeclaration)
            {
                return;
            }

            var attributeSyntheses = recordDeclaration.AttributeLists
                .SelectMany(a => a.Attributes)
                .Where(a => nameof(UpdateableRecordAttribute).StartsWith(a.Name.ToString()));

            if (!attributeSyntheses.Any())
            {
                return;
            }

            if (!recordDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                _diagnostics.Add
                (
                    Diagnostic.Create
                    (
                        DiagnosticDescriptors.UpdateableRecordsMustBePartial,
                        recordDeclaration.GetLocation()
                    )
                );

                return;
            }

            if (recordDeclaration.ParameterList is null)
            {
                _diagnostics.Add
                (
                    Diagnostic.Create
                    (
                        DiagnosticDescriptors.UpdateableRecordsMustBePositional,
                        recordDeclaration.GetLocation()
                    )
                );

                return;
            }

            _updatableRecords.Add(recordDeclaration);
        }
    }
}
