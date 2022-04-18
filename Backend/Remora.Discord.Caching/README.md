Remora.Discord.Caching
======================

This package contains cache functionality for the gateway and REST API, enabling
replacement of the standard services with caching counterparts. These services
attempt to retrieve entities from a cache provider whenever possible instead of
performing a remote call to Discord's REST API, saving bandwidth and increasing
performance.

A set of responders are also registered, enabling near real-time cache updates
as new events come in.

## Structure
The library defines a set of replacement services for the API services from 
Remora.Discord.Rest, which override the base implementation in order to perform
cache operations before and after normal calls.

In addition, two responders - one in the early group, and one in the late - are 
also defined, which handle eviction and insertion of incoming objects.

## Usage
Usage is simple, but with one caveat - caching must be enabled after the REST
and gateway services have been added to the dependency injection container, as 
this library replaces services from them.

After that, it's as simple as calling the following method. By default, the 
caching system uses an in-memory cache provider.

```c#
services.AddDiscordCaching();
```
