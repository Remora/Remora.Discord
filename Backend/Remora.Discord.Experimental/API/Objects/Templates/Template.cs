//
//  Template.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Template : ITemplate
    {
        /// <inheritdoc />
        public string Code { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <inheritdoc />
        public int UsageCount { get; }

        /// <inheritdoc />
        public Snowflake CreatorID { get; }

        /// <inheritdoc />
        public IUser Creator { get; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; }

        /// <inheritdoc />
        public DateTimeOffset UpdatedAt { get; }

        /// <inheritdoc />
        public Snowflake SourceGuildID { get; }

        /// <inheritdoc />
        public IGuildTemplate SerializedSourceGuild { get; }

        /// <inheritdoc />
        public bool? IsDirty { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="code">The unique ID of the template.</param>
        /// <param name="name">The name of the template.</param>
        /// <param name="description">The template's description.</param>
        /// <param name="usageCount">The number of times the template has been used.</param>
        /// <param name="creatorID">The ID of the template's creator.</param>
        /// <param name="creator">The template's creator.</param>
        /// <param name="createdAt">The time when the template was created.</param>
        /// <param name="updatedAt">The last time the template was updated.</param>
        /// <param name="sourceGuildID">The ID of the source guild.</param>
        /// <param name="serializedSourceGuild">The source guild.</param>
        /// <param name="isDirty">Whether the template has unsynchronized changes.</param>
        public Template
        (
            string code,
            string name,
            string? description,
            int usageCount,
            Snowflake creatorID,
            IUser creator,
            DateTimeOffset createdAt,
            DateTimeOffset updatedAt,
            Snowflake sourceGuildID,
            IGuildTemplate serializedSourceGuild,
            bool? isDirty
        )
        {
            this.Code = code;
            this.Name = name;
            this.Description = description;
            this.UsageCount = usageCount;
            this.CreatorID = creatorID;
            this.Creator = creator;
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt;
            this.SourceGuildID = sourceGuildID;
            this.SerializedSourceGuild = serializedSourceGuild;
            this.IsDirty = isDirty;
        }
    }
}
