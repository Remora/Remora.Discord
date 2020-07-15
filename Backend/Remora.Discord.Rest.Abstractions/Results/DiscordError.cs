//
//  DiscordError.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.Rest.Abstractions.Results
{
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
        /// A general error (suc as a malformed request body, amongst other things).
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
        /// Unknown redistributable.
        /// </summary>
        UnknownRedistributable = 10036,

        /// <summary>
        /// Bots cannot use this endpoint.
        /// </summary>
        NoBotsAllowed = 20001,

        /// <summary>
        /// Only bots can use this endpoint.
        /// </summary>
        BotsOnly = 20002,

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
        /// Maximum number of guild roles reached (250).
        /// </summary>
        MaxGuildRolesReached = 30005,

        /// <summary>
        /// Maximum number of webhooks reached (10).
        /// </summary>
        MaxWebhooksReached = 30007,

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
        /// Unauthorized. Provide a valid token and try again.
        /// </summary>
        Unauthorized = 40001,

        /// <summary>
        /// You need to verify your account in order to perform this action.
        /// </summary>
        UnverifiedAccount = 40002,

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
        /// Cannot send messages in a voice channel.
        /// </summary>
        CannotSendMessageToVoiceChannel = 50008,

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
        /// Invalid OAuth2 access token provided.
        /// </summary>
        InvalidOAuth2Token = 50025,

        /// <summary>
        /// A message provided was too old to bulk delete.
        /// </summary>
        MessageTooOldToBulkDelete = 50034,

        /// <summary>
        /// Invalid form body (returned for both <code>application/json</code> and <code>multipart/form-data</code>
        /// bodies), or invalid <code>Content-Type</code> provided.
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
        /// Reaction was blocked.
        /// </summary>
        ReactionBlocked = 90001,

        /// <summary>
        /// API resource temporarily overloaded. Try again a little later.
        /// </summary>
        ResourceOverloaded = 130000
    }
}
