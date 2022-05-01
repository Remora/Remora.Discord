using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.Commands.Attributes;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.Data.InternalLimits;

/// <summary>
/// Wraps two test groups.
/// </summary>
public class AtMostOneDMPermissionAttributeAllowed
{
    /// <summary>
    /// Wraps two named test groups.
    /// </summary>
    public class Named
    {
        /// <summary>
        /// The first group.
        /// </summary>
        [Group("a")]
        [DiscordDefaultDMPermission(true)]
        public class GroupOne : CommandGroup
        {
            /// <summary>
            /// The first command.
            /// </summary>
            /// <returns>Nothing.</returns>
            [Command("b")]
            [DoesNotReturn]
            public Task<Result> B() => throw new NotImplementedException();
        }

        /// <summary>
        /// The second group.
        /// </summary>
        [Group("a")]
        [DiscordDefaultDMPermission(false)]
        public class GroupTwo : CommandGroup
        {
            /// <summary>
            /// The second command.
            /// </summary>
            /// <returns>Nothing.</returns>
            [Command("c")]
            [DoesNotReturn]
            public Task<Result> C() => throw new NotImplementedException();
        }
    }
}
