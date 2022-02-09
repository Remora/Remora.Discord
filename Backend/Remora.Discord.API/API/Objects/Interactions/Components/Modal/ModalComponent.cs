using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Objects.Form;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects.Modal;

public record ModalComponent
(
    Optional<string> CustomID,
    Optional<string> Title,
    Optional<IReadOnlyList<IMessageComponent>> Components
) : IModalComponent;
