//
//  DiscordPermissionSet.cs
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
using System.Numerics;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects;

/// <inheritdoc />
[PublicAPI]
public class DiscordPermissionSet : IDiscordPermissionSet
{
    /// <summary>
    /// Gets an empty permission set.
    /// </summary>
    public static DiscordPermissionSet Empty { get; } = new(Array.Empty<DiscordPermission>());

    /// <inheritdoc />
    public BigInteger Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public DiscordPermissionSet(BigInteger value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
    /// </summary>
    /// <param name="permissions">The permissions in the set.</param>
    public DiscordPermissionSet(params DiscordPermission[] permissions)
    {
        if (permissions.Length == 0)
        {
            return;
        }

        var largestPermission = permissions.Max(p => (ulong)p);
        var largestByteIndex = (int)Math.Floor(largestPermission / 8.0);

        // Create a sufficiently sized byte area
        Span<byte> bytes = stackalloc byte[largestByteIndex + 1];

        // Convert the permission set to a byte array
        foreach (var permission in permissions)
        {
            // The value of the permission is defined as the zero-based bit index, that is, a Discord value of
            // 0x00000001 (the first permission) is represented with the value 0.
            var bit = (ulong)permission;

            // The byte index is the zero-based index into the Value property where the bit in question is.
            var byteIndex = (int)Math.Floor(bit / 8.0);

            // The bit index is the index into the byte of the value
            var bitIndex = (byte)(bit - (ulong)(8 * byteIndex));

            // Update the value in the array
            ref var currentValue = ref bytes[byteIndex];
            currentValue |= (byte)(1 << bitIndex);
        }

        this.Value = new BigInteger(bytes, true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
    /// </summary>
    /// <param name="permissions">The permissions in the set.</param>
    public DiscordPermissionSet(params DiscordTextPermission[] permissions)
        : this(permissions.Cast<DiscordPermission>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordPermissionSet"/> class.
    /// </summary>
    /// <param name="permissions">The permissions in the set.</param>
    public DiscordPermissionSet(params DiscordVoicePermission[] permissions)
        : this(permissions.Cast<DiscordPermission>().ToArray())
    {
    }

    /// <inheritdoc />
    public bool HasPermission(DiscordPermission permission)
    {
        // The value of the permission is defined as the zero-based bit index, that is, a Discord value of
        // 0x00000001 (the first permission) is represented with the value 0.
        var bit = (ulong)permission;

        // The byte index is the zero-based index into the Value property where the bit in question is.
        var byteIndex = (int)Math.Floor(bit / 8.0);

        // The bit index is the index into the byte of the value
        var bitIndex = (byte)(bit - (ulong)(8 * byteIndex));

        if (this.Value.GetByteCount(true) <= byteIndex)
        {
            // This would fall outside of the value; therefore, it cannot be contained in the set.
            return false;
        }

        var byteValue = this.Value.ToByteArray(true)[byteIndex];
        var isBitSet = ((byteValue >> bitIndex) & 0x00000001) > 0;

        return isBitSet;
    }

    /// <inheritdoc />
    public IReadOnlyList<DiscordPermission> GetPermissions()
    {
        var permissions = new List<DiscordPermission>();
        var valueBytes = this.Value.ToByteArray(true);

        for (var byteIndex = 0; byteIndex < valueBytes.Length; byteIndex++)
        {
            var b = valueBytes[byteIndex];

            for (var bitIndex = 0; b != 0; bitIndex++)
            {
                var bitValue = b & 0x1;
                if (bitValue == 1)
                {
                    permissions.Add((DiscordPermission)((byteIndex * 8) + bitIndex));
                }

                b >>= 1;
            }
        }

        return permissions;
    }

    /// <summary>
    /// Computes a full permission set for a user, taking roles and overwrites into account.
    /// </summary>
    /// <param name="memberID">The ID of the member.</param>
    /// <param name="everyoneRole">The @everyone role, assigned to every user.</param>
    /// <param name="memberRoles">The roles that the user has, if any.</param>
    /// <param name="overwrites">The channel overwrites currently in effect, if any.</param>
    /// <returns>The true permission set.</returns>
    public static IDiscordPermissionSet ComputePermissions
    (
        Snowflake memberID,
        IRole everyoneRole,
        IReadOnlyList<IRole> memberRoles,
        IReadOnlyList<IPermissionOverwrite>? overwrites = default
    )
    {
        overwrites ??= Array.Empty<IPermissionOverwrite>();

        // Start calculations with the everyone role
        var basePermissions = everyoneRole.Permissions.Value;
        foreach (var memberRole in memberRoles)
        {
            basePermissions |= memberRole.Permissions.Value;
        }

        // Apply the everyone role overwrites, if applicable
        var everyoneOverwrite = overwrites.FirstOrDefault(o => o.ID == everyoneRole.ID);
        if (everyoneOverwrite is not null)
        {
            basePermissions &= ~everyoneOverwrite.Deny.Value;
            basePermissions |= everyoneOverwrite.Allow.Value;
        }

        // Apply role overwrites, if applicable
        var rolesAllow = BigInteger.Zero;
        var rolesDeny = BigInteger.Zero;

        foreach (var roleOverwrite in overwrites.Where(o => memberRoles.Any(r => r.ID == o.ID)))
        {
            rolesAllow |= roleOverwrite.Allow.Value;
            rolesDeny |= roleOverwrite.Deny.Value;
        }

        basePermissions &= ~rolesDeny;
        basePermissions |= rolesAllow;

        // Apply member overwrites, if applicable
        var memberOverwrite = overwrites.FirstOrDefault(o => o.ID == memberID);

        // ReSharper disable once InvertIf
        if (memberOverwrite is not null)
        {
            basePermissions &= ~memberOverwrite.Deny.Value;
            basePermissions |= memberOverwrite.Allow.Value;
        }

        return new DiscordPermissionSet(basePermissions);
    }

    /// <summary>
    /// Computes a full permission set for a role, taking overwrites into account.
    /// </summary>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="everyoneRole">The @everyone role, assigned to every user.</param>
    /// <param name="overwrites">The channel overwrites currently in effect, if any.</param>
    /// <returns>The true permission set.</returns>
    public static IDiscordPermissionSet ComputePermissions
    (
        Snowflake roleID,
        IRole everyoneRole,
        IReadOnlyList<IPermissionOverwrite>? overwrites = default
    )
    {
        overwrites ??= Array.Empty<IPermissionOverwrite>();

        // Start calculations with the everyone role
        var basePermissions = everyoneRole.Permissions.Value;

        // Apply the everyone role overwrites, if applicable
        var everyoneOverwrite = overwrites.FirstOrDefault(o => o.ID == everyoneRole.ID);
        if (everyoneOverwrite is not null)
        {
            basePermissions &= ~everyoneOverwrite.Deny.Value;
            basePermissions |= everyoneOverwrite.Allow.Value;
        }

        // Apply role overwrites, if applicable
        var roleOverwrite = overwrites.FirstOrDefault(o => o.ID == roleID);

        // ReSharper disable once InvertIf
        if (roleOverwrite is not null)
        {
            basePermissions &= ~roleOverwrite.Deny.Value;
            basePermissions |= roleOverwrite.Allow.Value;
        }

        return new DiscordPermissionSet(basePermissions);
    }

    /// <inheritdoc />
    public bool HasPermission(DiscordTextPermission permission) => HasPermission((DiscordPermission)permission);

    /// <inheritdoc />
    public bool HasPermission(DiscordVoicePermission permission) => HasPermission((DiscordPermission)permission);

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise,
    /// <see langword="false" />.
    /// </returns>
    protected bool Equals(DiscordPermissionSet other)
    {
        return this.Value.Equals(other.Value);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((DiscordPermissionSet)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }
}
