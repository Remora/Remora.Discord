//
//  WebhookEvent.cs
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
using OneOf;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IWebhookEvent"/>
public record WebhookEvent
(
    WebhookEventType Type,
    DateTimeOffset Timestamp,
    OneOf<IApplicationAuthorizedWebhookEvent, IEntitlement> Data
) : IWebhookEvent;
