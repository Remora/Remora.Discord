Remora.Discord
==============

Remora.Discord is a C# library for interfacing with the Discord API. It is built to fulfill a need for robust, 
feature-complete, highly available and concurrent bots.

The library is currently in early development.

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
Remora.Discord originates from the original author's frustration with many inconsistencies in various APIs in the 
C#/Discord ecosystem, both in relation to the Discord API itself and the language usage within existing solutions.

Therefore, Remora.Discord defines the following three goals that guides its development. These are shorter summaries - 
to read the full goal definitions and see examples, please refer to the 
[Contributing Guidelines](.github/CONTRIBUTING.md).

### 1.1 Correctness
Correctness, in the context of Remora.Discord, means that the API available to the end user should as faithfully and
accurately represent the actual reality of data presented to or from an API; that is, no data or structure of data 
should meaningfully change between the library receiving it and the user accessing it.

### 1.2 Robustness
Robustness refers to a focus on never allowing problems originating from user data or real-life runtime conditions to 
bring down or otherwise corrupt the end user's application. The end user should be confident that, should an error
arise, they will be aware of the fault potential before even compiling the application.

### 1.3 True Asynchronicity and Concurrency
Remora.Discord aims to be truly asynchronous from the ground up, respecting and utilizing established best practices for
C# and the TPL. Furthermore, it aims to be concurrent, allowing end users to react to and perform actions upon many 
incoming events at once.

## 2. Status
Remora.Discord is currently in early development. It has not been released in a stable state onto any common platforms, 
such as nuget.

### 2.1 Gateway
The Discord Gateway API (v8) is fully implemented. The gateway client can connect, heartbeat, reconnect, resume, 
receive events, and send commands.

### 2.2 REST
The Discord REST API (v8) is fully implemented.

## 3. Installation
Remora.Discord has a prerelease, early alpha version up on [nuget](https://www.nuget.org/packages/Remora.Discord/) - get
it there!

If you wish to use or develop the library further, you will need to compile it from source.

```bash
git clone git@github.com:Nihlus/Remora.Discord.git
cd Remora.Discord
dotnet build
dotnet pack -c Release
```

## 4. Usage
Basic usage of the library is currently a fluid topic. Please refer to the [Samples](Samples) for up-to-date usage - 
proper documentation and quickstarts will be written for the 1.0 release.

## 5. Contributing
See [Contributing](.github/CONTRIBUTING.md).
