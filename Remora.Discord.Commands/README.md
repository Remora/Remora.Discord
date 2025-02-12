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

Not every feature supported by `Remora.Commands` is supported by Discord - if 
you end up using incompatible features, an exception will be thrown at this 
point.

### Special Attributes
Remora.Discord.Commands exposes a number of attributes you can use to influence
or configure the way your commands are translated to Discord's slash command UX.

For the most part, these attributes map directly to fields or properties on 
Discord's application command object, but some have more specialized uses.

#### `ChannelTypes`
Sets the channel types that should be show in Discord's autocompletion when a
user is typing.

Can be applied to `Snowflake` or `IPartialChannel` (and implementors)-typed 
command parameters.

#### `DiscordDefaultDMPermission`
Sets the default DM accessibility for a group or command.

Can be applied to top-level groups or commands. The attribute is *not* 
compatible with nested groups or commands, and will produce an exception if 
applied to those.

#### `DiscordDefaultMemberPermissions`
Sets the default member permissions required for a user to use a group or 
command.

Can be applied to top-level groups or commands. The attribute is *not*
compatible with nested groups or commands, and will produce an exception if
applied to those.

#### `DiscordNsfw`
Sets whether the group or command is age-restricted and hidden or otherwise made
unavailable in open channels.

Can be applied to top-level groups or commands. The attribute is *not*
compatible with nested groups or commands, and will produce an exception if
applied to those.

#### `DiscordTypeHint`
Hints Discord's autocompletion about the type of the parameter it is applied to, 
allowing only certain types of autocompleted input data.

Can be applied to any parameter, but is typically most useful for 
`Snowflake`-typed parameters.

#### `ExcludeFromChoices`
Removes the marked enumeration member from Discord's autocomplete list.

Can be applied to enumeration members.

#### `ExcludeFromSlashCommands`
Excludes the marked group or command from the slash command UI. This is 
primarily useful when you have certain commands which use incompatible 
Remora.Commands features, since it also removes all restrictions that would 
otherwise come from being a slash command.

Can be applied to groups and commands.

#### `MinValue`
Marks a numeric parameter as having a minimum allowed value. This restricts
Discord's autocompletion and client-side validation to the specified range.

The range is inclusive.

Can be applied to any parameter which has a numeric C# type.

#### `MaxValue`
Marks a numeric parameter as having a maximum allowed value. This restricts
Discord's autocompletion and client-side validation to the specified range.

The range is inclusive.

Can be applied to any parameter which has a numeric C# type.

#### `SuppressInteractionResponse`
This attribute prevents Remora from automatically sending an interaction 
response on your behalf when receiving a slash command (or similar interaction,
such as a context menu click or button press). If you suppress the response, you
must send one yourself within a few seconds of entering user code or Discord 
will consider the interaction failed.

Can be applied to groups and commands.

### Context Menu Commands
Discord also supports context menu items on users and messages, which are 
treated as special slash commands by their system.

These commands have some additional restrictions.
  * they must have a particular C# signature
  * they must be declared as top-level commands
  * they must *not* have descriptions

To mark a command as either a `User` or a `Message` command, apply the 
`CommandType` attribute.

A `User` command must take a single parameter named `user`. 
A `Message` command must take a single parameter named `message`.

In both cases, the data passed to the command will be a `string` containing the
user or message's ID; typically, this means you'd want to declare the parameter 
with a type that can parse this ID into the entity you want. `IUser` or 
`IMessage` is probably a good bet, but you can use any type you want as long as 
its parser understands a `Snowflake` in `string` form.

Context menus also have relaxed rules when it comes to command names, and you
can use both varied capitalization as well as whitespace.

```c#
[Command("My User Command")]
[CommandType(ApplicationCommandType.User)]
public async Task<IResult> MyUserCommand(IUser user)
{
    // ...
}
```

### User Feedback
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

### Preparation error events
If a command can't be successfully prepared from whatever the user sends (due to
malformed input, parsing failures, failed conditions etc.), the command's 
"preparation" is considered failed.

In many cases, it's useful to hook into this event in order to provide the end
user with helpful information about what they did wrong or why the command 
didn't work.

This can be accomplished by registering a preparation error event, which is a 
class that implements `IPreparationErrorEvent`.

```c#
public class MyPreparationErrorEvent : IPreparationErrorEvent
{
    public Task<Result> PreparationFailed(IOperationContext context, IResult preparationResult, CancellationToken ct = default)
    {
        // ...
    }
}

...

services.AddPreparationErrorEvent<MyPreparationErrorEvent>();
```

The preparation result contains information about the error - try checking for
things like `CommandNotFoundError` or `ParameterParsingError` when you get a 
preparation error.

If you return an error in a preparation error event, the command invocation is
cancelled, and never progresses to the real execution stage.

By default, user- or environment-caused preparation errors don't produce any log
messages or user-facing output. It's up to you to decide if and how you want to
handle these.

Note that if you register multiple preparation events, they will run 
sequentially within the same service scope. Order is not guaranteed, but 
typically ends up being the same as registration order. Every event will get a
chance to run even if one of them fails, but failure of any of the events is 
considered a collective failure, and will cause the command invocation to be
cancelled.

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

Note that if you register multiple pre- or post-execution events, they will run
sequentially within the same service scope. Order is not guaranteed, but
typically ends up being the same as registration order. Every event will get a
chance to run even if one of them fails, but failure of any of the events is
considered a collective failure. In the case of pre-execution events, this will
cause the command invocation to be cancelled.


[1]: https://github.com/Remora/Remora.Commands
