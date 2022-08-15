//
//  FeedbackMessageOptions.cs
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
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Rest.Core;

namespace Remora.Discord.Commands.Feedback.Messages;

/// <summary>
/// Defines options to customise a feedback message response.
/// </summary>
/// <param name="IsTTS">Indicates whether the message should use text-to-speech.</param>
/// <param name="Attachments">The attachments to send with the message.</param>
/// <param name="AllowedMentions">The allowed mentions for the message.</param>
/// <param name="MessageComponents">A list of message components to include with the message.</param>
/// <param name="MessageFlags">The flags to set on the message.</param>
public record FeedbackMessageOptions
(
    Optional<bool> IsTTS = default,
    Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> Attachments = default,
    Optional<IAllowedMentions> AllowedMentions = default,
    Optional<IReadOnlyList<IMessageComponent>> MessageComponents = default,
    Optional<MessageFlags> MessageFlags = default
);
