//
//  Component.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Objects.IComponent" />
[PublicAPI]
public record Component
(
    ComponentType Type,
    Optional<IReadOnlyList<IMessageComponent>> Components,
    Optional<ButtonComponentStyle> Style,
    Optional<string> Label,
    Optional<IPartialEmoji> Emoji,
    Optional<string> CustomID,
    Optional<string> URL,
    Optional<bool> IsDisabled,
    Optional<IReadOnlyList<ISelectOption>> Options,
    Optional<string> Placeholder,
    Optional<int> MinValues,
    Optional<int> MaxValues,
    Optional<string> Title,
    Optional<int> MinimumLength,
    Optional<int> MaximumLength,
    Optional<bool> IsRequired,
    Optional<string> Value
) : IMessageComponent, IComponent;
