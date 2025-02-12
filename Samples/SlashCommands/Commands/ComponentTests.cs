//
//  SPDX-FileName: ComponentTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Samples.SlashCommands.Commands;

/// <summary>
/// Tests for components.
/// </summary>
public class ComponentTests
(
    IInteractionContext interactionContext,
    IDiscordRestInteractionAPI interactionAPI
) : CommandGroup
{
    /// <summary>
    /// Displays a component.
    /// </summary>
    /// <returns>A task.</returns>
    [Command("bookmark_example")]
    public async Task<Result> DisplayAsync()
    {
        IMessageComponent[] components =
        [
            new ContainerComponent
            (
                [
                    new TextDisplayComponent
                    (
                        $"""
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent non sollicitudin augue[...]
                        -# This bookmark contains 2 attachments
                        
                        
                        
                        
                        
                        
                        
                        \u200b
                        """.Replace("\\u200b", "\u200b")
                    ),
                    new ActionRowComponent([new ButtonComponent(ButtonComponentStyle.Danger, "Paginate Left", CustomID: "paginate_left", IsDisabled: true), new ButtonComponent(ButtonComponentStyle.Success, "Paginate Right", CustomID: "paginate_right", IsDisabled: false)]),
                    new ActionRowComponent
                    (
                        [
                            new StringSelectComponent
                            (
                                "select",
                                [new SelectOption("The first one kek", "tfok")],
                                "Select a bookmark to view",
                                MinValues: 1,
                                MaxValues: 1
                            )
                        ]
                    )
                ],
                AccentColour: Color.PaleGreen
            ),
        ];

        await interactionAPI.CreateFollowupMessageAsync(interactionContext.Interaction.ApplicationID, interactionContext.Interaction.Token, components: components, flags: MessageFlags.IsComponentV2 | MessageFlags.SuppressNotifications);
        return Result.Success;
    }
}
