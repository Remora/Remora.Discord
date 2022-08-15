//
//  DiscordError.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Abstractions.Results;

/// <summary>
/// Enumerates the various Discord error codes.
/// </summary>
[PublicAPI]
public enum DiscordError
{
    /// <summary>
    /// An unknown error code.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// A general error (such as a malformed request body, amongst other things).
    /// </summary>
    GeneralError = 0,

    /// <summary>
    /// Unknown account.
    /// </summary>
    UnknownAccount = 10001,

    /// <summary>
    /// Unknown application.
    /// </summary>
    UnknownApplication = 10002,

    /// <summary>
    /// Unknown channel.
    /// </summary>
    UnknownChannel = 10003,

    /// <summary>
    /// Unknown guild.
    /// </summary>
    UnknownGuild = 10004,

    /// <summary>
    /// Unknown integration.
    /// </summary>
    UnknownIntegration = 10005,

    /// <summary>
    /// Unknown invite.
    /// </summary>
    UnknownInvite = 10006,

    /// <summary>
    /// Unknown member.
    /// </summary>
    UnknownMember = 10007,

    /// <summary>
    /// Unknown message.
    /// </summary>
    UnknownMessage = 10008,

    /// <summary>
    /// Unknown permission overwrite.
    /// </summary>
    UnknownPermissionOverwrite = 10009,

    /// <summary>
    /// Unknown provider.
    /// </summary>
    UnknownProvider = 10010,

    /// <summary>
    /// Unknown role.
    /// </summary>
    UnknownRole = 10011,

    /// <summary>
    /// Unknown token.
    /// </summary>
    UnknownToken = 10012,

    /// <summary>
    /// Unknown user.
    /// </summary>
    UnknownUser = 10013,

    /// <summary>
    /// Unknown emoji.
    /// </summary>
    UnknownEmoji = 10014,

    /// <summary>
    /// Unknown webhook.
    /// </summary>
    UnknownWebhook = 10015,

    /// <summary>
    /// Unknown webhook service.
    /// </summary>
    UnknownWebhookService = 10016,

    /// <summary>
    /// Unknown session.
    /// </summary>
    UnknownSession = 10020,

    /// <summary>
    /// Unknown ban.
    /// </summary>
    UnknownBan = 10026,

    /// <summary>
    /// Unknown stock keeping unit.
    /// </summary>
    UnknownSKU = 10027,

    /// <summary>
    /// Unknown store listing.
    /// </summary>
    UnknownStoreListing = 10028,

    /// <summary>
    /// Unknown entitlement.
    /// </summary>
    UnknownEntitlement = 10029,

    /// <summary>
    /// Unknown build.
    /// </summary>
    UnknownBuild = 10030,

    /// <summary>
    /// Unknown lobby.
    /// </summary>
    UnknownLobby = 10031,

    /// <summary>
    /// Unknown branch.
    /// </summary>
    UnknownBranch = 10032,

    /// <summary>
    /// Unknown store directory layout.
    /// </summary>
    UnknownStoreDirectoryLayout = 10033,

    /// <summary>
    /// Unknown redistributable.
    /// </summary>
    UnknownRedistributable = 10036,

    /// <summary>
    /// Unknown gift code.
    /// </summary>
    UnknownGiftCode = 10038,

    /// <summary>
    /// Unknown stream.
    /// </summary>
    UnknownStream = 10049,

    /// <summary>
    /// Unknown premium server subscription cooldown.
    /// </summary>
    UnknownPremiumServerSubscriptionCooldown = 10050,

    /// <summary>
    /// Unknown guild template.
    /// </summary>
    UnknownGuildTemplate = 10057,

    /// <summary>
    /// Unknown discoverable server category.
    /// </summary>
    UnknownDiscoverableServerCategory = 10059,

    /// <summary>
    /// Unknown sticker.
    /// </summary>
    UnknownSticker = 10060,

    /// <summary>
    /// Unknown interaction.
    /// </summary>
    UnknownInteraction = 10062,

    /// <summary>
    /// Unknown application command.
    /// </summary>
    UnknownApplicationCommand = 10063,

    /// <summary>
    /// Unknown voice state.
    /// </summary>
    UnknownVoiceState = 10065,

    /// <summary>
    /// Unknown application command permissions.
    /// </summary>
    UnknownApplicationCommandPermissions = 10066,

    /// <summary>
    /// Unknown stage instance.
    /// </summary>
    UnknownStageInstance = 10067,

    /// <summary>
    /// Unknown guild member verification form.
    /// </summary>
    UnknownGuildMemberVerificationForm = 10068,

