using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IInteractionModalCallbackData"/>
public record InteractionModalCallbackData
(
    string Title,
    string CustomID,
    IReadOnlyList<IMessageComponent> Components
) : IInteractionModalCallbackData;