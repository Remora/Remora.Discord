//
//  DiscordRestChannelAPI.cs
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
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;
using Remora.Discord.Rest.Results;

namespace Remora.Discord.Rest.API
{
    /// <summary>
    /// Implements the Discord REST channel API.
    /// </summary>
    public class DiscordRestChannelAPI : IDiscordRestChannelAPI
    {
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestChannelAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        public DiscordRestChannelAPI(DiscordHttpClient discordHttpClient, IOptions<JsonSerializerOptions> jsonOptions)
        {
            _discordHttpClient = discordHttpClient;
            _jsonOptions = jsonOptions.Value;
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IChannel>> GetChannelAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IChannel>
            (
                $"channels/{channelID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public async Task<IModifyRestEntityResult<IChannel>> ModifyChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<ChannelType> type = default,
            Optional<int?> position = default,
            Optional<string?> topic = default,
            Optional<bool?> isNsfw = default,
            Optional<int?> rateLimitPerUser = default,
            Optional<int?> bitrate = default,
            Optional<int?> userLimit = default,
            Optional<IReadOnlyList<IPermissionOverwrite>> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            CancellationToken ct = default
        )
        {
            if (name.HasValue && (name.Value!.Length > 100 || name.Value!.Length < 2))
            {
                return ModifyRestEntityResult<IChannel>.FromError("The name must be between 2 and 100 characters.");
            }

            if (topic.HasValue && !(topic.Value is null) && (topic.Value.Length > 1024 || topic.Value.Length < 0))
            {
                return ModifyRestEntityResult<IChannel>.FromError("The topic must be between 0 and 1024 characters.");
            }

            if (userLimit.HasValue && !(userLimit.Value is null) && (name.Value!.Length > 99 || name.Value!.Length < 0))
            {
                return ModifyRestEntityResult<IChannel>.FromError("The user limit must be between 0 and 99.");
            }

            return await _discordHttpClient.PatchAsync<IChannel>
            (
                $"channels/{channelID}",
                b => b.WithJson
                (
                    json =>
                    {
                        if (name.HasValue)
                        {
                            json.WriteString("name", name.Value);
                        }

                        if (type.HasValue)
                        {
                            json.WriteNumber("type", (int)type.Value);
                        }

                        if (position.HasValue)
                        {
                            if (position.Value is null)
                            {
                                json.WriteNull("position");
                            }
                            else
                            {
                                json.WriteNumber("position", position.Value.Value);
                            }
                        }

                        if (topic.HasValue)
                        {
                            if (topic.Value is null)
                            {
                                json.WriteNull("topic");
                            }
                            else
                            {
                                json.WriteString("topic", topic.Value);
                            }
                        }

                        if (isNsfw.HasValue)
                        {
                            if (isNsfw.Value is null)
                            {
                                json.WriteNull("nsfw");
                            }
                            else
                            {
                                json.WriteBoolean("nsfw", isNsfw.Value.Value);
                            }
                        }

                        if (rateLimitPerUser.HasValue)
                        {
                            if (rateLimitPerUser.Value is null)
                            {
                                json.WriteNull("rate_limit_per_user");
                            }
                            else
                            {
                                json.WriteNumber("rate_limit_per_user", rateLimitPerUser.Value.Value);
                            }
                        }

                        if (bitrate.HasValue)
                        {
                            if (bitrate.Value is null)
                            {
                                json.WriteNull("bitrate");
                            }
                            else
                            {
                                json.WriteNumber("bitrate", bitrate.Value.Value);
                            }
                        }

                        if (userLimit.HasValue)
                        {
                            if (userLimit.Value is null)
                            {
                                json.WriteNull("user_limit");
                            }
                            else
                            {
                                json.WriteNumber("user_limit", userLimit.Value.Value);
                            }
                        }

                        if (permissionOverwrites.HasValue)
                        {
                            if (permissionOverwrites.Value is null)
                            {
                                json.WriteNull("permission_overwrites");
                            }
                            else
                            {
                                json.WritePropertyName("permission_overwrites");
                                JsonSerializer.Serialize(json, permissionOverwrites.Value, _jsonOptions);
                            }
                        }

                        if (parentId.HasValue)
                        {
                            if (parentId.Value is null)
                            {
                                json.WriteNull("parent_id");
                            }
                            else
                            {
                                json.WriteString("parent_id", parentId.Value.Value.ToString());
                            }
                        }
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteChannelAsync
        (
            Snowflake channelID,
            CancellationToken ct
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public async Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
        (
            Snowflake channelID,
            Optional<Snowflake> around = default,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            if (limit.HasValue && (limit.Value > 100 || limit.Value < 1))
            {
                return RetrieveRestEntityResult<IReadOnlyList<IMessage>>.FromError
                (
                    "The message limit must be between 1 and 100."
                );
            }

            return await _discordHttpClient.GetAsync<IReadOnlyList<IMessage>>
            (
                $"channels/{channelID}/messages",
                b =>
                {
                    if (around.HasValue)
                    {
                        b.AddQueryParameter("around", around.Value.ToString());
                    }

                    if (before.HasValue)
                    {
                        b.AddQueryParameter("before", before.Value.ToString());
                    }

                    if (after.HasValue)
                    {
                        b.AddQueryParameter("after", after.Value.ToString());
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct
            );
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IMessage>> GetChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IMessage>
            (
                $"channels/{channelID}/messages/{messageID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<ICreateRestEntityResult<IMessage>> CreateMessageAsync
        (
            Snowflake channelID,
            Optional<string> content = default,
            Optional<string> nonce = default,
            Optional<bool> isTTS = default,
            Optional<Stream> file = default,
            Optional<IEmbed> embed = default,
            Optional<IAllowedMentions> allowedMentions = default,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PostAsync<IMessage>
            (
                $"channels/{channelID}/messages",
                b =>
                {
                    if (file.HasValue)
                    {
                        b.AddContent(new StreamContent(file.Value), "file");
                    }

                    b.WithJson
                    (
                        writer =>
                        {
                            if (content.HasValue)
                            {
                                writer.WriteString("content", content.Value);
                            }

                            if (nonce.HasValue)
                            {
                                writer.WriteString("nonce", nonce.Value);
                            }

                            if (isTTS.HasValue)
                            {
                                writer.WriteBoolean("tts", isTTS.Value);
                            }

                            if (embed.HasValue)
                            {
                                writer.WritePropertyName("embed");
                                JsonSerializer.Serialize(writer, embed.Value, _jsonOptions);
                            }

                            if (!allowedMentions.HasValue)
                            {
                                return;
                            }

                            writer.WritePropertyName("allowed_mentions");
                            JsonSerializer.Serialize(writer, allowedMentions.Value, _jsonOptions);
                        }
                    );
                },
                ct
            );
        }

        /// <inheritdoc />
        public Task<IRestResult> CreateReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PutAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/@me",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteOwnReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/@me",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteUserReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Snowflake user,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}/{user}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public async Task<IRetrieveRestEntityResult<IReadOnlyList<IUser>>> GetReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            if (limit.HasValue && (limit.Value > 100 || limit.Value < 1))
            {
                return RetrieveRestEntityResult<IReadOnlyList<IUser>>.FromError("The limit must be between 1 and 100.");
            }

            return await _discordHttpClient.GetAsync<IReadOnlyList<IUser>>
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}",
                b =>
                {
                    if (before.HasValue)
                    {
                        b.AddQueryParameter("before", before.Value.ToString());
                    }

                    if (after.HasValue)
                    {
                        b.AddQueryParameter("after", after.Value.ToString());
                    }

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
                },
                ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteAllReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteAllReactionsForEmojiAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}/reactions/{HttpUtility.UrlEncode(emoji)}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IModifyRestEntityResult<IMessage>> EditMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IEmbed?> embed = default,
            Optional<MessageFlags?> flags = default,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PatchAsync<IMessage>
            (
                $"channels/{channelID}/messages/{messageID}",
                b => b.WithJson
                (
                    json =>
                    {
                        if (content.HasValue)
                        {
                            if (content.Value is null)
                            {
                                json.WriteNull("content");
                            }
                            else
                            {
                                json.WriteString("content", content.Value);
                            }
                        }

                        if (embed.HasValue)
                        {
                            if (embed.Value is null)
                            {
                                json.WriteNull("embed");
                            }
                            else
                            {
                                json.WritePropertyName("embed");
                                JsonSerializer.Serialize(json, embed.Value, _jsonOptions);
                            }
                        }

                        if (flags.HasValue)
                        {
                            if (flags.Value is null)
                            {
                                json.WriteNull("flags");
                            }
                            else
                            {
                                json.WriteNumber("flags", (int)flags.Value);
                            }
                        }
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/{messageID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public async Task<IDeleteRestEntityResult> BulkDeleteMessagesAsync
        (
            Snowflake channelID,
            IReadOnlyList<Snowflake> messageIDs,
            CancellationToken ct = default
        )
        {
            if (messageIDs.Count > 100 || messageIDs.Count < 2)
            {
                return DeleteRestEntityResult.FromError("The number of messages to delete must be between 2 and 100.");
            }

            return await _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/messages/bulk-delete",
                b => b.WithJson
                (
                    json =>
                    {
                        json.WritePropertyName("messages");
                        JsonSerializer.Serialize(json, messageIDs, _jsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRestResult> EditChannelPermissionsAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            Optional<IDiscordPermissionSet> allow = default,
            Optional<IDiscordPermissionSet> deny = default,
            Optional<PermissionOverwriteType> type = default,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PutAsync
            (
                $"channels/{channelID}/permissions/{overwriteID}",
                b => b.WithJson
                (
                    json =>
                    {
                        if (allow.HasValue)
                        {
                            json.WritePropertyName("allow");
                            JsonSerializer.Serialize(json, allow.Value, _jsonOptions);
                        }

                        if (deny.HasValue)
                        {
                            json.WritePropertyName("deny");
                            JsonSerializer.Serialize(json, deny.Value, _jsonOptions);
                        }

                        if (type.HasValue)
                        {
                            json.WriteString("type", type.Value.ToString().ToLowerInvariant());
                        }
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IReadOnlyList<IInvite>>
            (
                $"channels/{channelID}/invites",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<ICreateRestEntityResult<IInvite>> CreateChannelInviteAsync
        (
            Snowflake channelID,
            Optional<TimeSpan> maxAge = default,
            Optional<int> maxUses = default,
            Optional<bool> isTemporary = default,
            Optional<bool> isUnique = default,
            Optional<Snowflake> targetUser = default,
            Optional<TargetUserType> targetUserType = default,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PostAsync<IInvite>
            (
                $"channels/{channelID}/invites",
                b => b.WithJson
                (
                    json =>
                    {
                        if (maxAge.HasValue)
                        {
                            json.WriteNumber("max_age", maxAge.Value.TotalSeconds);
                        }

                        if (maxUses.HasValue)
                        {
                            json.WriteNumber("max_uses", maxUses.Value);
                        }

                        if (isTemporary.HasValue)
                        {
                            json.WriteBoolean("temporary", isTemporary.Value);
                        }

                        if (isUnique.HasValue)
                        {
                            json.WriteBoolean("unique", isUnique.Value);
                        }

                        if (targetUser.HasValue)
                        {
                            json.WriteString("target_user", targetUser.Value.ToString());
                        }

                        if (targetUserType.HasValue)
                        {
                            json.WriteNumber("target_user_type", (int)targetUserType.Value);
                        }
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteChannelPermissionAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/permissions/{overwriteID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRestResult> TriggerTypingIndicatorAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PostAsync
            (
                $"channels/{channelID}/typing",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IReadOnlyList<IMessage>>
            (
                $"channels/{channelID}/pins",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRestResult> AddPinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PutAsync
            (
                $"channels/{channelID}/pins/{messageID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeletePinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/pins/{messageID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRestResult> GroupDMAddRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            string accessToken,
            Optional<string> nickname = default,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PutAsync
            (
                $"channels/{channelID}/recipients/{userID}",
                b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("access_token", accessToken);

                        if (nickname.HasValue)
                        {
                            json.WriteString("nick", nickname.Value);
                        }
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> GroupDMRemoveRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync
            (
                $"channels/{channelID}/recipients/{userID}",
                ct: ct
            );
        }
    }
}
