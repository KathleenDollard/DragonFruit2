# Overview

DragonFruit2 is intended as the next generation of the DragonFruit portion of System.CommandLine. The simplicity of System.CommandLine.DragonFruit made it very popular. However, it was a proof of concept and the underlying approach was not customizable or extensible. Also, we have come to believe that the key win is to maximize the scenarios where the implementing programmer can rely on their C# knowledge and avoid the distraction of learning a new library.

## Notes on docs

Unfortunately, is evolving relatively rapidly and up to date docs are challenging. [The Design Overview](docs/Design/DesignOverview.md) explains how the application runs. The documents in the Design folder are at least OK, and those in its subfolders will not be helpful.

This `ReadMe.md` file is also being kept up to date.

Issues and PRs are welcome to keep these docs current, and all other docs can be considered suspect. As other documents are updated, they will be included here.

## Scenarios

### Build a simple console app

- Create a console application
  - Use `dotnet new console` at the terminal, or
  - "Create new project" from the Visual Studio startup screen. If you are unfamiliar with .NET, take the defaults, except the project name.
- Add the DragonFruit2 NuGet package
  - `dotnet package add <TBD>`
  - "Project/Manage NuGet packages" pick the browse tab in the upper left and search for <TBD>
  - _This prototype is not yet ready for NuGet, and you'll have to source build. The artifacts folder can be used as a NuGet store._
- Replace the default contents of `Program.cs` with:

```csharp
using DragonFruit2;

if (Cli.TryParse<Args>(out var result))
{
   var args = result.Args
   // Do work here. The following line is an example
   Console.WriteLine($"{args.Greeting}, {args.Name}")
}
else
{
   result.ReportErrorsToConsole();
}
```

_Currently, `TryParse` is not yet implemented, so you will need to use `ParseArgs`._

You may use any name in place of `Args` as the type parameter to `TryParse`. It will be squiggled at this point in your process.

Positioning your cursor on the `Args` type parameter, hold Ctl and hit the period. You should get a menu that includes "Generate type `Args`", and "Generate a class in a new file".

Hit F12 to navigate to this new class and add `partial` to the class declaration:

```csharp
public partial class Args
```

The partial designator indicates that the DragonFruit2 generator will generate the code for the System.CommandLine interface and other DragonFruit2 features. 

_The hope is that we will eventually have a fixer that appears for the DragonFruit2 entry points and offers to create the `Args` class with the appropriate name and `partial`.

Add properties to the `Args` class. By default properties are created as options. Use Pascal case for property names (the normal C# standard). DragonFruit2 will convert these to appropriate names for your CLI based on the Posix standard. If you would like a property to appear as an argument, use the `Argument` attribute and specify the position in which the argument will appear. Argument positions should be unique. Here is an example:

_We should create an analyzer that checks for unique positions._

```csharp
public partial class Args
{
  /// <summary>
  /// "Your name"
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// "Greeting message"
  /// </summary>
  [Argument(1)]
  public string Greeting { get; set; } = string.Empty;
}
```

The summary will be used as the help description. _Not yet implemented._

You would call this in the terminal as:

```dotnetcli
> .\<your project name>.exe Hello --name World
```

### Learn how to use System.CommandLine

Build a simple console app and explore the generated code. This is a scenario to support the grow-up story
wgere 

## Overall design

Commands are defined via a class or struct (which can be a record) that is passed as the generic argument to
the `ParseArgs` or `Invoke` method.

Options and arguments are defined via properties on the command declaring class.

Subcommands are defined via classes that are derived from the parent class.

```c#
namespace SampleConsoleApp;

public partial class MyArgs
{
    /// <summary>
    /// "Your name"
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// "Your age"
    /// </summary>
    public int Age { get; set; } = 0;

    /// <summary>
    /// "Greeting message"
    /// </summary>
    public string Greeting { get; set; } = string.Empty;
}
```
 
## Naming

_ It is generally suggested that your arg class use normal naming without suffixes. However,
sufixes are also supported
- The name of the class is the name of the command
  - If it is the root command, the name is not used
  - If the name ends in `Command` or `Args` it is removed unless the `UseExactName` _(not yet implemented)_ attribute is used
- The name of the property is the name of the option or argument
  - By default, properties are options, because arguments need position information
  - Arguments use the `Argument` attribute, which must include `Position`
  - It is an error not to supply unique positions
  - The position of the property within the class is never used, because C# almost never has meaning to order and refactoring order is common 

## Required options and arguments

- Arguments and options are _not_ required unless:
  - The corresponding property is marked with the `required` keyword
  - The corresponding property is marked with the `Required` attribute (generally for downlevel usage) (_not yet implemented_)
  - It is a non-nullable reference type, unless it contains the `NotRequired` attribute _(not yet implemented)_, which is supplied for downlevel support

## Default values

- Default values for non-required arguments and options can be explicit or implicit
  - If is an auto-property which supplies an initialization value, it must be a constant and is used
  - The `Default` attribute is used on the property and must be a constant, in which case it's value is used
  - _Future: provide a programmatic way to declare a default so non-constants, especially `Today` can be used. This is partially implemented as the method call generated based on initialization and attributes._
  - _Future: Dependent defaults, such as a date that appears as an offset from another date. A scenario to track: Two dates and an integer. Each date is the other date with the days specified by the integer added or subtracted from the other date._
  - _Future: As much as possible, specific definitions will be used to support intelligent help, but in the end we will need a "whatever you need" ("get out of jail free") approach. This might be a custom type based on an interface to really encourage the author to supply help information._

## Subcommands

To create a subcommand, create a class that is derived from the `Args` class specified in the DragonFruit2 entry point (`TryParse`, `ParseArgs` or `TryExecute`). This class must be marked as `partial` and in the same project. It can be nested or in another namespace if desired. (_currently needs to be in the same namespace_)

The decision to use derived class for subcommands is largely pragmatic, but it may also make it more natural
to create nicely shaped CLIs. For example, considering `dotnet package add` to be an `add` specialization of 
the `package` command group, which is itself a specialization of the `dotnet` command group.

The pragmatic considerations are: 

- It reduces the number of attributes or special knowledge needed to build a CLI
- Only leaf nodes are ever invoked/created as a parse result and making non-invokable classes abstract is natural
- Any options or arguments that apply to parent commands apply to the leaf and this makes their values naturally available
- `recursive`, which has sometimes challenged folks, is strictly an aspect of System.CommandLine and based on an attribute. It's unrelated to the Args data.
- When working with a result it can be typed to the parent and a `switch` used to determine which subcommand was executed.

## Non-negotiables

- Simple, simple, simple for simple CLIs with linear scaling on complicating features. If cliffs exist, they are for very complex scenarios that do not follow the System.CommandLine interpretation of Posix
- .NETStandard support. Also, using code written in C# 7.3 must work.
- Focus on System.CommandLine doing what it does really well - parsing and strong typing results.

