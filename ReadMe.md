
## NEWSFLASH!!!!

### Generation redesign

The long delay between PRs here is partially because of "life" and also because this is a major redesign that aligns with my "Staying sane with Roslyn source generators" talk that I will be giving at Techorama in May. Watch for future documentation in this repo on a th arc of the generation process.

The impact on the design is that `CommandClass` types must have a `CommandClass` attribute. This improves generation performance, and indicates that they are special DTO classes with extra behavior, and in non-trivial cases, should probably be isolated to the CLI layer. It is also a breaking change in the prototype.


### .NET Rocks

Kathleen was interviewed about CLIs and DragonFruit2 on [.NET Rocks show #1992](https://open.spotify.com/episode/7uio0R72UtP1bEh8YKJfqn?si=25c3d592aba8444b ), scheduled for release March 5, 2026. If you are visiting based on that show - Welcome!!! 

Kathleen will be speaking at [Techorama](https://www.techorama.be/) May 11-13. One of her talks will be _Staying sane when writing Roslyn source generators_, which will feature the techniques in the DragonFruit2 source generators.

## Housekeeping

Check out the ["Help wanted" label under issues](https://github.com/KathleenDollard/DragonFruit2/issues?q=is%3Aissue%20state%3Aopen%20label%3A%22help%20wanted%22).

Please open issues for anything you think is a bug, or anything that you would like to see in the project (technically).

I've opened up [discussions](https://github.com/KathleenDollard/DragonFruit2/discussions). If you have questions or want to better understand the project, that would be a great place. General discussions on CLI design are fine too.

### Code of conduct

I appreciate kindness. If you need to read a code of conduct to know how to be kind, the [.NET Foundation Code of Conduct part "Our Standards"](https://dotnetfoundation.org/about/policies/code-of-conduct) seems as good as any. However, enforcement in this repo will be - if I think you are being a jerk I will toss you out.

Specific concerns are requested, appreciated, and will often be embraced.

### State of the project

This project is a prototype. I do not plan for it to be anything else while it is in this location. At the moment that also means I have not put it on NuGet yet.

You should be able to use it to simplify creation of System.CommandLine apps. Defaults are limited to constants, validation is limited, and alternative data providers (for defaults), such as configuration files are not implemented. Please report any bugs.

I would appreciate it greatly if you played with this and gave feedback. If you wish to use it in a production project, you should be prepared to fork it and maintain it. My hope is that we will find enough interest and contributors to find a good home for this project, decide if it needs a new name, and all the things involved moving a project to a permanent presence. I cannot assure you that will ever happen, so you will want a back up plan.

## Overview

DragonFruit2 is intended as the next generation of the DragonFruit portion of System.CommandLine. The simplicity of System.CommandLine.DragonFruit made it very popular. However, it was a proof of concept and the underlying approach was not customizable or extensible. Also, we have come to believe that the key win is to maximize the scenarios where the implementing programmer can rely on their C# knowledge and avoid the distraction of learning a new library.

## Notes on docs

This project evolving relatively rapidly and up to date docs are challenging. [The Design Overview](docs/Design/DesignOverview.md) explains how the application runs. The documents in the Design folder are at least OK, and those in its subfolders will not be helpful.

This `ReadMe.md` file is being kept up to date.

Issues and PRs are welcome to keep docs current, and all other docs can be considered suspect. As other documents are updated, they will be included here.

## Non-negotiables

- Simple, simple, simple for simple CLIs with linear scaling when using more complex features.
- .NETStandard support. Also, CLIs using code written in C# 7.3 must work.
- Let System.CommandLine do what it does really well - parsing and strong typing results. Wrap it to allow evolving behavior.

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

`TryParse` and `TryExecute` are planned.

You may use any name in place of `Args` as the type parameter to `TryParse`. It will be squiggled at this point in your process.

Positioning your cursor on the `Args` type parameter, hold Ctl and hit the period. You should get a menu that includes "Generate type `Args`", and "Generate a class in a new file".

Hit F12 to navigate to this new class and add `partial` and the `CommandClass` attribute to the class declaration:

```csharp
[CommandClass]
public partial class Args
```

The `CommandClass` attribute indicates that the DragonFruit2 generator will generate the code for the System.CommandLine interface and other DragonFruit2 features. The `partial` keyword indicates that the DragonFruit2 generator will add code to the class.

_Eventually, there should be a fixer that appears for the DragonFruit2 entry points and offers to create the `Args` class with the appropriate name and `partial`._

Add properties to the `Args` class. By default properties are created as options. Use Pascal case for property names (the normal C# standard). DragonFruit2 will convert these to appropriate names for your CLI based on the POSIX standard. If you would like a property to appear as an argument, use the `Argument` attribute and specify the position in which the argument will appear. Argument positions should be unique. Here is an example:

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

### SubCommands

Many CLIs consist of a single level. Some CLIs, such as the .NET CLI have subcommands, such as `dotnet add package DragonFruit2`. Whether or not there are subcommands, the initial entry point (`dotnet` in this case) is called the root command.

Subcommands in DragonFruit2are handled via derived classes. While this may be surprising, it allows a very natural handling of things like common options and how the returned result specifies which command was selected. A simple example is:

```csharp
public partial class SubCommandArgs
{
    public required string Name { get; set; }
    public virtual string Greeting { get; set; } = "Hello";
}

public partial class MorningArgs : SubCommandArgs
{
}

public partial class EveningArgs : SubCommandArgs
{
    public override string Greeting { get; set; } = "Good evening";
}
```

#### SubCommand notes

The decision to use derived class for subcommands is largely pragmatic, but it may also make it more natural
to create nicely shaped CLIs. For example, considering `dotnet package add` to be an `add` specialization of 
the `package` command group, which is itself a specialization of the `dotnet` command group.

The pragmatic considerations for using derived classes for subcommandsare: 

- It reduces the number of attributes or special knowledge needed to build a CLI
- Only leaf nodes are ever invoked/created as a parse result and making non-invokable classes abstract is natural
- Any options or arguments that apply to parent commands apply to the leaf and this makes their values naturally available
- `recursive`, which has sometimes challenged folks, is strictly an aspect of System.CommandLine and based on an attribute. It's unrelated to the Args data.
- When working with a result it can be typed to the parent and a `switch` used to determine which subcommand was executed.

### Validation

Validation (and defaults) can be defined in one of two ways: attributes or registration. Attributes are convenient, but there are things you cannot express in attributes, including the current date. Registration offers a fluent style of defining all of the validation for each command in a single place.

Declaring a property as `required` results in `Required` validation.

Defining validation via an attribute:

```csharp
internal partial class MyArgs : ArgsRootBase<MyArgs>
{
    public required string Name { get; init; }
    [GreaterThan(0)]
    public int Age { get; init; } = 1;
    public required string Greeting { get; init; } = "Howdy";
}
```

Defining validation via registration:

```csharp
internal partial class MyArgs : ArgsRootBase<MyArgs>
{
    public required string Name { get; init; }
    [GreaterThan(0)]
    public int Age { get; init; } = 1;
    public required string Greeting { get; init; } = "Howdy";

    partial class MyArgsDataDefinition
    {
        public override void RegisterCustomizations()
        {
            Age.ValidateGreaterThan(0);
        }
    }
}
```

Registration requires a nested partial class, which you might be unfamiliar with. Trust me it works!

### Default values

Default values work almost like validation, in that both attribute and registration approaches are supported. In addition we are experimenting with support for simple property initialization.

Adding a default value to the previous example via an attribute:

```csharp
internal partial class MyArgs : ArgsRootBase<MyArgs>
{
    public required string Name { get; init; }
    [GreaterThan(0)]
    public int Age { get; init; }
    [DefaultConstant("Hello")]
    public required string Greeting { get; init; };
}
```

Adding a default value to the previous example via registration:

```csharp
internal partial class MyArgs : ArgsRootBase<MyArgs>
{
    public required string Name { get; init; }
    [GreaterThan(0)]
    public int Age { get; init; }
    public required string Greeting { get; init; }

    partial class MyArgsDataDefinition
    {
        public override void RegisterCustomizations()
        {
            Age.ValidateGreaterThan(0);
            Age.DefaultConstant("Hello");
        }
    }
}
```

#### Default value notes

- Default values for non-required arguments and options can be explicit or implicit
  - If is an auto-property which supplies an initialization value, it must be a constant and is used
  - The `Default` attribute is used on the property and must be a constant, in which case it's value is used
  - _Future: provide a programmatic way to declare a default so non-constants, especially `Today` can be used. This is partially implemented as the method call generated based on initialization and attributes._
  - _Future: Dependent defaults, such as a date that appears as an offset from another date. A scenario to track: Two dates and an integer. Each date is the other date with the days specified by the integer added or subtracted from the other date._
  - _Future: As much as possible, specific definitions will be used to support intelligent help, but in the end we will need a "whatever you need" ("get out of jail free") approach. This might be a custom type based on an interface to really encourage the author to supply help information._

## Required options and arguments

There are several ways to indicate that an option or argument is required. 

- Arguments and options are _not_ required unless:
  - The corresponding property is marked with the `required` keyword
  - The corresponding property is marked with the `Required` attribute (generally for downlevel usage) (_not yet implemented_)
  - It is a non-nullable reference type, unless it contains the `NotRequired` attribute _(not yet implemented)_, which is supplied for downlevel support
 
 More guidelines may be needed to determine when a value is required. 

 Required values should not have default values. _A future analyzer will give a warning or error if a required value has a default value._

### Execution

A `TryExecute` method that runs an `Execute` method on the `Args` class is planned.
 
## Naming

It is generally suggested that your arg class use normal naming without suffixes. However, sufixes are also supported

- The name of the class is the name of the command
  - If it is the root command, the name is not used
  - If the name ends in `Command` or `Args` it is removed unless the `UseExactName` _(not yet implemented)_ attribute is used
- The name of the property is the name of the option or argument
  - By default, properties are options, because arguments need position information
  - Arguments use the `Argument` attribute, which must include `Position`
  - It is an error not to supply unique positions for arguments
  - If an option ends in `Option` or an argument ends in `Argument`, the suffix is removed unless the `UseExactName` _(not yet implemented)_ attribute is used
  - The position of the property within the class is never used, because C# almost never has meaning to order and refactoring order is common 














