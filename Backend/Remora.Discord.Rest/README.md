Remora.Discord.Rest
===================

This package contains an implementations of Discord's REST API, complete with 
client-side data validation checks and full rate limiting support.

The primary goal of this project is to provide a concrete, rules-compliant 
implementation of Discord's REST API endpoints, mapping them to appropriately
accessible asynchronous tasks and services.

## Structure
The library mostly mirrors Remora.Discord.API.Abstractions in structure,
maintaining an implementation of each corresponding REST API interface. The 
implementations are registered as transient services, available to you via 
dependency injection.

## Usage
To enable injection of the REST API services, add them to your service container
by way of the `AddDiscordRest` extension method.

```c#
services.AddDiscordRest(serviceProvider => GetTokenFromSomewhere(serviceProvider));
```

Two things to note:
  * You are not required to call any other methods from dependency libraries
    (such as `Remora.Discord.API`) - this call takes care of all transitive
    dependencies itself.
  * You must, at this point, provide a way to access your bot's token in plain 
    text. This access only happens after the container is constructed, and an 
    API instance is requested, so you have access to the built container.

After this point, you may inject any of the `IDiscordRest[...]API` services into
your own services.
