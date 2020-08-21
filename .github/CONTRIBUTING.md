Contributing
============
Welcome to Remora.Discord's contributing guidelines! I'm glad you've decided to read this document, and I hope that 
it'll be helpful in any contributions you make to the library.

Generally, this document will serve to give you some background on design choices made in the library, as well as 
to highlight things to be aware of while developing new features or fixing bugs. If there are any questions or
irregularities that can't be answered by this document, open an issue and ask for assistance - I'll do my best to 
clarify and answer.

# Table of Contents
1. [Goals](#1-goals)
    1. [Correctness](#11-correctness)
    2. [Robustness](#12-robustness)
    3. [True Asynchronicity and Concurrency](#13-true-asynchronicity-and-concurrency)
2. [Structure](#2-structure)
3. [How to Contribute](#3-how-to-contribute)
4. [Tips & Tricks](#4-tips--tricks)

## 1. Goals
To understand many of the design choices, you must first understand the three pillar goals of Remora.Discord, and why 
they are the way they are. Remora.Discord originates from the original author's frustration with many inconsistencies 
in various APIs in the  C#/Discord ecosystem, both in relation to the Discord API itself and the language usage within 
existing solutions. The goals below were set early in the development process to guide development away from these 
problems, and to find parts of the user experience that should be placed at the forefront of the library.

Therefore, Remora.Discord defines the following three goals.

### 1.1 Correctness
Correctness, in the context of Remora.Discord, means that the API available to the end user should as faithfully and
accurately represent the actual reality of data presented to or from an API; that is, no data or structure of data 
should meaningfully change between the library receiving it and the user accessing it.

As an example, Discord sometimes sends what they refer to as "partial" objects, while still defining the data entities
as having fields not transmitted in partial objects as required. Remora.Discord interprets this as an error in Discord's
API documentation and implements these fields as optional, thus providing a "correct" view of the data.

### 1.2 Robustness
Robustness refers to a focus on never allowing problems originating from user data or real-life runtime conditions to 
bring down or otherwise corrupt the end user's application. The end user should be confident that, should an error
arise, they will be aware of the fault potential before even compiling the application.

Any method that has a fault potential should be declared in such a way that the user must consider whether the operation
was successful before proceeding. To this end, Remora.Discord further breaks issues down into the following categories:

  * Programmer errors
  * User errors
  * Environment errors
  
Programmer errors are caused by incorrect, invalid, or inappropriate use of Remora.Discord's API that cannot be caught
at compile time. These errors should, as early as possible, throw an exception to prevent the invalid usage from making
it out of the development phase.

```c#
_object.CallMeAfterA(); // throws InvalidOperationException
_object.A();
```

User errors originate from externally sourced input to the library, such as payloads from Discord, or data from the end
user provided in a method call. These errors typically involve Remora.Discord encountering data it is unable to parse, 
data it is unfamiliar with, or data violating some form of constraint. The primary distinction from programmer errors
is that these issues may appear frequently or nondeterministically, usually due to the end user varying their input.

```c#
var result = _object.PerformPotentiallyFailingAction();
if (result.IsSuccess)
{
    ...
}
```

In all cases, this category of errors should result in an unsuccessful return type being returned by the callee.

Environment errors stem from indirect problems; network outages, disk space, memory runout, etc. While in their own 
category due to their unpredictability, they are treated and reported the same way as user errors.

### 1.3 True Asynchronicity and Concurrency
Remora.Discord aims to be truly asynchronous from the ground up, respecting and utilizing established best practices for
C# and the TPL. Furthermore, it aims to be concurrent, allowing end users to react to and perform actions upon many 
incoming events at once.

Everything that Remora.Discord provides to the end user which involves some form of either IO- or CPU-bound work is
presented as a task, and it assumes everything a user wishes to perform in registered callbacks or customized services
may become IO or CPU bound. 

Any asynchronous operation is also designed to be cancellable, allowing clean terminations of processes and units of 
work.

## 2. Structure
The library is structured into three main parts - the abstractions, the concrete reference implementations, and a 
high-level layer. 

At its core, the library exposes nothing but abstract interfaces without any implementations behind 
them, serving only as a common and basic 1:1 mapping to the Discord API (with some allowances made for C#-ification of 
data types). This abstraction layer has no external dependencies and no business logic whatsoever. 
 
On top of that, a concrete reference implementation is defined, where the abstraction layer is used to build a base 
library that actually does the work against the real API - serializing JSON, mapping properties, setting up websockets, 
communicating over HTTP, etc; this library serves as the default implementation for any consumers that make use of the 
abstractions in their projects.

Finally, a high-level layer utilizes the default implementation to provide a more consumer-friendly and C#-oriented API
for interacting with Discord - caching, setting nicknames, sending messages, joining servers, etc. This high-level layer
also provides an abstraction layer; this is what consumers should primarily use.

This structure enables Remora.Discord to first and foremost define a de facto C# mapping of the Discord API that any 
library can implement, and end user applications will not have to adapt or be rewritten to support specific libraries.
Beyond that usage potential, Remora.Discord also takes the role of an implementer and provides a usable backend for the
abstractions.

## 3. How to Contribute
To contribute to the library, start by browsing open issues and seeing if anything catches your eye & interest. If it 
does, fork the project (if you haven't already) and create a separate branch for your changes. Open a pull request early
and claim the issue, then begin working. Following the goal guidelines and input from other community members, finalize 
your changes into a state where you're happy with them, then request a review and merge from a maintainer.

Once your pull request clears the review phase, it'll be merged. If any changes are necessary, you will be alerted 
during the review phase and a maintainer will work with you to ensure your changes are in line with the library's goals
and up to snuff.

Generally, code at the review phase is expected to fulfill the following requirements; that is, that it: 

  * Compiles. Code that does not compile will instantly fail code review.
  * Follows the library's syntax standards. In most cases, the compiler will yell at you if you do something 
    incorrectly. If not, a maintainer will most likely catch it during review and send it back so you can fix it.
  * Has unit tests. If your contribution adds code that is uncovered by tests, you are expected to also write tests for
    it that has as high a coverage as is realistically possible.
  * Passes all tests. If you have failing unit tests, you must correct either your changes or the tests to pass and 
    appropriately test the changes.
  * Follows the library goals. See the goals above; this is a "soft" requirement, in that there are no hard rules - it's 
    the spirit of the goals that matters. If a maintainer believes that something is implemented contrary to the goals,
    they will alert you during the review phase, or earlier if they've noticed by themselves.

## 4. Tips & Tricks
In order to more quickly write code that's in line with the library standards, consider applying the following tips & 
tricks while developing.

  * Fail fast, fail often. If your code takes an input, verify it as early as possible and return an error (or throw, if
    appropriate) as quickly as you can. If you call a method that has failure potential, always check the results.
  * Minimize indentation. Keep things as flat and linear as you can, avoiding deep nesting. Invert conditionals where 
    possible; prefer `if (!condition)` and returning over `if (condition)` and performing actions inside the scope.
  * Avoid throwing exceptions. Exceptions should only be used for programmer errors, as defined in [Goals](#1-goals).
  * Avoid catching exceptions in inner scopes. If something does go wrong, it should bubble up as far as possible before
    being caught and wrapped in a result.
  * Never let exceptions bubble up into user code. While exceptions should bubble up as far as possible, it should 
    *never* reach user code. An uncaught exception that bubbles up into user code is considered a library bug.
  * Formulate boolean parameters and properties as questions. Write `ShouldDoThing` instead of `DoThing`, `IsDone` 
    instead of `Done`, etc.
  * Avoid returning `null`. `null` is to be considered an exceptional value, and should not be returned without a very
    good reason. Similarly, `null` should not be accepted as a valid input unless explicitly intended.
  * Use C#8's nullability annotations. Be clear and concise with your intent.
  * Don't surprise the caller. Follow the principle of least astonishment - if a user would be surprised by what your 
    code does, it's probably time to change it.
  * Name methods and variables descriptively. It's better to have a long and descriptive name than a short and 
    abbreviated one.