    /// <summary>
    /// Unknown guild welcome screen.
    /// </summary>
    UnknownGuildWelcomeScreen = 10069,

    /// <summary>
    /// Unknown guild scheduled event.
    /// </summary>
    UnknownGuildScheduledEvent = 10070,

    /// <summary>
    /// Unknown guild scheduled event user.
    /// </summary>
    UnknownGuildScheduledEventUser = 10071,

    /// <summary>
    /// Bots cannot use this endpoint.
    /// </summary>
    NoBotsAllowed = 20001,

    /// <summary>
    /// Only bots can use this endpoint.
    /// </summary>
    BotsOnly = 20002,

    /// <summary>
    /// Explicit content cannot be sent to the intended recipient(s).
    /// </summary>
    ExplicitContentCannotBeSent = 20009,

    /// <summary>
    /// You are not allowed to perform that operation on this application.
    /// </summary>
    ApplicationActionUnauthorized = 20012,

    /// <summary>
    /// This action cannot be performed due to slow mode.
    /// </summary>
    SlowMode = 20016,

    /// <summary>
    /// Only the owner of this account can perform that action.
    /// </summary>
    OwnerOnly = 20018,

    /// <summary>
    /// This message cannot be edited due to announcement rate limits.
    /// </summary>
    EditingNotAllowedDueToAnnouncementRateLimits = 20022,

    /// <summary>
    /// The user is below the minimum required age for the attempted operation.
    /// </summary>
    UnderMinimumAge = 20024,

    /// <summary>
    /// The channel you are writing to has hit the write rate limit.
    /// </summary>
    WriteRateLimitHit = 20028,

    /// <summary>
    /// The write action you are performing on the server has hit the write rate limit.
    /// </summary>
    ServerWriteRateLimitHit = 20029,

    /// <summary>
    /// Your stage channel topic, server name, description, or channel name contains disallowed words.
    /// </summary>
    DisallowedWords = 20031,

    /// <summary>
    /// The guild's premium subscription level is too low for the requested action.
    /// </summary>
    GuildPremiumLevelTooLow = 20035,

    /// <summary>
    /// Maximum number of guilds reached (100).
    /// </summary>
    MaxGuildsReached = 30001,

    /// <summary>
    /// Maximum number of friends reached (1000).
    /// </summary>
    MaxFriendsReached = 30002,

    /// <summary>
    /// Maximum number of pins reached for the channel (50).
    /// </summary>
    MaxPinsInChannelReached = 30003,

    /// <summary>
    /// Maximum number of recipients reached.
    /// </summary>
    MaxRecipientsReached = 30004,

    /// <summary>
    /// Maximum number of guild roles reached (250).
    /// </summary>
    MaxGuildRolesReached = 30005,

    /// <summary>
    /// Maximum number of webhooks reached (10).
    /// </summary>
    MaxWebhooksReached = 30007,

    /// <summary>
    /// Maximum number of emojis reached.
    /// </summary>
    MaxEmojisReached = 30008,

    /// <summary>
    /// Maximum number of reactions reached (20).
    /// </summary>
    MaxReactionsReached = 30010,

    /// <summary>
    /// Maximum number of guild channels reached (500).
    /// </summary>
    MaxGuildChannelsReached = 30013,

    /// <summary>
    /// Maximum number of attachments in a message reached (10).
    /// </summary>
    MaxAttachmentsInMessageReached = 30015,

    /// <summary>
    /// Maximum number of invites reached (1000).
    /// </summary>
    MaxGuildInvitesReached = 30016,

    /// <summary>
    /// Maximum number of animated emojis reached.
    /// </summary>
    MaxAnimatedEmojisReached = 30018,

    /// <summary>
    /// Maximum number of server members reached.
    /// </summary>
    MaxServerMembersReached = 30019,

    /// <summary>
    /// Maximum number of server categories reached (5).
    /// </summary>
    MaxServerCategoriesReached = 30030,

    /// <summary>
    /// The guild already has a template.
    /// </summary>
    GuildAlreadyHasATemplate = 30031,

    /// <summary>
    /// The maximum number of registered application commands has been reached.
    /// </summary>
    MaximumNumberOfApplicationCommandsReached = 30032,

    /// <summary>
    /// The maximum number of participants in a thread has been reached (1000).
    /// </summary>
    MaxNumberOfThreadParticipantsReached = 30033,

    /// <summary>
    /// The maximum number of application command create calls has been reached (200).
    /// </summary>
    MaxNumberOfDailyApplicationCommandCreationsReached = 30034,

    /// <summary>
    /// The maximum number of bans for non-guild members has been exceeded.
    /// </summary>
    MaxNumberOfNonMemberBansReached = 30035,

    /// <summary>
    /// The maximum number of ban fetches has been exceeded.
    /// </summary>
    MaxNumberOfBanFetchesReached = 30037,

    /// <summary>
    /// The maximum number of uncompleted scheduled events has been reached (100).
    /// </summary>
    MaxNumberOfUncompletedGuildScheduledEventsReached = 30038,

    /// <summary>
    /// Maximum number of stickers reached.
    /// </summary>
    MaxStickersReached = 30039,

    /// <summary>
    /// Maximum number of prune requests has been reached. Try again later.
    /// </summary>
    MaxNumberOfPruneRequestsReached = 30040,

    /// <summary>
    /// Maximum number of guild widget setting updates has been reached. Try again later.
    /// </summary>
    MaxNumberOfGuildWidgetSettingUpdatesReached = 30042,

    /// <summary>
    /// Maximum number of edits to messages older than 1 hour reached. Try again later.
    /// </summary>
    MaxNumberOfEditsToMessagesOlderThanOneHourReached = 30046,

    /// <summary>
    /// The maximum number of pinned threads in the forum channel has been reached.
    /// </summary>
    MaxNumberOfPinnedThreadsInForumChannelReached = 30047,

    /// <summary>
    /// The maximum number of tags in the forum channel has been reached.
    /// </summary>
    MaxNumberOfTagsInForumChannelReached = 30048,

    /// <summary>
    /// The requested bitrate is too high for a channel of this type.
    /// </summary>
    BitrateTooHighForChannelType = 30052,

    /// <summary>
    /// Unauthorized. Provide a valid token and try again.
    /// </summary>
    Unauthorized = 40001,

    /// <summary>
    /// You need to verify your account in order to perform this action.
    /// </summary>
    UnverifiedAccount = 40002,

    /// <summary>
    /// You are opening direct messages too fast.
    /// </summary>
    OpeningDMsTooFast = 40003,

    /// <summary>
    /// Your ability to send messages has been temporarily disabled.
    /// </summary>
    SendMessagesHasBeenTemporarilyDisabled = 40004,

    /// <summary>
    /// Request entity too large. Try sending something smaller in size.
    /// </summary>
    RequestEntityTooLarge = 40005,

    /// <summary>
    /// This feature has been temporarily disabled server-side.
    /// </summary>
    FeatureTemporarilyDisabled = 40006,

    /// <summary>
    /// The user is banned from this guild.
    /// </summary>
    UserBanned = 40007,

    /// <summary>
    /// The connection has been revoked.
    /// </summary>
    ConnectionRevoked = 40012,

    /// <summary>
    /// The target user is not connected to voice.
    /// </summary>
    UserNotInVoice = 40032,

    /// <summary>
    /// This message has already been crossposted.
    /// </summary>
    MessageAlreadyCrossposted = 40033,

    /// <summary>
    /// An application command with that name already exists.
    /// </summary>
    ApplicationCommandWithNameExists = 40041,

    /// <summary>
    /// An application interaction failed to send for some reason.
    /// </summary>
    ApplicationInteractionFailedToSend = 40034,

    /// <summary>
    /// The interaction has already been acknowledged by the application.
    /// </summary>
    InteractionHasAlreadyBeenAcknowledged = 40060,

    /// <summary>
    /// Missing access.
    /// </summary>
    MissingAccess = 50001,

    /// <summary>
    /// Invalid account type.
    /// </summary>
    InvalidAccountType = 50002,

    /// <summary>
    /// Cannot execute action on a DM channel.
    /// </summary>
    CannotExecuteActionOnDMChannel = 50003,

    /// <summary>
    /// Guild widget disabled.
    /// </summary>
    GuildWidgetDisabled = 50004,

    /// <summary>
    /// Cannot edit a message authored by another user.
    /// </summary>
    CannotEditMessageByAnotherUser = 50005,

    /// <summary>
    /// Cannot send an empty message.
    /// </summary>
    CannotSendEmptyMessage = 50006,

    /// <summary>
    /// Cannot send messages to this user.
    /// </summary>
    CannotSendMessageToUser = 50007,

    /// <summary>
    /// Cannot send messages in a non-text channel.
    /// </summary>
    CannotSendMessageToNonTextChannel = 50008,

    /// <summary>
    /// Channel verification level is too high for you to gain access.
    /// </summary>
    ChannelVerificationLevelTooHighForAccess = 50009,

