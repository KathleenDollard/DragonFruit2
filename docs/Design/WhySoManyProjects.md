# Why so many projects?

Each project has a separate purpose, or is required to manage dependencies. Yes there are 11.

This is presented in roughly alphabetical order.

## DragonFruit2 library

``` bash
DragonFruit2
DragonFruit2.Test
```

This is the library that the user interacts with and that the generated code relies on.

## Common library

``` bash
DragonFruit2.Common
DragonFruit2.Common.Test
```

This project contains code that is needed by both the library and the generator. This is primarily for marker attributes that are used by the generator to manage generation, but that must also be part of the library because they appear in user code.

## Generators

``` bash
DragonFruit2.Generators
DragonFruit2.Generators.Test
```

This project contains the generator.

## C# latest support

```
DragonFruit2.Polyfills
```

Roslyn generators run in .NET Standard 2.0.

The only supported version of C# for .NET Standard is is 7.3. However, many people, including all the .NET teams and many others in Microsoft, use the latest version of C#. This requires extra classes that appear in this project.

This particular version of polyfills has _**not**_ been reviewed and should not be copied for use in any other project.

## Initial work on testing generated applications

```
DragonFruit2.Functional.Test
```

This is early work on testing the behavior of apps written with DragonFruit2, particularly where programmers using the library and the apps they create will demonstrate DragonFruit2 behavior (instead of just System.CommandLine behavior). Some tests for validating command line args is in place.

## Prototype application

```dotnetcli
PrototypeConsoleApp
```

This is the prototype that contains an example of code that will trigger the generator and use the generated code. It also contains a handcrafted version of the code that w8ill be output. 

This project was used for initial development and remains in use as the project evolves and will remain in place for maintenance. 

All changes are made first in this app and checks are made that the code works correctly before changing the generator. You have to know what you are going to build.

## Sample applications

```dotnetcli
SampleConsoleApp
SubCommandConsoleApp
```

These projects contain code that exercises the generator. The code that triggers generation and uses the generated code is similar to the prototype application. However, the sample applications run the generator. 

This is especially useful when changing the generator because the resulting code can be viewed and some information is available when the generator fails.

There are two sample apps to provide samples of simple and complex usage. This helps DragonFruit2 maintainers by letting basic generation issues be resolved in minimal code, while still testing the more complex subcommand scenario. 

This also serves developers using the library because users can explore these applications to see how to use DragonFruit2, and code from these can be copied into docs to make it easier to keep them current. Users of DragonFruit2 are expected to fall into two groups - those that think of CLIs are a command tree, and those that think of them as a super simple app (or are beginners). Hopefully the naming will help them find the correct app.

