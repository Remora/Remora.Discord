using System.Collections.Generic;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects.Form;

/// <summary>
/// Represents a modal component with one or more component fields.
/// </summary>
public interface IModalComponent : IMessageComponent
{
    /// <inheritdoc cref="IComponent.CustomID"/>
    Optional<string> CustomID { get; }

    /// <inheritdoc cref="IComponent.Title"/>
    Optional<string> Title { get; }

    /// <inheritdoc cref="IComponent.Components"/>
    Optional<IReadOnlyList<IMessageComponent>> Components { get; }
}
