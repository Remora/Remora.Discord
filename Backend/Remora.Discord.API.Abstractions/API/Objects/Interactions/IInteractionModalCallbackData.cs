using System.Collections.Generic;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents return payload data for an interaction response.
/// </summary>
public interface IInteractionModalCallbackData
{
    /// <summary>
    /// Gets the title for the modal.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Gets the custom ID for the modal.
    /// </summary>
    string CustomID { get; }
    
    /// <summary>
    /// Gets the components for the modal.
    /// </summary>
    /// <remarks>
    /// Currently only supports <see cref="ITextInputComponent"/>s.
    /// </remarks>
    IReadOnlyList<IMessageComponent> Components { get; }
}