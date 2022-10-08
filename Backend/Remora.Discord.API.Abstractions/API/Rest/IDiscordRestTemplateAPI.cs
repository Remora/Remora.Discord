//
//  IDiscordRestTemplateAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Template API.
/// </summary>
[PublicAPI]
public interface IDiscordRestTemplateAPI
{
    /// <summary>
    /// Gets the template object for the given code.
    /// </summary>
    /// <param name="templateCode">The template code.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<ITemplate>> GetTemplateAsync
    (
        string templateCode,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new guild from the given template.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="templateCode">The template code.</param>
    /// <param name="name">The name of the new guild.</param>
    /// <param name="icon">The icon of the new guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IGuild>> CreateGuildFromTemplateAsync
    (
        string templateCode,
        string name,
        Optional<Stream> icon = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the template for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<ITemplate>>> GetGuildTemplatesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new guild template from the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the template.</param>
    /// <param name="description">The description of the template.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<ITemplate>> CreateGuildTemplateAsync
    (
        Snowflake guildID,
        string name,
        Optional<string?> description = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Synchronized the template to the guild's current state.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="templateCode">The template code.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<ITemplate>> SyncGuildTemplateAsync
    (
        Snowflake guildID,
        string templateCode,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the template's metadata.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="templateCode">The template code.</param>
    /// <param name="name">The new name of the template.</param>
    /// <param name="description">The new description of the template.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<ITemplate>> ModifyGuildTemplateAsync
    (
        Snowflake guildID,
        string templateCode,
        Optional<string> name,
        Optional<string?> description,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given guild template.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="templateCode">The template code.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded. This contains the deleted template.</returns>
    Task<Result<ITemplate>> DeleteGuildTemplateAsync
    (
        Snowflake guildID,
        string templateCode,
        CancellationToken ct = default
    );
}
