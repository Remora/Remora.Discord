Remora.Discord.Caching.Redis
============================

This package contains a distributed, Redis-based cache provider which can 
replace the default in-memory cache provider. This alternate cache provider 
enables a shared cache between multiple applications, which may include (but is 
not limited to) shards, other bots, and shark-mounted laser cannons.

## Structure
The structure is simple, and merely provides a Redis-backed `ICacheProvider`
implementation.

## Usage
Usage is simple, but with one caveat - caching must be enabled after the REST
and gateway services have been added to the dependency injection container, as
this library replaces services from them.

After that, it's as simple as calling the following method. By default, the
caching system uses an in-memory cache provider.

```c#
services.AddRedisDiscordCaching();
```

Note that you are not required to call `AddDiscordCaching`; this is taken care
of by the method above.
