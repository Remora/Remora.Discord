Remora.Discord.Interactivity
============================

This package provides a framework for creating interaction-driven entities using 
Discord's message components.

## Structure
The library's design is very similar to Remora.Discord.Commands, utilizing many
of the same concepts. Interactions are treated as named commands, letting you
separate the logic from the frontend in a clean and reusable way.

All the familiar concepts from normal commands are available to you such as 
conditions, parsers, groups, and true concurrency.

## Usage
First, add the required services to the dependency injection container.

```c#
services.AddInteractivity();
```

In order to respond to incoming component interactions, declare a class that
inherits from the abstract `InteractionGroup` class. 

```c#
public class MyInteractions : InteractionGroup
{
}
```

Each supported component interaction has an associated attribute and function 
signature that lets the incoming data bind to and invoke your method. To use
them, declare one or more methods and decorate them with the appropriate
attribute inside your interaction group.

As with Remora.Commands, interaction methods may return any type implementing 
`IResult` (including `IResult` itself) as either a `Task<T>` or `ValueTask<T>`.  

### Buttons
Buttons are parameterless functions decorated with the `Button` attribute.

```c#
[Button("my-button")]
public Task<Result> OnButtonPressedAsync()
{
    // ...
}
```

### Select Menus
Select menus are functions with a list of objects as its sole parameter, 
decorated with the `SelectMenu` attribute. The parameter *must* be named 
`values`, and will contain zero or more values selected by the end user in the 
menu.

You can control the number of allowed values when creating the component through
its `MinValues` and `MaxValues` properties.

```c#
[SelectMenu("my-menu")]
public Task<Result> OnMenuSelectionAsync(IReadOnlyList<string> values)
{
    // ...
}
```

Since this set of values are passed through Remora.Commands' parsing system, you
can use any parsable type (including your own!) as the list's contained type. 
This means that, for example, the following snippets are all valid.

```c#
[SelectMenu("my-menu")]
public Task<Result> OnMenuSelectionAsync(IReadOnlyList<int> values)
{
    // ...
}
```

```c#
[SelectMenu("my-menu")]
public Task<Result> OnMenuSelectionAsync(IReadOnlyList<Snowflake> values)
{
    // ...
}
```

```c#
[SelectMenu("my-menu")]
public Task<Result> OnMenuSelectionAsync(IReadOnlyList<MyArbitraryType> values)
{
    // ...
}
```

The raw values in question are taken from the select menu options of the 
component, and can be any parseable data.

### Modals
Modals are functions with zero or more parameters matching the value types of 
the modal's contained components. This might sound a little complex, but is in
practice quite easy to use.

Modal interactions fire when a user submits an opened modal containing some 
values.

For example, if your modal contains a `TextInput` component with a custom ID
of `my-text-input`, you could then declare your function with a single parameter
that matches that custom ID. This parameter would then be passed the value of
that `TextInput` component.

```c#
[Modal("my-modal")]
public Task<Result> OnModalSubmittedAsync(string myTextInput)
{
    // ...
}
```

Multiple `TextInput` components map the same way, and can - same as with 
`SelectMenu` components - be declared as any parseable type. Parameters can
even be provided with default values, which will be used if the modal does not
contain a matching component.

```c#
[Modal("my-modal")]
public Task<Result> OnModalSubmittedAsync
(
    Snowflake mySnowflakeInput, 
    string myTextInput, 
    int myNumberInput = 0
)
{
    // ...
}
```

If you do not require any data from the modal, you can declare the
function without any parameters. This is also useful when your parsing 
requirements are more complex than the default system supports, in which case 
the raw modal payload is available on the `InteractionContext` type. An instance
of this type can be injected into your interaction group just like any other
dependency or service.

```c#
[Modal("my-modal")]
public Task<Result> OnModalSubmittedAsync()
{
    // ...
}
```

### Registering Your Interaction Group
Once you're ready to start using your interactions, it's as simple as 
registering it with your service collection and sending a component with an
appropriately formatted custom ID.

