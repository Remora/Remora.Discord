//
//  DiscordRestInteractionAPI.cs
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
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using Remora.Discord.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestInteractionAPI" />
    [PublicAPI]
    public class DiscordRestInteractionAPI : AbstractDiscordRestAPI, IDiscordRestInteractionAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestInteractionAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The json options.</param>
        public DiscordRestInteractionAPI
        (
            DiscordHttpClient discordHttpClient,
            IOptions<JsonSerializerOptions> jsonOptions
        )
            : base(discordHttpClient, jsonOptions)
        {
        }

        /// <inheritdoc />
        public virtual Task<Result> CreateInteractionResponseAsync
        (
            Snowflake interactionID,
            string interactionToken,
            IInteractionResponse response,
            CancellationToken ct
        )
        {
            return this.DiscordHttpClient.PostAsync
            (
                $"interactions/{interactionID}/{interactionToken}/callback",
                b => b.WithJson
                (
                    json =>
                    {
                        JsonSerializer.Serialize(json, response, this.JsonOptions);
                    },
                    false
                ),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IMessage>> GetOriginalInteractionResponseAsync
        (
            Snowflake applicationID,
            string interactionToken,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IMessage>
            (
                $"webhooks/{applicationID}/{interactionToken}/messages/@original",
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
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            CancellationToken ct = default
        )
        {
            if (content.HasValue && content.Value?.Length > 2000)
            {
                return new NotSupportedError("Message content is too long (max 2000).");
            }

            if (embeds.HasValue && embeds.Value?.Count > 10)
            {
                return new NotSupportedError("Too many embeds (max 10).");
            }

            return await this.DiscordHttpClient.PatchAsync<IMessage>
            (
                $"webhooks/{applicationID}/{token}/messages/@original",
                b =>
                {
                    b.WithJson
                    (
                        json =>
                        {
                            json.Write("content", content, this.JsonOptions);
                            json.Write("embeds", embeds, this.JsonOptions);
                            json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                            json.Write("components", components, this.JsonOptions);
                        }
                    );
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
            return this.DiscordHttpClient.DeleteAsync
            (
                $"webhooks/{applicationID}/{token}/messages/@original",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IMessage>> CreateFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Optional<string> content = default,
            Optional<string> username = default,
            Optional<string> avatarUrl = default,
            Optional<bool> isTTS = default,
            Optional<FileData> file = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IAllowedMentions> allowedMentions = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            Optional<MessageFlags> flags = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PostAsync<IMessage>
            (
                $"webhooks/{applicationID}/{token}",
                b =>
                {
                    if (file.HasValue)
                    {
                        b.AddContent(new StreamContent(file.Value.Content), "file", file.Value.Name);
                    }

                    b.WithJson
                    (
                        json =>
                        {
                            json.Write("content", content, this.JsonOptions);
                            json.Write("username", username, this.JsonOptions);
                            json.Write("avatar_url", avatarUrl, this.JsonOptions);
                            json.Write("tts", isTTS, this.JsonOptions);
                            json.Write("embeds", embeds, this.JsonOptions);
                            json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                            json.Write("components", components, this.JsonOptions);
                            json.Write("flags", flags, this.JsonOptions);
                        }
                    );
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
            return this.DiscordHttpClient.GetAsync<IMessage>
            (
                $"webhooks/{applicationID}/{token}/messages/{messageID}",
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
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            CancellationToken ct = default
        )
        {
            if (content.HasValue && content.Value?.Length > 2000)
            {
                return new NotSupportedError("Message content is too long (max 2000).");
            }

            if (embeds.HasValue && embeds.Value?.Count > 10)
            {
                return new NotSupportedError("Too many embeds (max 10).");
            }

            return await this.DiscordHttpClient.PatchAsync<IMessage>
            (
                $"webhooks/{applicationID}/{token}/messages/{messageID}",
                b =>
                {
                    b.WithJson
                    (
                        json =>
                        {
                            json.Write("content", content, this.JsonOptions);
                            json.Write("embeds", embeds, this.JsonOptions);
                            json.Write("allowed_mentions", allowedMentions, this.JsonOptions);
                            json.Write("components", components, this.JsonOptions);
                        }
                    );
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
            return this.DiscordHttpClient.DeleteAsync
            (
                $"webhooks/{applicationID}/{token}/messages/{messageID}",
                ct: ct
            );
        }
    }
}
