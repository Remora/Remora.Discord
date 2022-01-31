using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Conditions;

/// <summary>
/// Marks an entity as requiring a certain permission for the bot user (self/current user).
/// </summary>
/// <remarks>
/// Supported entities include the following:
///     * Command groups (which require the bot to have the specified permission(s)).
///     * Commands (which require the bot to have the specified permission(s)).
///     * IChannels (which require the bot to have the specified permission(s) in the channel).
/// More than one permission may be specified, in which case the behaviour is controlled with
/// <see cref="Operator"/>.
/// </remarks>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
public class RequireBotDiscordPermissionsAttribute : ConditionAttribute
{
    /// <summary>
    /// Gets the logical operator used to combine the permissions.
    /// </summary>
    public LogicalOperator Operator { get; init; } = LogicalOperator.And;

    /// <summary>
    /// Gets the permissions that should be checked.
    /// </summary>
    public IReadOnlyList<DiscordPermission> Permissions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBotDiscordPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="permissions">The permissions to require.</param>
    public RequireBotDiscordPermissionsAttribute(params DiscordPermission[] permissions)
    {
        this.Permissions = permissions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBotDiscordPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="permissions">The permissions to require.</param>
    public RequireBotDiscordPermissionsAttribute(params DiscordTextPermission[] permissions)
    {
        this.Permissions = permissions.Cast<DiscordPermission>().ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBotDiscordPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="permissions">The permissions to require.</param>
    public RequireBotDiscordPermissionsAttribute(params DiscordVoicePermission[] permissions)
    {
        this.Permissions = permissions.Cast<DiscordPermission>().ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBotDiscordPermissionsAttribute"/> class.
    /// </summary>
    /// <param name="permissions">The permissions to require.</param>
    public RequireBotDiscordPermissionsAttribute(params DiscordStagePermission[] permissions)
    {
        this.Permissions = permissions.Cast<DiscordPermission>().ToArray();
    }
}
