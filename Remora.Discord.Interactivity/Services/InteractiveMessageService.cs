//
//  InteractiveMessageService.cs
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
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Interactivity.Services;

/// <summary>
/// Defines functionality for sending interactive messages.
/// </summary>
[PublicAPI]
public class InteractiveMessageService
{
    private readonly IMemoryCache _memoryCache;
    private readonly FeedbackService _feedback;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractiveMessageService"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    /// <param name="feedback">The feedback service.</param>
    public InteractiveMessageService(IMemoryCache memoryCache, FeedbackService feedback)
    {
        _memoryCache = memoryCache;
        _feedback = feedback;
    }

    /// <summary>
    /// Sends the given embed to the given channel, and creates a piece of in-memory persistent data that will be
    /// associated with the created nonce.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <param name="channel">The channel to send the embed to.</param>
    /// <param name="embed">The embed.</param>
    /// <param name="nonceFactory">
    /// A factory function to create the nonce which will be used to associate the data with interactive entities.
    /// </param>
    /// <param name="dataFactory">A factory function to create the data to associate with the nonce.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result<IMessage>> SendInteractiveEmbedWithPersistentDataAsync<TData>
    (
        Snowflake channel,
        Embed embed,
        Func<IMessage, string> nonceFactory,
        Func<IMessage, TData> dataFactory,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        where TData : class
    {
        var sendEmbed = await _feedback.SendEmbedAsync(channel, embed, options, ct);
        if (!sendEmbed.IsSuccess)
        {
            return sendEmbed;
        }

        var message = sendEmbed.Entity;
        var nonce = nonceFactory(message);

        var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(nonce);
        _memoryCache.Set<(SemaphoreSlim, object)>(cacheKey, (new SemaphoreSlim(1, 1), dataFactory(message)));

        return sendEmbed;
    }

    /// <summary>
    /// Sends the given embed to the current context, and creates a piece of in-memory persistent data that will be
    /// associated with the created nonce.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <param name="embed">The embed.</param>
    /// <param name="nonceFactory">
    /// A factory function to create the nonce which will be used to associate the data with interactive entities.
    /// </param>
    /// <param name="dataFactory">A factory function to create the data to associate with the nonce.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result<IMessage>> SendInteractiveContextualEmbedWithPersistentDataAsync<TData>
    (
        Embed embed,
        Func<IMessage, string> nonceFactory,
        Func<IMessage, TData> dataFactory,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        where TData : class
    {
        var sendEmbed = await _feedback.SendContextualEmbedAsync(embed, options, ct);
        if (!sendEmbed.IsSuccess)
        {
            return sendEmbed;
        }

        var message = sendEmbed.Entity;
        var nonce = nonceFactory(message);

        var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(nonce);
        _memoryCache.Set<(SemaphoreSlim, object)>(cacheKey, (new SemaphoreSlim(1, 1), dataFactory(message)));

        return sendEmbed;
    }

    /// <summary>
    /// Sends the given embed to the he given user in their private DM channel, and creates a piece of in-memory
    /// persistent data that will be associated with the created nonce.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <param name="user">The ID of the user to send the embed to.</param>
    /// <param name="embed">The embed.</param>
    /// <param name="nonceFactory">
    /// A factory function to create the nonce which will be used to associate the data with interactive entities.
    /// </param>
    /// <param name="dataFactory">A factory function to create the data to associate with the nonce.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result<IMessage>> SendInteractivePrivateEmbedWithPersistentDataAsync<TData>
    (
        Snowflake user,
        Embed embed,
        Func<IMessage, string> nonceFactory,
        Func<IMessage, TData> dataFactory,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        where TData : class
    {
        var sendEmbed = await _feedback.SendPrivateEmbedAsync(user, embed, options, ct);
        if (!sendEmbed.IsSuccess)
        {
            return sendEmbed;
        }

        var message = sendEmbed.Entity;
        var nonce = nonceFactory(message);

        var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(nonce);
        _memoryCache.Set<(SemaphoreSlim, object)>(cacheKey, (new SemaphoreSlim(1, 1), dataFactory(message)));

        return sendEmbed;
    }

    /// <summary>
    /// Sends the given embed to the given channel, and creates a piece of in-memory persistent data that will be
    /// associated with the created nonce. One nonce and one data instance will be created for each message, and will
    /// be associated with each other.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <param name="channel">The channel to send the embed to.</param>
    /// <param name="contents">The contents to send.</param>
    /// <param name="color">The embed colour.</param>
    /// <param name="nonceFactory">
    /// A factory function to create the nonce which will be used to associate the data with interactive entities.
    /// </param>
    /// <param name="dataFactory">A factory function to create the data to associate with the nonce.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result<IReadOnlyList<IMessage>>> SendInteractiveContentsWithPersistentDataAsync<TData>
    (
        Snowflake channel,
        string contents,
        Color color,
        Func<IMessage, string> nonceFactory,
        Func<IMessage, TData> dataFactory,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        where TData : class
    {
        var sendContent = await _feedback.SendContentAsync(channel, contents, color, target, options, ct);
        if (!sendContent.IsSuccess)
        {
            return sendContent;
        }

        var messages = sendContent.Entity;
        var nonces = messages.Select(nonceFactory);
        var dataInstances = messages.Select(dataFactory);

        foreach (var (nonce, data) in nonces.Zip(dataInstances, (n, d) => (n, d)))
        {
            var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(nonce);
            _memoryCache.Set<(SemaphoreSlim, object)>(cacheKey, (new SemaphoreSlim(1, 1), data));
        }

        return sendContent;
    }

    /// <summary>
    /// Sends the given embed to the current context, and creates a piece of in-memory persistent data that will be
    /// associated with the created nonce. One nonce will be created for each message, and the data will be associated
    /// with each nonce.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <param name="contents">The contents to send.</param>
    /// <param name="color">The embed colour.</param>
    /// <param name="nonceFactory">
    /// A factory function to create the nonce which will be used to associate the data with interactive entities.
    /// </param>
    /// <param name="dataFactory">A factory function to create the data to associate with the nonce.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result<IReadOnlyList<IMessage>>> SendInteractiveContextualContentWithPersistentDataAsync<TData>
    (
        string contents,
        Color color,
        Func<IMessage, string> nonceFactory,
        Func<IMessage, TData> dataFactory,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        where TData : class
    {
        var sendContent = await _feedback.SendContextualContentAsync(contents, color, target, options, ct);
        if (!sendContent.IsSuccess)
        {
            return sendContent;
        }

        var messages = sendContent.Entity;
        var nonces = messages.Select(nonceFactory);
        var dataInstances = messages.Select(dataFactory);

        foreach (var (nonce, data) in nonces.Zip(dataInstances, (n, d) => (n, d)))
        {
            var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(nonce);
            _memoryCache.Set<(SemaphoreSlim, object)>(cacheKey, (new SemaphoreSlim(1, 1), data));
        }

        return sendContent;
    }

    /// <summary>
    /// Sends the given string to the given user in their private DM channel, and creates a piece of in-memory
    /// persistent data that will be associated with the created nonce. One nonce will be created for each message, and
    /// the data will be associated with each nonce.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <param name="user">The ID of the user to send the string to.</param>
    /// <param name="contents">The contents to send.</param>
    /// <param name="color">The embed colour.</param>
    /// <param name="nonceFactory">
    /// A factory function to create the nonce which will be used to associate the data with interactive entities.
    /// </param>
    /// <param name="dataFactory">A factory function to create the data to associate with the nonce.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result<IReadOnlyList<IMessage>>> SendInteractivePrivateContentWithPersistentDataAsync<TData>
    (
        Snowflake user,
        string contents,
        Color color,
        Func<IMessage, string> nonceFactory,
        Func<IMessage, TData> dataFactory,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        where TData : class
    {
        var sendContent = await _feedback.SendPrivateContentAsync(user, contents, color, options, ct);
        if (!sendContent.IsSuccess)
        {
            return sendContent;
        }

        var messages = sendContent.Entity;
        var nonces = messages.Select(nonceFactory);
        var dataInstances = messages.Select(dataFactory);

        foreach (var (nonce, data) in nonces.Zip(dataInstances, (n, d) => (n, d)))
        {
            var cacheKey = InMemoryPersistenceHelpers.CreateNonceKey(nonce);
            _memoryCache.Set<(SemaphoreSlim, object)>(cacheKey, (new SemaphoreSlim(1, 1), data));
        }

        return sendContent;
    }
}
