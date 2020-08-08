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
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;

namespace Remora.Discord.Rest.API.Gateway
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IModifyRestEntityResult<IChannel>> ModifyChannelAsync
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteChannelAsync
        (
            Snowflake channelID,
            CancellationToken ct
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
        (
            Snowflake channelID,
            Optional<Snowflake> around = default,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IMessage>> GetChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
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

                    b.AddJsonConfigurator
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

                            if (allowedMentions.HasValue)
                            {
                                writer.WritePropertyName("allowed_mentions");
                                JsonSerializer.Serialize(writer, allowedMentions.Value, _jsonOptions);
                            }
                        }
                    );
                },
                ct
            );
        }

        /// <inheritdoc />
        public Task<ICreateRestEntityResult<IReaction>> CreateReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<IUser>>> GetReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteAllReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> BulkDeleteMessagesAsync
        (
            Snowflake channelID,
            IReadOnlyList<Snowflake> messageIDs,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IModifyRestEntityResult<IPermissionOverwrite>> EditChannelPermissionsAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            Optional<DiscordPermission> allow = default,
            Optional<DiscordPermission> deny = default,
            Optional<PermissionOverwriteType> type = default,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ICreateRestEntityResult<IInvite>> CreateChannelInviteAsync
        (
            Snowflake channelID,
            Optional<DateTime> maxAge = default,
            Optional<int> maxUses = default,
            Optional<bool> isTemporary = default,
            Optional<bool> isUnique = default,
            Optional<Snowflake> targetUser = default,
            Optional<TargetUserType> targetUserType = default,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeleteChannelPermissionAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRestResult> TriggerTypingIndicatorAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IRestResult> AddPinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> DeletePinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult> GroupDMRemoveRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            throw new NotImplementedException();
        }
    }
}
