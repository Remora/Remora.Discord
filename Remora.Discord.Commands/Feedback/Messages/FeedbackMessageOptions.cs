//
//  FeedbackMessageOptions.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.Commands.Feedback.Messages
{
    /// <summary>
    /// Defines options to customise a feedback message response.
    /// </summary>
    /// <param name="IsTTS">Indicates whether the message should use text-to-speech.</param>
    /// <param name="File">A file to send with the message.</param>
    /// <param name="AllowedMentions">The allowed mentions for the message.</param>
    /// <param name="MessageComponents">A list of message components to include with the message.</param>
    public record FeedbackMessageOptions
    (
        Optional<bool> IsTTS,
        Optional<FileData> File,
        Optional<IAllowedMentions> AllowedMentions,
        Optional<IReadOnlyList<IMessageComponent>> MessageComponents
    );
}
