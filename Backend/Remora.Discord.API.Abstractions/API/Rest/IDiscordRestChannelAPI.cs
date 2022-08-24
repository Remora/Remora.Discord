//
//  IDiscordRestChannelAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

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
    Task<Result<IChannel>> GetChannelAsync(Snowflake channelID, CancellationToken ct = default);

    /// <summary>
    /// Modifies the given channel.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="icon">The new icon.</param>
    /// <param name="type">
    /// The new type of the channel. Only conversions between <see cref="ChannelType.GuildText"/> and
    /// <see cref="ChannelType.GuildAnnouncement"/> are supported.
    /// </param>
    /// <param name="position">The new position of the channel in the listing.</param>
    /// <param name="topic">The new topic of the channel.</param>
    /// <param name="isNsfw">The new NSFW status of the channel.</param>
    /// <param name="rateLimitPerUser">The new rate limit per user.</param>
    /// <param name="bitrate">The new bitrate.</param>
    /// <param name="userLimit">The new user limit.</param>
    /// <param name="permissionOverwrites">The new permission overwrites.</param>
    /// <param name="parentID">The new parent category ID.</param>
    /// <param name="videoQualityMode">The new video quality mode.</param>
    /// <param name="isArchived">Whether the thread is archived.</param>
    /// <param name="autoArchiveDuration">The time of inactivity after which the thread is archived.</param>
    /// <param name="isLocked">Whether the thread is locked.</param>
    /// <param name="isInvitable">The value indicating whether non-moderators can add other non-moderators to the private thread.</param>
    /// <param name="defaultAutoArchiveDuration">
    /// The default time of inactivity after which threads in the channel are archived.
    /// </param>
    /// <param name="rtcRegion">The channel's voice region. Automatic when null.</param>
    /// <param name="flags">The channel flags to use.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<Stream> icon = default,
        Optional<ChannelType> type = default,
        Optional<int?> position = default,
        Optional<string?> topic = default,
        Optional<bool?> isNsfw = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<int?> bitrate = default,
        Optional<int?> userLimit = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<VideoQualityMode?> videoQualityMode = default,
        Optional<bool> isArchived = default,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<bool> isLocked = default,
        Optional<bool> isInvitable = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string?> rtcRegion = default,
        Optional<ChannelFlags> flags = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given group DM channel.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="icon">The new icon.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyGroupDMChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<Stream> icon = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given guild text channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="type">
    /// The new type of the channel. Only conversions between <see cref="ChannelType.GuildText"/> and
    /// <see cref="ChannelType.GuildAnnouncement"/> are supported.
    /// </param>
    /// <param name="position">The new position of the channel in the listing.</param>
    /// <param name="topic">The new topic of the channel.</param>
    /// <param name="isNsfw">The new NSFW status of the channel.</param>
    /// <param name="rateLimitPerUser">The new rate limit per user.</param>
    /// <param name="permissionOverwrites">The new permission overwrites.</param>
    /// <param name="parentID">The new parent category ID.</param>
    /// <param name="defaultAutoArchiveDuration">
    /// The default time of inactivity after which threads in the channel are archived.
    /// </param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyGuildTextChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<ChannelType> type = default,
        Optional<int?> position = default,
        Optional<string?> topic = default,
        Optional<bool?> isNsfw = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given guild voice channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="position">The new position of the channel in the listing.</param>
    /// <param name="isNsfw">The new NSFW status of the channel.</param>
    /// <param name="bitrate">The new bitrate.</param>
    /// <param name="userLimit">The new user limit.</param>
    /// <param name="permissionOverwrites">The new permission overwrites.</param>
    /// <param name="parentID">The new parent category ID.</param>
    /// <param name="rtcRegion">The channel's voice region. Automatic when null.</param>
    /// <param name="videoQualityMode">The new video quality mode.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyGuildVoiceChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<int?> position = default,
        Optional<bool?> isNsfw = default,
        Optional<int?> bitrate = default,
        Optional<int?> userLimit = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<string?> rtcRegion = default,
        Optional<VideoQualityMode?> videoQualityMode = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given guild stage channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="position">The new position of the channel in the listing.</param>
    /// <param name="bitrate">The new bitrate.</param>
    /// <param name="permissionOverwrites">The new permission overwrites.</param>
    /// <param name="rtcRegion">The channel's voice region. Automatic when null.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyGuildStageChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<int?> position = default,
        Optional<int?> bitrate = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<string?> rtcRegion = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given guild announcement channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="type">
    /// The new type of the channel. Only conversions between <see cref="ChannelType.GuildText"/> and
    /// <see cref="ChannelType.GuildAnnouncement"/> are supported.
    /// </param>
    /// <param name="position">The new position of the channel in the listing.</param>
    /// <param name="topic">The new topic of the channel.</param>
    /// <param name="isNsfw">The new NSFW status of the channel.</param>
    /// <param name="permissionOverwrites">The new permission overwrites.</param>
    /// <param name="parentID">The new parent category ID.</param>
    /// <param name="defaultAutoArchiveDuration">
    /// The default time of inactivity after which threads in the channel are archived.
    /// </param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyGuildAnnouncementChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<ChannelType> type = default,
        Optional<int?> position = default,
        Optional<string?> topic = default,
        Optional<bool?> isNsfw = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="name">The new name of the channel.</param>
    /// <param name="isArchived">Whether the thread is archived.</param>
    /// <param name="autoArchiveDuration">The time of inactivity after which the thread is archived.</param>
    /// <param name="isLocked">Whether the thread is locked.</param>
    /// <param name="isInvitable">The value indicating whether non-moderators can add other non-moderators to the private thread.</param>
    /// <param name="rateLimitPerUser">The new rate limit per user.</param>
    /// <param name="flags">The channel flags to use.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> ModifyThreadChannelAsync
    (
        Snowflake channelID,
        Optional<string> name = default,
        Optional<bool> isArchived = default,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<bool> isLocked = default,
        Optional<bool> isInvitable = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<ChannelFlags> flags = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes a channel by its ID.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result> DeleteChannelAsync
    (
        Snowflake channelID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

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
    Task<Result<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
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
    Task<Result<IMessage>> GetChannelMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Posts a message to the given channel.
    /// <remarks>
    /// At least one of <paramref name="content"/>, <paramref name="embeds"/>, or <paramref name="attachments"/> must be
    /// present.
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="nonce">A nonce that can be used for optimistic message sending.</param>
    /// <param name="isTTS">Whether the message is a TTS message.</param>
    /// <param name="embeds">The rich embeds in the message.</param>
    /// <param name="allowedMentions">An object describing the allowed mention types.</param>
    /// <param name="messageReference">A reference to another message.</param>
    /// <param name="components">The components of the message.</param>
    /// <param name="stickerIDs">The stickers to send with the message (max 3).</param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="flags">The message flags.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> CreateMessageAsync
    (
        Snowflake channelID,
        Optional<string> content = default,
        Optional<string> nonce = default,
        Optional<bool> isTTS = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        Optional<IAllowedMentions> allowedMentions = default,
        Optional<IMessageReference> messageReference = default,
        Optional<IReadOnlyList<IMessageComponent>> components = default,
        Optional<IReadOnlyList<Snowflake>> stickerIDs = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        Optional<MessageFlags> flags = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Crosspost a message otherwise known as "publishing" a message from a announcement channel that other guilds can follow.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> CrosspostMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
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
    Task<Result> CreateReactionAsync
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
    Task<Result> DeleteOwnReactionAsync
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
    Task<Result> DeleteUserReactionAsync
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
    /// <param name="after">The users to get after this user ID.</param>
    /// <param name="limit">The maximum page size.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IUser>>> GetReactionsAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
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
    Task<Result> DeleteAllReactionsAsync
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
    Task<Result> DeleteAllReactionsForEmojiAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string emoji,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits a previously sent message.
    /// </summary>
    /// <remarks>
    /// At least one of <paramref name="content"/>, <paramref name="embeds"/>, or <paramref name="attachments"/> must be
    /// present.
    ///
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="channelID">The ID of the channel the message is in.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="content">The new content of the message.</param>
    /// <param name="embeds">The new embeds associated with the message.</param>
    /// <param name="flags">The new message flags.</param>
    /// <param name="allowedMentions">The allowed mentions for the message.</param>
    /// <param name="components">The components of the message.</param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> EditMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<MessageFlags?> flags = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given message.
    /// </summary>
    /// <param name="channelID">The ID of the channel the message is in.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string> reason = default,
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
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> BulkDeleteMessagesAsync
    (
        Snowflake channelID,
        IReadOnlyList<Snowflake> messageIDs,
        Optional<string> reason = default,
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
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result> EditChannelPermissionsAsync
    (
        Snowflake channelID,
        Snowflake overwriteID,
        Optional<IDiscordPermissionSet?> allow = default,
        Optional<IDiscordPermissionSet?> deny = default,
        Optional<PermissionOverwriteType> type = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the invites for a given channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
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
    /// <param name="targetType">The target type for this invite.</param>
    /// <param name="targetUserID">The target user ID for this invite.</param>
    /// <param name="targetApplicationID">
    /// The ID of the application to open for this invite. Required if <paramref name="targetType"/> is
    /// <see cref="InviteTarget.EmbeddedApplication"/>.
    /// </param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IInvite>> CreateChannelInviteAsync
    (
        Snowflake channelID,
        Optional<TimeSpan> maxAge = default,
        Optional<int> maxUses = default,
        Optional<bool> isTemporary = default,
        Optional<bool> isUnique = default,
        Optional<InviteTarget> targetType = default,
        Optional<Snowflake> targetUserID = default,
        Optional<Snowflake> targetApplicationID = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given permission overwrite from the given channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="overwriteID">The ID of the permission overwrite.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteChannelPermissionAsync
    (
        Snowflake channelID,
        Snowflake overwriteID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Follows a announcement channel to send messages to a target channel.
    /// </summary>
    /// <param name="channelID">The ID of the announcement channel.</param>
    /// <param name="webhookChannelID">The ID of the channel to send announcement to.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IFollowedChannel>> FollowAnnouncementChannelAsync
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
    Task<Result> TriggerTypingIndicatorAsync
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
    Task<Result<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Pins the given message in the channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> PinMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Unpins the given message in the channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> UnpinMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        Optional<string> reason = default,
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
    Task<Result> GroupDMAddRecipientAsync
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
    Task<Result> GroupDMRemoveRecipientAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Starts a new public thread from an existing message.
    /// </summary>
    /// <param name="channelID">The channel to start the thread in.</param>
    /// <param name="messageID">The message to start the thread from.</param>
    /// <param name="name">The name of the thread.</param>
    /// <param name="autoArchiveDuration">The time of inactivity after which to archive the thread.</param>
    /// <param name="rateLimitPerUser">
    /// The message rate limit per user, that is, the number of seconds they have to wait between sending messages.
    /// </param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> StartThreadWithMessageAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        string name,
        Optional<AutoArchiveDuration> autoArchiveDuration,
        Optional<int?> rateLimitPerUser = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Starts a new private thread.
    /// </summary>
    /// <param name="channelID">The channel to start the thread in.</param>
    /// <param name="name">The name of the thread.</param>
    /// <param name="type">
    /// The thread type to create. Discord defaults to creating a <see cref="ChannelType.PrivateThread"/>,
    /// but this is likely to change in a future API version. Prefer always setting this explicitly.</param>
    /// <param name="autoArchiveDuration">The time of inactivity after which to archive the thread.</param>
    /// <param name="isInvitable">The value indicating whether non-moderators can add other non-moderators to the private thread.</param>
    /// <param name="rateLimitPerUser">
    /// The message rate limit per user, that is, the number of seconds they have to wait between sending messages.
    /// </param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> StartThreadWithoutMessageAsync
    (
        Snowflake channelID,
        string name,
        ChannelType type,
        Optional<AutoArchiveDuration> autoArchiveDuration = default,
        Optional<bool> isInvitable = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Joins the given thread.
    /// </summary>
    /// <param name="channelID">The thread to join.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> JoinThreadAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Adds the given user to the given thread.
    /// </summary>
    /// <param name="channelID">The thread to add the user to.</param>
    /// <param name="userID">The user to add to the thread.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> AddThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Leaves the given thread.
    /// </summary>
    /// <param name="channelID">The thread to leave.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> LeaveThreadAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Removes the given user from the given thread.
    /// </summary>
    /// <param name="channelID">The thread to remove the user from.</param>
    /// <param name="userID">The user to remove from the thread.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> RemoveThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a thread member object for the specified user if they're a member of the thread.
    /// </summary>
    /// <param name="channelID">The ID of the thread.</param>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IThreadMember>> GetThreadMemberAsync
    (
        Snowflake channelID,
        Snowflake userID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Lists the members of the given thread. Restricted to bots with with GuildMembers intent.
    /// </summary>
    /// <param name="channelID">The thread to list the members of.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IThreadMember>>> ListThreadMembersAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the public archived threads for a channel.
    /// </summary>
    /// <param name="channelID">The channel to list the threads of.</param>
    /// <param name="before">The timestamp to return threads before of.</param>
    /// <param name="limit">The maximum number of threads to retrieve.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IChannelThreadQueryResponse>> ListPublicArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<DateTimeOffset> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the private archived threads for a channel.
    /// </summary>
    /// <param name="channelID">The channel to list the threads of.</param>
    /// <param name="before">The timestamp to return threads before of.</param>
    /// <param name="limit">The maximum number of threads to retrieve.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IChannelThreadQueryResponse>> ListPrivateArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<DateTimeOffset> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the private archived threads for a channel that the user has joined.
    /// </summary>
    /// <param name="channelID">The channel to list the threads of.</param>
    /// <param name="before">The snowflake to return threads before of.</param>
    /// <param name="limit">The maximum number of threads to retrieve.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IChannelThreadQueryResponse>> ListJoinedPrivateArchivedThreadsAsync
    (
        Snowflake channelID,
        Optional<Snowflake> before = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    );
}
