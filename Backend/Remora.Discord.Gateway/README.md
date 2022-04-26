Remora.Discord.Gateway
======================

This package provides a fully-featured implementation of a Discord gateway
client, complete with resume capabilities and user-facing events.

## Structure
The package consists of two main parts - the client itself, and the event 
dispatch system. The latter will be familiar to users of the mediator pattern,
wherein an intermediate type handles communication between separate objects; in 
Remora.Discord.Gateway's case, all incoming Discord events are dispatched and 
executed concurrently with each other, and the user may register responders to 
handle these events.

Provided you yourself do not inject services with a shared mutable state, all
responders are thread-safe and do not require additional synchronization through
semaphores or mutexes.

The client is a singleton service by design.

## Usage
First and foremost, the gateway client must be registered with your dependency
injection container. 

```c#
services.AddDiscordGateway(serviceProvider => GetTokenFromSomewhere(serviceProvider));
```

Two things to note:
  * You are not required to call any other methods from dependency libraries
    (such as `Remora.Discord.API`) - this call takes care of all transitive
    dependencies itself.
  * You must, at this point, provide a way to access your bot's token in plain
    text. This access only happens after the container is constructed, and an
    API instance is requested, so you have access to the built container.

After this point, you may inject the gateway client into your own services. In 
order to connect and start dispatching events, you must call `RunAsync`
somewhere, and subsequently either await the task directly or store it for later
use. This task represents the entire lifetime of the gateway client, and will 
run until you explicitly request its termination via the cancellation token
passed to this method.

If you wish to explicitly send a gateway command (for example, to update
presence information), you may do so via the `SubmitCommand` method on the
client. Note that no validations are performed on this command, so you can cause
unexpected termination of your session if the data is malformed or inappropriate
for the current state of the client.

Responders - the types which react to and handle incoming events - are created
by declaring a class that implements one or more instances of the 
`IResponder<T>` interface, and then registering these types with the service 
container.

```c#
public class MyResponder : IResponder<IMessageCreate>
{
    public async Task<Result> RespondAsync
    (
        IMessageCreate gatewayEvent,
        CancellationToken ct = default
    )
    {
        // ...
    }
}

...

services.AddResponder<MyResponder>();
```

There are no restrictions placed on how long an individual responder may run, 
but you should strive to keep it short in order to avoid thread pool exhaustion. 

Responders have no guarantees related to sequential execution, but you are able
to sort them into one of three "groups", which do run sequentially to one 
another.

```c#
services.AddResponder<MyResponder>(ResponderGroup.Early); // executes before...
services.AddResponder<MyResponder>(ResponderGroup.Normal); // executes before...
services.AddResponder<MyResponder>(ResponderGroup.Late);
```

This is useful in cases where two responders to the same event depend on each 
other in some way.