    /// <summary>
    /// OAuth2 application does not have a bot.
    /// </summary>
    OAuthApplicationDoesNotHaveBot = 50010,

    /// <summary>
    /// OAuth2 application limit reached.
    /// </summary>
    OAuthApplicationLimitReached = 50011,

    /// <summary>
    /// Invalid OAuth2 state.
    /// </summary>
    InvalidOAuthState = 50012,

    /// <summary>
    /// You lack permissions to perform that action.
    /// </summary>
    MissingPermission = 50013,

    /// <summary>
    /// Invalid authentication token provided.
    /// </summary>
    InvalidAuthenticationToken = 50014,

    /// <summary>
    /// Note was too long.
    /// </summary>
    NoteTooLong = 50015,

    /// <summary>
    /// Provided too few or too many messages to delete. Must provide at least 2 and fewer than 100 messages to
    /// delete.
    /// </summary>
    TooFewOrTooManyMessagesToDelete = 50016,

    /// <summary>
    /// The provided MFA level was invalid.
    /// </summary>
    InvalidMFALevel = 50017,

    /// <summary>
    /// A message and only be pinned  to the channel it was went in.
    /// </summary>
    MessageCanOnlyBePinnedInSameChannel = 50019,

    /// <summary>
    /// Invite code was either invalid or taken.
    /// </summary>
    InvalidOrUsedInviteCode = 50020,

    /// <summary>
    /// Cannot execute action on a system message.
    /// </summary>
    CannotExecuteActionOnSystemMessage = 50021,

    /// <summary>
    /// Cannot execute action on this channel type.
    /// </summary>
    CannotExecuteActionOnChannelType = 50024,

    /// <summary>
    /// Invalid OAuth2 access token provided.
    /// </summary>
    InvalidOAuth2Token = 50025,

    /// <summary>
    /// You are missing a required OAuth2 scope.
    /// </summary>
    MissingOAuth2Scope = 50026,

    /// <summary>
    /// Invalid webhook token provided.
    /// </summary>
    InvalidWebhookToken = 50027,

    /// <summary>
    /// Invalid role provided.
    /// </summary>
    InvalidRole = 50028,

    /// <summary>
    /// One or more recipients were invalid.
    /// </summary>
    InvalidRecipients = 50033,

    /// <summary>
    /// A message provided was too old to bulk delete.
    /// </summary>
    MessageTooOldToBulkDelete = 50034,

    /// <summary>
    /// Invalid form body (returned for both application/json and multipart/form-data bodies), or invalid
    /// Content-Type provided.
    /// </summary>
    InvalidFormBody = 50035,

    /// <summary>
    /// An invite was accepted to a guild the application's bot is not in.
    /// </summary>
    InviteAcceptedButBotIsNotInGuild = 50036,

    /// <summary>
    /// Invalid API version provided.
    /// </summary>
    InvalidAPIVersion = 50041,

    /// <summary>
    /// The uploaded file is greater than the maximum size allowed.
    /// </summary>
    FileUploadExceedsMaximumSize = 50045,

    /// <summary>
    /// The uploaded file is invalid.
    /// </summary>
    InvalidFileUploaded = 50046,

    /// <summary>
    /// You can't redeem this gift yourself.
    /// </summary>
    CannotSelfRedeemGift = 50054,

    /// <summary>
    /// Some guild is invalid.
    /// </summary>
    InvalidGuild = 50055,

    /// <summary>
    /// Some message type is invalid.
    /// </summary>
    InvalidMessageType = 50068,

    /// <summary>
    /// You need a payment source to redeem this gift.
    /// </summary>
    PaymentSourceRequiredForRedemption = 50070,

    /// <summary>
    /// A channel required for community guilds cannot be deleted.
    /// </summary>
    CannotDeleteCommunityRequiredChannel = 50074,

    /// <summary>
    /// Stickers sent in a message can't be edited.
    /// </summary>
    CannotEditStickersInMessage = 50080,

    /// <summary>
    /// Invalid sticker sent.
    /// </summary>
    InvalidSticker = 50081,

    /// <summary>
    /// You attempted to perform an operation on an archived thread that's not allowed.
    /// </summary>
    InvalidOperationOnArchivedThread = 50083,

    /// <summary>
    /// Invalid thread notification settings were provided.
    /// </summary>
    InvalidThreadNotificationSettings = 50084,

    /// <summary>
    /// The "before" value provided was earlier than the thread creation date.
    /// </summary>
    BeforeIsEarlierThanThreadCreation = 50085,

    /// <summary>
    /// Community server channels must be text channels.
    /// </summary>
    CommunityServerChannelMustBeTextChannel = 50086,

