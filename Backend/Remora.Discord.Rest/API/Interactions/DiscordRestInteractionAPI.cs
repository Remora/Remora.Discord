//
//  DiscordRestInteractionAPI.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestInteractionAPI" />
[PublicAPI]
public class DiscordRestInteractionAPI : AbstractDiscordRestAPI, IDiscordRestInteractionAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestInteractionAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestInteractionAPI(IRestHttpClient restHttpClient, JsonSerializerOptions jsonOptions, ICacheProvider rateLimitCache)
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual async Task<Result> CreateInteractionResponseAsync
    (
        Snowflake interactionID,
        string interactionToken,
        IInteractionResponse response,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        CancellationToken ct = default
    )
    {
        Optional<IReadOnlyList<IPartialAttachment>> attachmentList = default;
        if (attachments.HasValue)
        {
            // build attachment list
            attachmentList = attachments.Value.Select
            (
                (f, i) => f.Match
                (
                    data => new PartialAttachment(DiscordSnowflake.New((ulong)i), data.Name, data.Description),
                    attachment => attachment
                )
            ).ToList();
        }

        return attachments.HasValue switch
        {
            true when !response.Data.HasValue => new InvalidOperationError
            (
                "Response data must be provided with the interaction response if attachments are to be " +
                "uploaded or retained."
            ),
            true when response.Data.HasValue &&
                      response.Data.Value.TryPickT0(out var messageData, out _) &&
                      messageData.Attachments.HasValue => new InvalidOperationError
            (
                "The response data may not contain user-supplied attachments; they would be overwritten by this " +
                $"call. Pass your desired attachments in the {nameof(attachments)} parameter instead."
            ),
            _ => await this.RestHttpClient.PostAsync
            (
                $"interactions/{interactionID}/{interactionToken}/callback",
                b =>
                {
                    if (attachmentList.HasValue)
                    {
                        for (var i = 0; i < attachments.Value.Count; i++)
                        {
                            if (!attachments.Value[i].IsT0)
                            {
                                continue;
                            }

                            var (name, stream, _) = attachments.Value[i].AsT0;
                            var contentName = $"files[{i}]";

                            b.AddContent(new StreamContent(stream), contentName, name);
                        }

                        if
                        (
                            response is not InteractionResponse { Data.HasValue: true } responseRecord ||
                            !responseRecord.Data.Value.TryPickT0(out var data, out _) ||
                            data is not InteractionMessageCallbackData dataRecord
                        )
                        {
                            throw new InvalidOperationException
                            (
                                "Currently, Remora doesn't support uploading attachments when the interaction " +
                                "response or callback data types don't derive from Remora's own record types. " +
                                "Sorry!"
                            );
                        }

                        response = responseRecord with { Data = new(dataRecord with { Attachments = attachmentList }) };
                    }

                    b.WithJson(json => JsonSerializer.Serialize(json, response, this.JsonOptions), false);
                    b.WithRateLimitContext(this.RateLimitCache, true);
                },
                ct
            )
        };
    }

    /// <inheritdoc />
    public virtual Task<Result<IMessage>> GetOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string interactionToken,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IMessage>
        (
            $"webhooks/{applicationID}/{interactionToken}/messages/@original",
            b => b.WithRateLimitContext(this.RateLimitCache, true),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IMessage>> EditOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string token,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    )
    {
        if (content.IsDefined(out var contentValue) && contentValue.Length > 2000)
        {
            return new NotSupportedError("Message content is too long (max 2000).");
        }

        if (embeds.IsDefined(out var embedsValue) && embedsValue.Count > 10)
        {
            return new NotSupportedError("Too many embeds (max 10).");
        }

        return await this.RestHttpClient.PatchAsync<IMessage>
        (
            $"webhooks/{applicationID}/{token}/messages/@original",
            b =>
            {
                Optional<IReadOnlyList<IPartialAttachment>?> attachmentList = default;
                if (attachments.HasValue && attachments.Value is null)
                {
                    attachmentList = null;
                }
                else if (attachments.HasValue && attachments.Value is not null)
                {
                    // build attachment list
                    attachmentList = attachments.Value.Select
                    (
                        (f, i) => f.Match
                        (
                            data => new PartialAttachment(DiscordSnowflake.New((ulong)i), data.Name, data.Description),
                            attachment => attachment
                        )
                    ).ToList();

                    for (var i = 0; i < attachments.Value.Count; i++)
                    {
                        if (!attachments.Value[i].IsT0)
                        {
                            continue;
                        }

                        var (name, stream, _) = attachments.Value[i].AsT0;
                        var contentName = $"files[{i}]";

                        b.AddContent(new StreamContent(stream), contentName, name);
                    }
                }

                b.WithJson
                (
                    json =>
                    {
                        json.Write("content", content, this.JsonOptions);
                        json.Write("embeds", embeds, this.JsonOptions);
                        json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                        json.Write("components", components, this.JsonOptions);
                        json.Write("attachments", attachmentList, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache, true);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string token,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"webhooks/{applicationID}/{token}/messages/@original",
            b => b.WithRateLimitContext(this.RateLimitCache, true),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IMessage>> CreateFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Optional<string> content = default,
        Optional<bool> isTTS = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        Optional<IAllowedMentions> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        Optional<MessageFlags> flags = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IMessage>
        (
            $"webhooks/{applicationID}/{token}",
            b =>
            {
                Optional<IReadOnlyList<IPartialAttachment>> attachmentList = default;
                if (attachments.HasValue)
                {
                    // build attachment list
                    attachmentList = attachments.Value.Select
                    (
                        (f, i) => f.Match
                        (
                            data => new PartialAttachment(DiscordSnowflake.New((ulong)i), data.Name, data.Description),
                            attachment => attachment
                        )
                    ).ToList();

                    for (var i = 0; i < attachments.Value.Count; i++)
                    {
                        if (!attachments.Value[i].IsT0)
                        {
                            continue;
                        }

                        var (name, stream, _) = attachments.Value[i].AsT0;
                        var contentName = $"files[{i}]";

                        b.AddContent(new StreamContent(stream), contentName, name);
                    }
                }

                b.WithJson
                (
                    json =>
                    {
                        json.Write("content", content, this.JsonOptions);
                        json.Write("tts", isTTS, this.JsonOptions);
                        json.Write("embeds", embeds, this.JsonOptions);
                        json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                        json.Write("components", components, this.JsonOptions);
                        json.Write("attachments", attachmentList, this.JsonOptions);
                        json.Write("flags", flags, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache, true);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IMessage>> GetFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IMessage>
        (
            $"webhooks/{applicationID}/{token}/messages/{messageID}",
            b => b.WithRateLimitContext(this.RateLimitCache, true),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IMessage>> EditFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    )
    {
        if (content.IsDefined(out var contentValue) && contentValue.Length > 2000)
        {
            return new NotSupportedError("Message content is too long (max 2000).");
        }

        if (embeds.IsDefined(out var embedsValue) && embedsValue.Count > 10)
        {
            return new NotSupportedError("Too many embeds (max 10).");
        }

        return await this.RestHttpClient.PatchAsync<IMessage>
        (
            $"webhooks/{applicationID}/{token}/messages/{messageID}",
            b =>
            {
                Optional<IReadOnlyList<IPartialAttachment>?> attachmentList = default;
                if (attachments.HasValue && attachments.Value is null)
                {
                    attachmentList = null;
                }
                else if (attachments.HasValue && attachments.Value is not null)
                {
                    // build attachment list
                    attachmentList = attachments.Value.Select
                    (
                        (f, i) => f.Match
                        (
                            data => new PartialAttachment(DiscordSnowflake.New((ulong)i), data.Name, data.Description),
                            attachment => attachment
                        )
                    ).ToList();

                    for (var i = 0; i < attachments.Value.Count; i++)
                    {
                        if (!attachments.Value[i].IsT0)
                        {
                            continue;
                        }

                        var (name, stream, _) = attachments.Value[i].AsT0;
                        var contentName = $"files[{i}]";

                        b.AddContent(new StreamContent(stream), contentName, name);
                    }
                }

                b.WithJson
                (
                    json =>
                    {
                        json.Write("content", content, this.JsonOptions);
                        json.Write("embeds", embeds, this.JsonOptions);
                        json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                        json.Write("components", components, this.JsonOptions);
                        json.Write("attachments", attachmentList, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache, true);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"webhooks/{applicationID}/{token}/messages/{messageID}",
            b => b.WithRateLimitContext(this.RateLimitCache, true),
            ct
        );
    }
}
