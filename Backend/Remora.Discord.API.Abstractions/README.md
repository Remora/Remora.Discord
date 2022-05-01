Remora.Discord.API.Abstractions
===============================

This package contains a complete set of type and API abstractions for the 
Discord API. It provides no concrete implementations; rather, it acts as a 
general, library-agnostic standard definition of Discord's API.

These types serve as the foundation of Remora.Discord's entire API surface, but 
can just as easily be used to implement your own Discord library, independently
of Remora.Discord.

The primary goal of this project is to model Discord's API as closely as 
possible, while at the same time applying appropriate C# practices and builtin
types (such as `DateTimeOffset`).

## Structure
The library is divided into type categories, organized to match Discord's API 
documentation as closely as is realistic. Each object defined by Discord has a 
corresponding interface, with inline documentation that matches Discord's.

The REST API surface is similarly divided by purpose, wherein related endpoints
are grouped together (application, audit log, channel, guild, etc).

## Usage
No particular usage recommendations exist for this library. It's up to you to 
decide how to implement or utilize these definitions.
