Remora.Discord.Commands
=======================

This package provides glue code for using `Remora.Commands` with
`Remora.Discord`, adding appropriate conditions, autocompletion, type parsers,
and uniform UX for Remora-based applications.

## Structure
Most of the library's functionality is offloaded to `Remora.Commands`, for which
documentation is available at its [repository][1]. Beyond this, the structure of
the library revolves around concepts that should be relatively familiar to users
of other Discord libraries.

Take a look in [Parsers](Parsers) for an overview of the various types supported
as first-class parameters, beyond what `Remora.Commands` provides out of the 
box.

For each command interaction that is received by your application, a matching 
appropriate command is identified in your registered command trees, and is 
executed with an implementation of `ICommandContext`, which provides contextual
information about the command environment (such as guilds, users, additional 
data payloads, etc).

Furthermore, any autocompletion requests are routed to implementations of 
`IAutocompleteProvider`, enabling you to provide bot-driven autocompletion of 
command parameters.

## Usage
To enable commands, call the following method on your dependency injection 
container.

```c#
services.AddDiscordCommands(enableSlash: true);
```

Note that slash commands are not enabled by default, and you must opt into them
by passing `true` as above.

### Registering slash commands
Once you've added the command services and instantiated your service container,
you must register your command tree(s) with Discord.

Inject or retrieve the `SlashService` from your container in an appropriate way,
and then call
```c#
await slashService.UpdateSlashCommandsAsync();
```

This will register the default command tree globally; optionally, you may 
register a different named tree, or register the tree at a guild level.

```c#
await slashService.UpdateSlashCommandsAsync(guildID: myServerID, treeName: myTreeName);
```

Remember to check the result of your command registration, as not every feature
supported by `Remora.Commands` is supported by Discord.

### User feedback
If you want to create messages with a uniform UX, you may use the 
`FeedbackService` for various message types, such as notices, successes, 
failures, etc. The styling of these messages is controlled by an 
`IFeedbackTheme`, of which there are two provided by default. These themes map
to Discord's own dark and light themes, respectively.

You may also implement your own theme and register it with the dependency
injection container.

### Autocomplete
To provide autocomplete options while users of your application type, create a 
class that implements either `IAutocompleteProvider<T>` (for autocompletion of 
specific parameter types) or `IAutocompleteProvider` (for generic 
autocompletion).

```c#
public class MyAutocompleteProvider : IAutocompleteProvider
{
    public string Identity => "autocomplete::my-provider";
    
    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync
    (
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default
    )
    {
        // ...
    }
}
```

You may suggest up to 25 choices when your provider is invoked.

In order to select which autocomplete provider to use, apply the 
`AutocompleteAttribute` to the appropriate parameters in your command groups. If
you're using the type-specific autocomplete provider, this is not required.

### Pre- and post-execution events
In some cases, it may be useful to execute pieces of code before or after 
invocation of a command (checking GDPR consent, logging error messages, etc). 

This can be accomplished by registering pre- and post-execution events, which 
are classes which implement either `IPreExecutionEvent` or 
`IPostExecutionEvent`, respectively.

```c#
public class MyPreExecutionEvent : IPreExecutionEvent
{
    public Task<Result> BeforeExecutionAsync(ICommandContext context, CancellationToken ct = default)
    {
        // ...
    }
}

...

services.AddPreExecutionEvent<MyPreExecutionEvent>();
```

If you return an error in a pre-execution event, the command invocation is 
cancelled, and never progresses to the real execution stage. If you return an
error in a post-execution event, this is treated as if the command itself had 
failed, which may be useful for things like scrubbing database transactions.


[1]: https://github.com/Remora/Remora.Commands
