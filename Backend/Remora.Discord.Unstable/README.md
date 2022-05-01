Remora.Discord.Unstable
=======================

This package contains unstable features from the Discord API - that is, features 
that aren't in the official documentation yet, but may be documented in a pull 
request or through community efforts.

The API in this package should never be considered stable, and breaking changes 
may be introduced in any release. Once unstable features move into the official
documentation, they will be transferred from this component to the appropriate 
stable component of Remora.Discord.

## Structure
Generally, the structure mirrors the intended future location of the types in 
their corresponding packages (API, Abstractions, etc).

## Usage
By the nature of this package, the usage may change from day to day. However, as
a ground rule, experimental features may be enabled by calling the following 
method on your dependency injection container.

```c#
services.AddExperimentalDiscordApi();
```