    /// <summary>
    /// This server is not available in your location.
    /// </summary>
    ServerUnavailableInYourLocation = 50095,

    /// <summary>
    /// This server must have monetization enabled to perform this action.
    /// </summary>
    ServerMonetizationRequired = 50097,

    /// <summary>
    /// This server needs more boosts to perform this action.
    /// </summary>
    ServerNeedsMoreBoosts = 50101,

    /// <summary>
    /// The request body contains invalid JSON.
    /// </summary>
    InvalidJSONInRequestBody = 50109,

    /// <summary>
    /// The ownership of the object cannot be transferred to a bot user.
    /// </summary>
    OwnershipCannotBeTransferredToBotUser = 50132,

    /// <summary>
    /// Failed to resize the uploaded asset below the maximum size (262144 bytes).
    /// </summary>
    FailedToResizeAsset = 50138,

    /// <summary>
    /// The uploaded file was not found.
    /// </summary>
    UploadedFileNotFound = 50146,

    /// <summary>
    /// You do not have permission to send this sticker.
    /// </summary>
    MissingPermissionToSendSticker = 50600,

    /// <summary>
    /// Two factor is required for this operation.
    /// </summary>
    TwoFactorRequired = 60003,

    /// <summary>
    /// No user with that tag exists.
    /// </summary>
    NoUserWithTag = 80004,

    /// <summary>
    /// Reaction was blocked.
    /// </summary>
    ReactionBlocked = 90001,

    /// <summary>
    /// The application isn't available yet. Try again later.
    /// </summary>
    ApplicationNotYetAvailable = 110001,

    /// <summary>
    /// API resource temporarily overloaded. Try again a little later.
    /// </summary>
    ResourceOverloaded = 130000,

    /// <summary>
    /// The stage channel is already open.
    /// </summary>
    StageAlreadyOpen = 150006,

    /// <summary>
    /// The bot can't reply to a message without the <see cref="DiscordPermission.ReadMessageHistory"/> permission.
    /// </summary>
    CannotReplyWithoutReadMessageHistory = 160002,

    /// <summary>
    /// A thread has already been created for this message.
    /// </summary>
    ThreadAlreadyCreated = 160004,

    /// <summary>
    /// The thread has been locked.
    /// </summary>
    ThreadLocked = 160005,

    /// <summary>
    /// The maximum number of threads has been reached.
    /// </summary>
    MaxNumberOfThreadsReached = 160006,

    /// <summary>
    /// The maximum number of announcement threads has been reached.
    /// </summary>
    MaxNumberOfAnnouncementThreadsReached = 160007,

    /// <summary>
    /// The JSON in the uploaded Lottie-format file was invalid.
    /// </summary>
    InvalidJsonForUploadedLottieFile = 170001,

    /// <summary>
    /// Uploaded Lottie files cannot contain rasterized images such as PNG or JPEG.
    /// </summary>
    UploadedLottiesCannotBeRasterized = 170002,

    /// <summary>
    /// The maximum framerate for a sticker has been exceeded.
    /// </summary>
    StickerFramerateExceeded = 170003,

    /// <summary>
    /// The maximum number of frames in a sticker has been exceeded (currently 1000).
    /// </summary>
    StickerFrameCountExceeded = 170004,

    /// <summary>
    /// The maximum dimensions of an animated Lottie sticker has been exceeded.
    /// </summary>
    LottieAnimationDimensionsExceeded = 170005,

    /// <summary>
    /// The framerate of the sticker is too small or too large.
    /// </summary>
    StickerFramerateTooSmallOrTooLarge = 170006,

    /// <summary>
    /// The sticker animation is too long (currently max 5 seconds).
    /// </summary>
    StickerAnimationDurationTooLong = 170007,

    /// <summary>
    /// A finished scheduled event cannot be updated.
    /// </summary>
    CannotUpdateFinishedEvent = 180000,

    /// <summary>
    /// For whatever reason, the system failed to create a stage for the stage event.
    /// </summary>
    FailedToCreatedStageForEvent = 180002,

    /// <summary>
    /// The posted message was blocked by an automatic moderation rule.
    /// </summary>
    MessageBlockedByAutomaticModeration = 200000,

    /// <summary>
    /// The title of the thread was blocked by an automatic moderation rule.
    /// </summary>
    TitleBlockedByAutomaticModeration = 200001,

    /// <summary>
    /// Webhooks can only create threads in forum channels, not in other channels.
    /// </summary>
    WebhooksCanOnlyCreateThreadsInForumChannels = 220003
}
