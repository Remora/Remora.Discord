Remora.Discord.Interactivity
============================

This package provides a framework for creating interaction-driven entities using 
Discord's message components.

## Structure
The library's design revolves around an MVC-like architecture, where your 
application provides and drives the model and controller, while Discord serves
as the view.

A controller is implemented by creating a class that implements 
`IInteractiveEntity`, which in turn may access data from an arbitrary model of
your choice (EF Core, in-memory data, external APIs, etc). This entity is then
instantiated and invoked when your application receives a component interaction,
and declares its interest in the interaction based on contextual data.

Interested entities (of which there may be several) are then allowed to perform
their implemented functionality in order to update the model or view.

## Usage
First, add the required services to the dependency injection container.

```c#
services.AddInteractivity();
```

In order to respond to incoming component interactions, declare a class that
implements one or more interfaces related to the component type the entity
wishes to provide business logic for (such as `IButtonInteractiveEntity`,
`ISelectMenuInteractiveEntity`, or `IModalInteractiveEntity`).

Entities are transient services, so you may inject any other dependencies in 
normal fashion.

```c#
public class MyEntity : IButtonInteractiveEntity
{
    public Task<Result<bool>> IsInterestedAsync(ComponentType? componentType, string customID, CancellationToken ct = default)
    {
        // ...
    }
    
    public Task<Result> HandleInteractionAsync(IUser user, string customID, CancellationToken ct = default)
    {
        // ...
    }
}
```

These entities must then be registered with your dependency injection container.

```c#
services.AddInteractiveEntity<MyEntity>();
```

### External model
If your controller manipulates data from an external model, it is up to you to 
appropriately synchronize access or inject relevant services into the entity.

### In-memory model
If your controller only has simple needs, a built-in in-memory model may be
used, wherein access to a shared object is controlled by Remora. Entities that
wish to access the same in-memory model are executed sequentially, and changes 
made to the object are propagated between each entity.

Entities which wish to use this in-memory model must inherit from 
`InMemoryPersistentInteractiveEntity<TData>`, and messages associated with these
entities must be sent via the `InteractiveMessageService` where an initial model
state is provided.

```c#
public class MyInMemoryEntity : InMemoryPersistentInteractiveEntity, IButtonInteractiveEntity
{
    public string Nonce => _myContextualInformation.Nonce;

    public Task<Result<bool>> IsInterestedAsync(ComponentType? componentType, string customID, CancellationToken ct = default)
    {
        // ...
    }
    
    public Task<Result> HandleInteractionAsync(IUser user, string customID, CancellationToken ct = default)
    {
        // ...
        this.Data.DoSomething();
    }
}
```

The `Nonce` property is used to identify which shared object the entity wishes 
to act upon, and is usually formed from contextual information injected into the
entity (such as a guild, channel, user, or message ID).

If the shared object should be deleted after the entity has finished executing, 
you may set the `DeleteData` property to `true`.

Note that the shared objects are *not* deleted automatically, and do not have a 
set lifetime. If you do not flag them for deletion at some point, they will
persist indefinitely.
