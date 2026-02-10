# DragonFruit2 Overview

(These are aspirational docs to communicate during development and evolve into final docs. Features described may not be implemented now or ever.)

One goal of DragonFruit2 is to use System.CommandLine using natural C# features, reducing of eliminating the need to learn the syntax of the library. System.CommandLine is a powerful Posix parser that provides consistent Posix rules across the ecosystem. DragonFruit2 makes it very simple to use, only becoming more complex when you need to do more complex things.

Another goal of DragonFruit2 is to provide a extensibility platform for future evolution in areas like help, validation, transforms, prompting, and execution.

The three cohorts under consideration:

- Beginners to programming
  - This cohort inspires limiting the concepts that are required for success
- Programmers that want to write a quick and dirty app
  - This cohort includes extremely sophisticated programmers and inspires DragonFruit2 being the simplest way to create a console app
- Library authors with CLIs of moderate complexity
  - This cohort inspires advanced features such as cross-property validation, transforms, and data providers
- (Aspirational) The .NET CLI
  - This cohort will be essential to us understanding the limitations of DragonFruit2
- Extension authors
  - This cohort inspires a rich extensibility story and Spectre may be important to understanding what kind of extensibility makes sense

## Goals and non-goals

(This section should be expanded)

### Goals

In order to support the identified cohorts:

- Basic parsing is dirt simple with no knowledge beyond classes and methods that are easy to find in IntelliSense
- .NET Standard/.NET (Full/Windows) Framework ca use DragonFruit2
  - This is a non-beginner's scenario and it may not be possible for it to be the same or quite as simple
  - The downlevel approach should work with modern .NET to support multi-targeting
  - While there can be compromises, most functionality should work, perhaps in a less ideal way, in the supported versions of C# Framework (C# 7.2)
- Invocation is optional
- `required` is via the `required` keyword, with attributes available for down-level .NET versions
  - This supports object creation by other means to support application testing
- Defaults
  - Can come from a dictionary (for testing DragonFruit2 setup), configuration files, custom data providers, and/or C# initialization, including lazy getters
  - To support this
    - Until args class (`MyArgs`) instantiation, all data is held in `DataValue` objects that have a `IsSet` property
    - A ctor is used rather than object initialization
    - Properties are only set if `IsSet` is true
- Validation
  - Validation is done on the created object, after data providers, transforms, and instantiation/defaults. System.CommandLine validation is not used.
  - Anticipated validations are via attributes
  - Unanticipated validations are done via Validator classes that are easy to write and apply and can use more than one property
  - Validator classes (including those that back attributes) are rich containing at least `Id`, `Description`, `Message`, `Predicate`
  - Priority will be considered. If used, this would allow "bands" of validation where is some failed others would not be checked or reported. The intent would be to help users find the needle in the haystack when one error cascades to many errors, making it difficult to find the underlying error
- Immutable args classes are fully embraced and supported
- Transforms can be performed during construction between data collection and validation (such as for trimming, string casing, or ceiling/floor)

### Non-goals

- There will be no grow-up story. Because DragonFruit2 is a layer on top of System.CommandLine designed to allow evolution of features, there will be no way to drop it without losing features. As such, extension points must be well thought out and most or all of the capabilities of System.CommandLine may be available.

- Empowering people to break System.CommandLine's version of Posix. This may be possible if we allow custom parsing, but it is a System.CommandLine feature DragonFruit2 itself does not support.
  - It is possible that exploring the .NET CLI will lead to some softening in this stance

- Supporting `struct` arg types. Supporting `struct` arg classes will be desirable if it happens, but we will not off trade simplicity for the programmer using DragonFruit2. This will result in a very small number of allocations, and even complex CLIs such as the .NET CLI have only 50-75 classes and may not stay in memory long enough for GC to occur.

## Syntax 

See [ReadMe](..\readme.md)

In this simplest form, on failures the DragonFruit2 will report errors and exit.

## Goals and scenarios

### Scenario: Beginner

Top level statements (C# 9) provides a great way for beginners to write code with a limited set of concepts. Assuming their input is strings, they only need to add indexing (`[]`) to get arguments. As they desire more features, or the author/teacher thinks it's the right time to introduce classes or the richness of the ecosystem, DragonFruit2 steps in.

This is an opportunity to introduce some of the richness of property declarations, such as `required` and `initialization`

It is also an opportunity to introduce attributes, such as those for 



## Other options for core syntax 

(remove this section later)

- Reflection: Just no
- Builder pattern: I did not find a way to keep the calling code simple
- Attributes: Does not solve the issues, or complicated the calling syntax