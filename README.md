Remora.Discord
==============

Remora.Discord is a C# library for interfacing with the Discord API. It is built
to fulfill a need for robust, feature-complete, highly available and concurrent 
bots.

The library is currently in early development.

Want to chat with users and developers? Come join us!

[![Discord Server][5]][4] 

# Table of Contents
1. [Goals](#1-goals)
    1. [Correctness](#11-correctness)
    2. [Robustness](#12-robustness)
    3. [True Asynchronicity and Concurrency](#13-true-asynchronicity-and-concurrency)
2. [Status](#2-status)
    1. [Gateway](#21-gateway)
    2. [REST](#22-rest)
3. [Installation](#3-installation)
4. [Usage](#4-usage)
5. [Contributing](.github/CONTRIBUTING.md)

## 1. Goals
Remora.Discord originates from the original author's frustration with many 
inconsistencies in various APIs in the C#/Discord ecosystem, both in relation to
the Discord API itself and the language usage within existing solutions.

Therefore, Remora.Discord defines the following three goals that guides its 
development. These are shorter summaries - to read the full goal definitions and
see examples, please refer to the [Contributing Guidelines][2].

### 1.1 Correctness
Correctness, in the context of Remora.Discord, means that the API available to 
the end user should as faithfully and accurately represent the actual reality of
data presented to or from an API; that is, no data or structure of data should 
meaningfully change between the library receiving it and the user accessing it.

### 1.2 Robustness
Robustness refers to a focus on never allowing problems originating from user 
data or real-life runtime conditions to bring down or otherwise corrupt the end 
user's application. The end user should be confident that, should an error 
arise, they will be aware of the fault potential before even compiling the 
application.

### 1.3 True Asynchronicity and Concurrency
Remora.Discord aims to be truly asynchronous from the ground up, respecting and
utilizing established best practices for C# and the TPL. Furthermore, it aims to
be concurrent, allowing end users to react to and perform actions upon many 
incoming events at once.

## 2. Status
Remora.Discord is currently in early development. It has not been released in a 
stable state onto any common platforms, such as nuget.

### 2.1 Gateway
The Discord Gateway API (v8) is fully implemented. The gateway client can 
connect, heartbeat, reconnect, resume, receive events, and send commands.

### 2.2 REST
The Discord REST API (v8) is fully implemented.

## 3. Installation
Remora.Discord has a prerelease, early alpha version up on 
[nuget][3] - get it there!

If you wish to use or develop the library further, you will need to compile it 
from source.

```bash
git clone git@github.com:Nihlus/Remora.Discord.git
cd Remora.Discord
dotnet build
dotnet pack -c Release
```

## 4. Usage
Up-to-date documentation for the API, as well as a quickstart guide, is 
available online at [the repository pages][1].

Please refer to the [Samples](Samples) for community-created example bots.

### 4.1 Versioning
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
public API changes as a result, it will be considered a MINOR upgrade.

The consequences of this is that you *may* see source-level breakages when 
upgrading from one minor version to the next. While undesirable, it is an effect
of Discord's uneven and inaccurate update cycle. Because of the way C# handles
dependencies, however, it's unlikely that this would affect anything outside of 
normal development - as such, it's been deemed an acceptable degradation.

### 4.2 Releases
Remora.Discord does not follow a set release cycle, and releases new versions 
on a rolling basis as new features of the Discord API are implemented or 
documented.

As a bot developer, you should check in every now and then to see what's 
changed - changelogs are released along with tags here on Github, as well as in
the individual package descriptions.

## 5. Contributing
See [Contributing][2].

## Thanks
Icon by [Twemoji][6], licensed under CC-BY 4.0.

[1]: https://nihlus.github.io/Remora.Discord/
[2]: .github/CONTRIBUTING.md
[3]: https://www.nuget.org/packages/Remora.Discord/
[4]: https://discord.gg/tRJbg8HNdt
[5]: https://img.shields.io/static/v1?label=Chat&message=on%20Discord&color=7289da&logo=discord
[6]: https://twemoji.twitter.com/
