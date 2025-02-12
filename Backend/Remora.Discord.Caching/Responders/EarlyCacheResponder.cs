//
//  EarlyCacheResponder.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.Responders;

/// <summary>
/// Caches incoming data from the gateway.
/// </summary>
[PublicAPI]
public class EarlyCacheResponder :
    IResponder<IChannelCreate>,
    IResponder<IChannelUpdate>,
    IResponder<IGuildBanAdd>,
    IResponder<IGuildCreate>,
    IResponder<IGuildEmojisUpdate>,
    IResponder<IGuildMemberAdd>,
    IResponder<IGuildMembersChunk>,
    IResponder<IGuildMemberUpdate>,
    IResponder<IGuildRoleCreate>,
    IResponder<IGuildRoleUpdate>,
    IResponder<IMessageCreate>,
    IResponder<IMessageReactionAdd>,
    IResponder<IUserUpdate>,
    IResponder<IInteractionCreate>
{
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EarlyCacheResponder"/> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    public EarlyCacheResponder(CacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IChannelCreate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.ChannelCacheKey(gatewayEvent.ID);
        await _cacheService.CacheAsync<IChannel>(key, gatewayEvent, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IChannelUpdate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.ChannelCacheKey(gatewayEvent.ID);
        await _cacheService.CacheAsync<IChannel>(key, gatewayEvent, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildBanAdd gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.GuildBanCacheKey(gatewayEvent.GuildID, gatewayEvent.User.ID);
        await _cacheService.CacheAsync<IBan>(key, new Ban(null, gatewayEvent.User), ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = default)
    {
        if (!gatewayEvent.Guild.TryPickT0(out var availableGuild, out _))
        {
            return Result.FromSuccess();
        }

        var key = new KeyHelpers.GuildCacheKey(availableGuild.ID);
        await _cacheService.CacheAsync<IGuild>(key, availableGuild, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildEmojisUpdate gatewayEvent, CancellationToken ct = default)
    {
        foreach (var emoji in gatewayEvent.Emojis)
        {
            if (emoji.ID is null)
            {
                continue;
            }

            var key = new KeyHelpers.EmojiCacheKey(gatewayEvent.GuildID, emoji.ID.Value);
            await _cacheService.CacheAsync(key, emoji, ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = default)
    {
        if (!gatewayEvent.User.TryGet(out var user))
        {
            return Result.FromSuccess();
        }

        var key = new KeyHelpers.GuildMemberKey(gatewayEvent.GuildID, user.ID);
        await _cacheService.CacheAsync<IGuildMember>(key, gatewayEvent, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildMembersChunk gatewayEvent, CancellationToken ct = default)
    {
        foreach (var member in gatewayEvent.Members)
        {
            if (!member.User.TryGet(out var user))
            {
                continue;
            }

            var key = new KeyHelpers.GuildMemberKey(gatewayEvent.GuildID, user.ID);
            await _cacheService.CacheAsync(key, member, ct);
        }

        if (!gatewayEvent.Presences.TryGet(out var presences))
        {
            return Result.FromSuccess();
        }

        foreach (var presence in presences)
        {
            if (!presence.User.ID.TryGet(out var userID))
            {
                continue;
            }

            var key = new KeyHelpers.PresenceCacheKey(gatewayEvent.GuildID, userID);
            await _cacheService.CacheAsync(key, presence, ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildMemberUpdate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.GuildMemberKey(gatewayEvent.GuildID, gatewayEvent.User.ID);

        // Since this event isn't playing nice, we'll have to update by creating an object of our own.
        var cacheResult = await _cacheService.TryGetValueAsync<IGuildMember>(key, ct);

        var cachedInstance = cacheResult.Entity;

        if (cacheResult.IsSuccess)
        {
            cachedInstance = new GuildMember
            (
                new Optional<IUser>(gatewayEvent.User),
                gatewayEvent.Nickname.TryGet(out var nickname) ? nickname : cachedInstance.Nickname,
                gatewayEvent.Avatar,
                gatewayEvent.Banner,
                gatewayEvent.Roles,
                gatewayEvent.JoinedAt ?? cachedInstance.JoinedAt,
                gatewayEvent.PremiumSince.TryGet(out var premiumSince) ? premiumSince : cachedInstance.PremiumSince,
                gatewayEvent.IsDeafened.TryGet(out var isDeafened) ? isDeafened : cachedInstance.IsDeafened,
                gatewayEvent.IsMuted.TryGet(out var isMuted) ? isMuted : cachedInstance.IsMuted,
                default, // TODO: this is probably on this event, but Discord hasn't documented it
                gatewayEvent.IsPending.TryGet(out var isPending) ? isPending : cachedInstance.IsPending,
                cachedInstance.Permissions,
                gatewayEvent.CommunicationDisabledUntil
            );
        }
        else if (gatewayEvent.JoinedAt.HasValue)
        {
            cachedInstance = new GuildMember
            (
                new Optional<IUser>(gatewayEvent.User),
                gatewayEvent.Nickname,
                gatewayEvent.Avatar,
                gatewayEvent.Banner,
                gatewayEvent.Roles,
                gatewayEvent.JoinedAt.Value,
                gatewayEvent.PremiumSince,
                gatewayEvent.IsDeafened.TryGet(out var isDeafened) && isDeafened,
                gatewayEvent.IsMuted.TryGet(out var isMuted) && isMuted,
                default, // TODO: this is probably on this event, but Discord hasn't documented it
                gatewayEvent.IsPending.TryGet(out var isPending) && isPending,
                default,
                gatewayEvent.CommunicationDisabledUntil
            );
        }
        else
        {
            return Result.FromSuccess();
        }

        await _cacheService.CacheAsync(key, cachedInstance, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildRoleCreate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.GuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.Role.ID);
        await _cacheService.CacheAsync(key, gatewayEvent.Role, ct);

        return await UpdateRolesList(gatewayEvent.GuildID, gatewayEvent.Role, ct);
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IGuildRoleUpdate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.GuildRoleCacheKey(gatewayEvent.GuildID, gatewayEvent.Role.ID);
        await _cacheService.CacheAsync(key, gatewayEvent.Role, ct);

        return await UpdateRolesList(gatewayEvent.GuildID, gatewayEvent.Role, ct);
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.MessageCacheKey(gatewayEvent.ChannelID, gatewayEvent.ID);
        await _cacheService.CacheAsync<IMessage>(key, gatewayEvent, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IMessageReactionAdd gatewayEvent, CancellationToken ct = default)
    {
        if (!gatewayEvent.GuildID.TryGet(out var guildID))
        {
            return Result.FromSuccess();
        }

        if (!gatewayEvent.Member.TryGet(out var member))
        {
            return Result.FromSuccess();
        }

        if (!member.User.TryGet(out var user))
        {
            return Result.FromSuccess();
        }

        var key = new KeyHelpers.GuildMemberKey(guildID, user.ID);
        await _cacheService.CacheAsync(key, member, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync(IUserUpdate gatewayEvent, CancellationToken ct = default)
    {
        var key = new KeyHelpers.UserCacheKey(gatewayEvent.ID);
        await _cacheService.CacheAsync<IUser>(key, gatewayEvent, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = default)
    {
        if (!gatewayEvent.Data.TryGet(out var data))
        {
            return Result.FromSuccess();
        }

        if (!data.TryPickT0(out var commandData, out _))
        {
            return Result.FromSuccess();
        }

        if (!commandData.Resolved.TryGet(out var resolved))
        {
            return Result.FromSuccess();
        }

        if (resolved.Users.TryGet(out var users))
        {
            foreach (var (key, value) in users)
            {
                var cacheKey = new KeyHelpers.UserCacheKey(key);
                await _cacheService.CacheAsync(cacheKey, value, ct);
            }
        }

        if (!resolved.Roles.TryGet(out var roles) || !gatewayEvent.GuildID.TryGet(out var guildID))
        {
            return Result.FromSuccess();
        }

        foreach (var (key, value) in roles)
        {
            var cacheKey = new KeyHelpers.GuildRoleCacheKey(guildID, key);
            await _cacheService.CacheAsync(cacheKey, value, ct);
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Updates the cached role list when a role is created or updated.
    /// </summary>
    /// <remarks>This is a workaround for the fact that Discord lacks an endpoint to get single roles.</remarks>
    /// <param name="guildID">The ID of the guild the role belongs to.</param>
    /// <param name="role">The role.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>Success if the list has been updated or one did not exist; otherwise, error.</returns>
    private async Task<Result> UpdateRolesList(Snowflake guildID, IRole role, CancellationToken ct = default)
    {
        var collectionKey = new KeyHelpers.GuildRolesCacheKey(guildID);
        var getCachedList = await _cacheService.TryGetValueAsync<IReadOnlyList<IRole>>(collectionKey, ct);
        if (getCachedList.IsSuccess)
        {
            var oldList = getCachedList.Entity;

            var newList = oldList.ToList();
            var existingRoleIndex = newList.FindIndex(r => r.ID == role.ID);
            if (existingRoleIndex > -1)
            {
                // It's already in the list; update it
                newList[existingRoleIndex] = role;
            }
            else
            {
                // new role, add it
                newList.Add(role);
            }

            await _cacheService.CacheAsync(collectionKey, newList, ct);
        }
        else if (getCachedList.Error is not NotFoundError)
        {
            // Some other problem; NotFound is fine
            return (Result)getCachedList;
        }

        return Result.FromSuccess();
    }
}
