//
//  IInviteStageInstance.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents stage information in an invite.
    /// </summary>
    [PublicAPI]
    public interface IInviteStageInstance
    {
        /// <summary>
        /// Gets the speaking members of the stage.
        /// </summary>
        IReadOnlyList<IPartialGuildMember> Members { get; }

        /// <summary>
        /// Gets the number of stage participants.
        /// </summary>
        int ParticipantCount { get; }

        /// <summary>
        /// Gets the number of users speaking in the stage.
        /// </summary>
        int SpeakerCount { get; }

        /// <summary>
        /// Gets the topic of the stage.
        /// </summary>
        string Topic { get; }
    }
}
