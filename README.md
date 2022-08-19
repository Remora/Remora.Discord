Remora.Discord
==============

Remora.Discord is a C# library for interfacing with the Discord API. It is built
to fulfill a need for robust, feature-complete, highly available and concurrent 
bots.

Want to chat with users and developers? Come join us!

[![Discord Server][5]][4] 

# Table of Contents
1. [Features](#1-features)
2. [Goals](#2-goals)
    1. [Correctness](#21-correctness)
    2. [Robustness](#22-robustness)
    3. [True Asynchronicity and Concurrency](#23-true-asynchronicity-and-concurrency)
3. [Status](#3-status)
    1. [Gateway](#31-gateway)
    2. [REST](#32-rest)
4. [Installation](#4-installation)
5. [Usage](#5-usage)
6. [Contributing](.github/CONTRIBUTING.md)

## 1. Features
  * Extensive API coverage - does anything and everything you need
  * Modern and active - uses contemporary technologies and usage patterns
  * Fully asynchronous - do many things at once at scale
  * Modular - swap parts of the library with your own implementations at will
  * Integrated - slash commands, traditional interfaces, or stateless bots

## 2. Goals
Remora.Discord originates from the original author's frustration with many 
inconsistencies in various APIs in the C#/Discord ecosystem, both in relation to
the Discord API itself and the language usage within existing solutions.

Therefore, Remora.Discord defines the following three goals that guides its 
development. These are shorter summaries - to read the full goal definitions and
see examples, please refer to the [Contributing Guidelines][2].

### 2.1 Correctness
Correctness, in the context of Remora.Discord, means that the API available to 
the end user should as faithfully and accurately represent the actual reality of
data presented to or from an API; that is, no data or structure of data should 
meaningfully change between the library receiving it and the user accessing it.

### 2.2 Robustness
Robustness refers to a focus on never allowing problems originating from user 
data or real-life runtime conditions to bring down or otherwise corrupt the end 
user's application. The end user should be confident that, should an error 
arise, they will be aware of the fault potential before even compiling the 
application.

### 2.3 True Asynchronicity and Concurrency
Remora.Discord aims to be truly asynchronous from the ground up, respecting and
utilizing established best practices for C# and the TPL. Furthermore, it aims to
be concurrent, allowing end users to react to and perform actions upon many 
incoming events at once.

## 3. Status
Remora.Discord is currently fully usable, and has been released for public 
consumption.

### 3.1 Gateway
The Discord Gateway API (v10) is fully implemented. The gateway client can 
connect, heartbeat, reconnect, resume, receive events, and send commands.

### 3.2 REST
The Discord REST API (v10) is fully implemented.

### 3.3 Voice
The Discord Voice API is not implemented. If you'd like to contribute to the 
library, this would be an excellent start.

## 4. Installation
Remora.Discord's primary distribution format is via [nuget][3] - get it there!

If you wish to use or develop the library further, you will need to compile it 
from source.

```bash
git clone git@github.com:Remora/Remora.Discord.git
cd Remora.Discord
dotnet build
dotnet pack -c Release
```

## 5. Usage
Up-to-date documentation for the API, as well as a quickstart guide, is 
available online at [the repository pages][1].

Each package has its own README with more detailed information regarding its 
purpose and use. If you want to know more about each one of these, please refer
to the list below. It's roughly organized in order of importance to end users, 
but feel free to explore.

  * [Remora.Discord](Remora.Discord/README.md)
    * [Remora.Discord.Extensions](Remora.Discord.Extensions/README.md)
    * [Remora.Discord.Hosting](Remora.Discord.Hosting/README.md)
    * [Remora.Discord.Interactivity](Remora.Discord.Interactivity/README.md)
    * [Remora.Discord.Pagination](Remora.Discord.Pagination/README.md)
  * [Remora.Discord.Commands](Remora.Discord.Commands/README.md)
  * [Remora.Discord.API](Backend/Remora.Discord.API/README.md)
    * [Remora.Discord.API.Abstractions](Backend/Remora.Discord.API.Abstractions/README.md)
  * [Remora.Discord.Rest](Backend/Remora.Discord.Rest/README.md)
  * [Remora.Discord.Gateway](Backend/Remora.Discord.Gateway/README.md)
  * [Remora.Discord.Caching.Abstractions](Backend/Remora.Discord.Caching.Abstractions/README.md)
    * [Remora.Discord.Caching](Backend/Remora.Discord.Caching/README.md)
      * [Remora.Discord.Caching.Redis](Backend/Remora.Discord.Caching.Redis/README.md)
  * [Remora.Discord.Unstable](Backend/Remora.Discord.Unstable/README.md)

If you want to get started quickly, please refer to the [Samples][7] for 
community-created example bots.

### 5.1 Versioning
A note on versioning - Remora.Discord uses SEMVER 2.0.0, which, in short, means

Given a version number MAJOR.MINOR.PATCH, increment the:

  1. MAJOR version when you make incompatible API changes,
  2. MINOR version when you add functionality in a backwards compatible manner,
     and
  3. PATCH version when you make backwards compatible bug fixes.

Due to the rapidly- and often-changing nature of Discord's API, this means that 
changes to the MAJOR component of the version in components of the library may
change almost every new release. Typically, new functionality in Discord's API
means that new fields are added, types of fields change, or parameters sent to 
endpoints change.

Generally, these changes only affect the API and API.Abstractions packages - 
these will often increment their MAJOR versions. Dependant packages - such as 
Gateway or Rest - will update together with these packages, but unless their 
public API changes as a result, it will be considered a PATCH upgrade.

The consequences of this is that you *may* see source-level breakages when 
upgrading from one minor version to the next. While undesirable, it is an effect
of Discord's uneven and inaccurate update cycle. Because of the way C# handles
dependencies, however, it's unlikely that this would affect anything outside of 
normal development - as such, it's been deemed an acceptable degradation.

### 5.2 Releases
Remora.Discord does not follow a set release cycle, and releases new versions 
on a rolling basis as new features of the Discord API are implemented or 
documented.

As a bot developer, you should check in every now and then to see what's 
changed - changelogs are released along with tags here on Github, as well as in
the individual package descriptions.

Whenever a new set of packages are released, the commit the releases were built 
from is tagged with the year and an incremental release number - for example,
`2021.1`.

#### 5.2.1 Bleeding Edge Builds
Whenever a new push to `master` is made, a new set of packages based on the 
latest commit will be published to GitHub Packages.

The URL of the NuGet source is `https://nuget.pkg.github.com/Remora/index.json`.
As the NuGet source requires authentication, follow GitHub's instructions: [here][9]

## 6. Contributing
See [Contributing][2].

## Thanks
Icon by [Twemoji][6], licensed under CC-BY 4.0.

[1]: https://remora.github.io/Remora.Discord/
[2]: .github/CONTRIBUTING.md
[3]: https://www.nuget.org/packages/Remora.Discord/
[4]: https://discord.gg/tRJbg8HNdt
[5]: https://img.shields.io/static/v1?label=Chat&message=on%20Discord&color=7289da&logo=discord
[6]: https://twemoji.twitter.com/
[7]: https://github.com/Remora/Remora.Discord/tree/master/Samples
[8]: https://github.com/Remora?tab=packages&repo_name=Remora.Discord
[9]: https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages
