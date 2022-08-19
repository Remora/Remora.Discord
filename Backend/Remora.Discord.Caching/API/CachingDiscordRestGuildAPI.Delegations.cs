//
//  CachingDiscordRestGuildAPI.Delegations.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

public partial class CachingDiscordRestGuildAPI
{
    /// <inheritdoc />
    public Task<Result> ModifyGuildChannelPositionsAsync
    (
        Snowflake guildID,
        IReadOnlyList<(Snowflake ChannelID, int? Position, bool? LockPermissions, Snowflake? ParentID)> positionModifications,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildChannelPositionsAsync(guildID, positionModifications, ct);
    }

    /// <inheritdoc />
    public Task<Result<IGuildThreadQueryResponse>> ListActiveGuildThreadsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return _actual.ListActiveGuildThreadsAsync(guildID, ct);
    }

    /// <inheritdoc />
    public Task<Result> ModifyGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string?> nickname = default,
        Optional<IReadOnlyList<Snowflake>?> roles = default,
        Optional<bool?> isMuted = default,
        Optional<bool?> isDeafened = default,
        Optional<Snowflake?> channelID = default,
        Optional<DateTimeOffset?> communicationDisabledUntil = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildMemberAsync
        (
            guildID,
            userID,
            nickname,
            roles,
            isMuted,
            isDeafened,
            channelID,
            communicationDisabledUntil,
            reason,
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> AddGuildMemberRoleAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.AddGuildMemberRoleAsync(guildID, userID, roleID, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result> RemoveGuildMemberRoleAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.RemoveGuildMemberRoleAsync(guildID, userID, roleID, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result> CreateGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<int> deleteMessageDays = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.CreateGuildBanAsync(guildID, userID, deleteMessageDays, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result<MultiFactorAuthenticationLevel>> ModifyGuildMFALevelAsync
    (
        Snowflake guildID,
        MultiFactorAuthenticationLevel level,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildMFALevelAsync(guildID, level, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result<IPruneCount>> GetGuildPruneCountAsync
    (
        Snowflake guildID,
        Optional<int> days = default,
        Optional<IReadOnlyList<Snowflake>> includeRoles = default,
        CancellationToken ct = default
    )
    {
        return _actual.GetGuildPruneCountAsync(guildID, days, includeRoles, ct);
    }

    /// <inheritdoc />
    public Task<Result<IPruneCount>> BeginGuildPruneAsync
    (
        Snowflake guildID,
        Optional<int> days = default,
        Optional<bool> computePruneCount = default,
        Optional<IReadOnlyList<Snowflake>> includeRoles = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.BeginGuildPruneAsync(guildID, days, computePruneCount, includeRoles, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result<IGuildWidget>> GetGuildWidgetAsync(Snowflake guildID, CancellationToken ct = default)
    {
        return _actual.GetGuildWidgetAsync(guildID, ct);
    }

    /// <inheritdoc />
    public Task<Result<IPartialInvite>> GetGuildVanityUrlAsync(Snowflake guildID, CancellationToken ct = default)
    {
        return _actual.GetGuildVanityUrlAsync(guildID, ct);
    }

    /// <inheritdoc />
    public Task<Result<Stream>> GetGuildWidgetImageAsync
    (
        Snowflake guildID,
        Optional<WidgetImageStyle> style = default,
        CancellationToken ct = default
    )
    {
        return _actual.GetGuildWidgetImageAsync(guildID, style, ct);
    }

    /// <inheritdoc />
    public Task<Result<IWelcomeScreen>> ModifyGuildWelcomeScreenAsync
    (
        Snowflake guildID,
        Optional<bool?> isEnabled = default,
        Optional<IReadOnlyList<IWelcomeScreenChannel>?> welcomeChannels = default,
        Optional<string?> description = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyGuildWelcomeScreenAsync(guildID, isEnabled, welcomeChannels, description, reason, ct);
    }

    /// <inheritdoc />
    public Task<Result> ModifyCurrentUserVoiceStateAsync
    (
        Snowflake guildID,
        Snowflake channelID,
        Optional<bool> suppress = default,
        Optional<DateTimeOffset?> requestToSpeakTimestamp = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyCurrentUserVoiceStateAsync(guildID, channelID, suppress, requestToSpeakTimestamp, ct);
    }

    /// <inheritdoc />
    public Task<Result<IVoiceState>> ModifyUserVoiceStateAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake channelID,
        Optional<bool> suppress = default,
        CancellationToken ct = default
    )
    {
        return _actual.ModifyUserVoiceStateAsync(guildID, userID, channelID, suppress, ct);
    }

    /// <inheritdoc/>
    public RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            // TODO: not ideal...
            throw new NotImplementedException("The decorated API type is not customizable.");
        }

        return customizable.WithCustomization(requestCustomizer);
    }

    /// <inheritdoc/>
    void IRestCustomizable.RemoveCustomization(RestRequestCustomization customization)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            return;
        }

        customizable.RemoveCustomization(customization);
    }
}
