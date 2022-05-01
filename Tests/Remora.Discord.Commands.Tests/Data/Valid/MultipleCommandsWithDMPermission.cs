using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.Commands.Attributes;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.Data.Valid;

/// <summary>
/// Wraps two test groups.
/// </summary>
public class MultipleCommandsWithDMPermission
{
    /// <summary>
    /// The first group.
    /// </summary>
    [DiscordDefaultDMPermission(true)]
    public class GroupOne : CommandGroup
    {
        /// <summary>
        /// The first command.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Command("a")]
        [DoesNotReturn]
        public Task<Result> A() => throw new NotImplementedException();
    }

    /// <summary>
    /// The second group.
    /// </summary>
    [DiscordDefaultDMPermission(false)]
    public class GroupTwo : CommandGroup
    {
        /// <summary>
        /// The second command.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Command("b")]
        public Task<Result> B() => throw new NotImplementedException();
    }
}