```c#
services.AddInteractionGroup<MyInteractions>
```

### Sending a Compatible Component
To avoid conflicts with components outside of Remora's interactivity system, you
are required to use specially prefixed IDs when creating components for use with
the system.

#### Custom IDs
These IDs can be created through the `CustomIDHelpers` class. For example, 
to create a compatible button, you would do something like the following.

```c#
new ButtonComponent
(
    ButtonComponentStyle.Primary,
    Label: "Click me!",
    CustomID: CustomIDHelpers.CreateButtonID("my-button")
)
```

Similar methods exist for the other component types, including modals.

There is one important exception to this - components inside a modal *must 
not* use any of the helper methods, and may only be comprised of a string 
convertible to a valid C# identifier. In practice, this means an ASCII string; 
dashes and underscores, however, may be used to delimit words, in which case 
they will be used for word boundaries when mapping to a `camelCase` name.

Some examples:

| Custom ID | Mapped C# Identifier |
|-----------|----------------------|
| "text"    | `text`               |
| "my-text" | `myText`             |
| "my_text" | `myText`             |

#### Named Groups
If you wish to utilize named groups (perhaps for the sake of organization, or
for distinguishing multiple interactions with the same desired ID), you can do
so by specifying the group name or names before the custom ID, delimited by a
space.

That is, a class like the one declared below:

```c#
[Group("separate")]
public class MyInteractions : InteractionGroup
{
    [Button("my-button")]
    public Task<Result> OnButtonPressedAsync()
    {
        // ...
    }
}
```
would respond to a button with a custom ID created like this:

```c#
CustomIDHelpers.CreateButtonID("separate my-button")
```

## In-Memory Data
Many interactions will want to share some type of persistent state, but not all
bots have the luxury or need of a full-fledged database behind them. Remora 
exposes a small type to aid with pure in-memory persistence that doesn't survive
a restart, but can serve simpler purposes such as pagination.

In essence, the type is a wrapped `ConcurrentDictionary` with stronger 
guarantees related to data access while you have access to a contained value.

To use it, register a singleton instance of whichever types you want to store in
your service provider.

```c#
services.AddSingleton(InMemoryDataService<MyKey, MyData>.Instance)
```

You can then inject this instance into your other services and groups in order
to manipulate the data contained within. You can also access the singleton
instance directly, though that should be avoided if at all possible.

The service has three CRUD-like methods for data manipulation:
```c#
bool TryAddData(TKey key, TData data);
Task<Result<DataLease<TKey, TData>>> LeaseDataAsync(TKey key, CancellationToken ct = default)
bool TryRemoveData(TKey key);
ValueTask<bool> TryRemoveDataAsync(TKey key);
```

Most signatures should be fairly self-explanatory, but there are some things to 
be mindful of. First and foremost, data is accessed by obtaining an exclusive 
lease from the container via `LeaseDataAsync`. This method produces a 
`DataLease`, which is a disposable wrapper type around the actual data. You may
use the data for as long as you hold the lease (that is, it is not disposed),
and provided everyone sticks to the same set of rules, you can be assured that 
you have exclusive access.

You can modify the value either by directly mutating it or by assigning a new
value to the `Data` property on the lease. If you no longer require the data, 
you can also delete it by calling `Delete` on the lease. 

Once you're done with the data, simply `DisposeAsync` (or `await using`) the
lease. The lease will then either update the associated data in the container or
delete it, depending on what you requested.

Any data passed into the service or a lease should be considered *moved*, and 
any further access after the lease expires is invalid on penalty of concurrency 
bugs and sad kittens. If you want to access the data again, you must also lease 
it once more.

Deleted data is, if required, disposed when removed from the container. The 
asynchronous disposal method takes precedence over the synchronous variant, but
both are supported. If you data only supports asynchronous disposal, however,
you *cannot* use `TryRemoveData` - it will throw an exception if it is unable to
fully dispose of removed data. Prefer using the asynchronous alternative 
whenever possible.
