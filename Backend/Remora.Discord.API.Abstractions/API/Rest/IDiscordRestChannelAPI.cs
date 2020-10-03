//
//  IDiscordRestChannelAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Rest
{
    /// <summary>
    /// Represents the Discord Channel API.
    /// </summary>
    [PublicAPI]
    public interface IDiscordRestChannelAPI
    {
        /// <summary>
        /// Gets a channel by its ID.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IChannel>> GetChannelAsync(Snowflake channelID, CancellationToken ct = default);

        /// <summary>
        /// Modifies the given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="name">The new name of the channel.</param>
        /// <param name="type">
        /// The new type of the channel. Only conversions between <see cref="ChannelType.GuildText"/> and
        /// <see cref="ChannelType.GuildNews"/> are supported.
        /// </param>
        /// <param name="position">The new position of the channel in the listing.</param>
        /// <param name="topic">The new topic of the channel.</param>
        /// <param name="isNsfw">The new NSFW status of the channel.</param>
        /// <param name="rateLimitPerUser">The new rate limit per user.</param>
        /// <param name="bitrate">The new bitrate.</param>
        /// <param name="userLimit">The new user limit.</param>
        /// <param name="permissionOverwrites">The new permission overwrites.</param>
        /// <param name="parentId">The new parent category ID.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IChannel>> ModifyChannelAsync
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
        );

        /// <summary>
        /// Deletes a channel by its ID.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteChannelAsync(Snowflake channelID, CancellationToken ct = default);

        /// <summary>
        /// Gets the messages for a channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="around">
        /// The ID of the message to get messages around. This is a mutually exclusive option with <paramref name="before"/>
        /// and <paramref name="after"/>.
        /// </param>
        /// <param name="before">
        /// The ID of the message to get messages before. This is a mutually exclusive option with <paramref name="around"/>
        /// and <paramref name="after"/>.
        /// </param>
        /// <param name="after">
        /// The ID of the message to get messages after. This is a mutually exclusive option with <paramref name="before"/>
        /// and <paramref name="around"/>.
        /// </param>
        /// <param name="limit">The maximum number of messages to retrieve. Ranges between 1 and 100.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
        (
            Snowflake channelID,
            Optional<Snowflake> around = default,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets a message from a channel by its ID.
        /// </summary>
        /// <param name="channelID">The channel ID.</param>
        /// <param name="messageID">The message ID.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IMessage>> GetChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Posts a message to the given channel.
        /// <remarks>
        /// At least one of <paramref name="content"/>, <paramref name="embed"/>, or <paramref name="file"/> must be present.
        /// </remarks>
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="nonce">A nonce that can be used for optimistic message sending.</param>
        /// <param name="isTTS">Whether the message is a TTS message.</param>
        /// <param name="file">The contents of the file to upload.</param>
        /// <param name="embed">The rich embed in the message.</param>
        /// <param name="allowedMentions">An object describing the allowed mention types.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IMessage>> CreateMessageAsync
        (
            Snowflake channelID,
            Optional<string> content = default,
            Optional<string> nonce = default,
            Optional<bool> isTTS = default,
            Optional<Stream> file = default,
            Optional<IEmbed> embed = default,
            Optional<IAllowedMentions> allowedMentions = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Adds a reaction to the given message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="emoji">The emoji to react with.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<IRestResult> CreateReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes a reaction from the given message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="emoji">The emoji to remove.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteOwnReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes a reaction from the given message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="emoji">The emoji to remove.</param>
        /// <param name="user">The user that has reacted with the emoji.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteUserReactionAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Snowflake user,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets a list of users that have reacted with the given emoji.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="emoji">The emoji to filter on.</param>
        /// <param name="before">The users to get before this user ID.</param>
        /// <param name="after">The users to get after this user ID.</param>
        /// <param name="limit">The maximum page size.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IUser>>> GetReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes all reactions from the given message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteAllReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes all reactions from the given message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="emoji">The emoji to delete.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteAllReactionsForEmojiAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            CancellationToken ct = default
        );

        /// <summary>
        /// Edits a previously sent message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="content">The new content of the message.</param>
        /// <param name="embed">The new embed associated with the message.</param>
        /// <param name="flags">The new message flags.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IMessage>> EditMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IEmbed?> embed = default,
            Optional<MessageFlags?> flags = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes the given message.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Bulk deletes a set of messages.
        /// </summary>
        /// <param name="channelID">The ID of the channel to delete messages in.</param>
        /// <param name="messageIDs">
        /// The IDs of the messages to delete. Messages older than 2 weeks may not be deleted in this manner. Any
        /// invalid message IDs will count towards the minimum and maximum number of messages to deleted (currently 2
        /// and 100, respectively).
        /// </param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> BulkDeleteMessagesAsync
        (
            Snowflake channelID,
            IReadOnlyList<Snowflake> messageIDs,
            CancellationToken ct = default
        );

        /// <summary>
        /// Edits a permission overwrite on the given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="overwriteID">The ID of the overwrite.</param>
        /// <param name="allow">The permissions to allow.</param>
        /// <param name="deny">The permissions to deny.</param>
        /// <param name="type">The new type of the overwrite.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<IRestResult> EditChannelPermissionsAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            Optional<IDiscordPermissionSet> allow = default,
            Optional<IDiscordPermissionSet> deny = default,
            Optional<PermissionOverwriteType> type = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the invites for a given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Creates an invite to the given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="maxAge">The duration of the invite before expiry, or 0 for never.</param>
        /// <param name="maxUses">The max number of uses, or 0 for unlimited.</param>
        /// <param name="isTemporary">Whether this invite grants temporary membership.</param>
        /// <param name="isUnique">If true, don't try to reuse an existing invite with the same settings.</param>
        /// <param name="targetUser">The target user ID for this invite.</param>
        /// <param name="targetUserType">The target user type for this invite.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IInvite>> CreateChannelInviteAsync
        (
            Snowflake channelID,
            Optional<TimeSpan> maxAge = default,
            Optional<int> maxUses = default,
            Optional<bool> isTemporary = default,
            Optional<bool> isUnique = default,
            Optional<Snowflake> targetUser = default,
            Optional<TargetUserType> targetUserType = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes the given permission overwrite from the given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="overwriteID">The ID of the permission overwrite.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteChannelPermissionAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Follows a news channel to send messages to a target channel.
        /// </summary>
        /// <param name="channelID">The ID of the news channel.</param>
        /// <param name="webhookChannelID">The ID of the channel to send news to.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IFollowedChannel>> FollowNewsChannelAsync
        (
            Snowflake channelID,
            Snowflake webhookChannelID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Triggers the typing indicator for the current user in the given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> TriggerTypingIndicatorAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets the pinned messages in the given channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Pins the given message in the channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> AddPinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Unpins the given message in the channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeletePinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Adds a recipient to a group DM using their access token.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="nickname">The nickname of the user to use in the group DM.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IRestResult> GroupDMAddRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            string accessToken,
            Optional<string> nickname = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Removes a recipient from a group DM.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> GroupDMRemoveRecipientAsync
        (
            Snowflake channelID,
            Snowflake userID,
            CancellationToken ct = default
        );
    }
}
