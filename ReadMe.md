# Overview

DragonFruit2 is intended as the next generation of the DragonFruit portion of System.CommandLine. The simplicity of System.CommandLine.DragonFruit made it very popular. However, it was a proof of concept and the underlying approach was not customizable or extensible. Also, we have come to believe that the key win is to maximize the scenarios where the implementing programmer can rely on their C# knowledge and avoid the distraction of learning a new library.

## Notes on docs

Unfortunately, the docs are not up to date as the system is evolving relatively rapidly. (The overview)[docs/__Overview.md] gives an idea of project thinking and [this design document](docs/DesignOverview.md) gives a view of what the individual pieces are intended to do. 

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


## Future features

If the first phase is getting all SCL featues working with simple gestures, the next phase is likely adding features. It would be highly desirable
to support this second phase with extensibilty points that did not reuquire understanding code generation details.

Some of the features that could be supported include:

- If we can identify error that occur because a required value is missing, we could prompt for it with something like "Please enter value for <option/argument name>: ".
- A different help system could be slotted in, using whatever approach is desired, by circumventing the default one. For example, verbose help or examples.
- Something like directives could be used to indicate alternate output - particularly full markdown or HTML documentation or platform specific tab completion. This could be help, or possibly application output.
- Defaults from configuration or calculated from environment.
- Wrappers for other front ends, such as APIs and GUIs.

Some features will require a deeper understanding of the code generation process:

- A fast generation mode that relies on a CLI attribute, rather than retrieving the type from the `ParseArgs` generic argument. Supporting two modes is not a high priority, but a non-attributed mode is likely not fast enough to be a design generator, and thus there sometimes be squiggles resolved by a build.

## Non-negotiables

- Simple, simple, simple for simple CLIs with linear scaling on complicating features. If cliffs exist, they are for very complex scenarios that do not follow the System.CommandLine interpretation of Posix
- .NETStandard support. Also, using code written in C# 7.1 must work.
- Focus on System.CommandLine doing what it does really well - parsing and strong typing results.

## TODO:

- Support CLI Name attribute
- Consider allowing Arg suffix to identify arguments
- Description from XML comments
- Handle case where there is no namespace

## Use of symbols in retrieving CLI values

If we support alternate providers for testing, configuration based defaults and other scenarios, it seems odd to force CLI specific 
name munging across the system. This would also result in changes in multiple places if an option was changed to an argument or similar.

As a result, the generated code tracks symbols. But, the other data providers won't use that. So, there are alternate keys which 
allow the main key to remain a string, but other keys to be added. This will be an implementation detail to most users.

## Note on the project layout

We're managing a generator and common files that need to be available in both `netstandard2.0` and `net7.0+` contexts. As a result there are a lot of projects at the moment.

## The static interface methods

To keep things simple for the common CLI case, we need a mechanism to call a static method based on a generic, especially for 
the `Create` static method. This must be static because it is a factory method.

We can create the method easily enough, but we need to _call_ it based solely on a generic argument. There are a few possible 
approaches:

- Use a static interface method, and a default interface implementation. This is appealing, but requires .NET 7. This is the approach
in the prototype
  - Upside: Simple, clean, efficient
  - Downside: .NET 7 only
- Use a static extension member against `IArgs` (without a static interface member) or similar 
  - Downside: I'm not clear which frameworks this works on
  - Downside: It will be messy because the method would have to accept `IArgs` as the member
  - My intuition says there is joy here, I just haven't spent much time on it

- Have a base class for `Args` that has a static method taking `IArgs` that throws, but avoids a 
compile error before generation runs, and then the derived class has a static method on `MyArgs`.
  - Downside: Does not work, because static methods can't be called via type arguments
  - Downside: We are stealing the user's inheritance chain

- Some sort of Builder class that the user specifies
  - Downside: Much more complex for the user, therefore, not pursuing

- Something I have not thought of
- Use reflection to find the method and call it. OK, we're not doing this

-- old notes

There are several scenarios where you may wish to use the classes in this assembly in other ways.

- You may wish to modify how data is collected - for example you could
use attributes that have different names. You can modify the data extraction 
without changes other changes.

- You may wish to modify the code that is output. For example, you may wish to add more attributes
to the generated output or change the accessibility of one or more members.You can modify the 
code that is created without other changes.

- You may wish to change both the way data is extracted and change how `ParseArgs` works.For example, 
you may want to extract information from methods, rather than properties. This would 
require changes to extraction and also a diferent method (rather than `ParseArgs` that using 
code would call (such as a new static extension method or deriving from `DragonFruit2.Cli`

- You may wish to add more information to the process. In this case, you will need to make 
changes to the data extraction process, the core information classes (`CommandInfo` and `PropertyInfo`), 
and the code that is emitted.

- You may wish to do something drastically different, such as using JSON to define your CLI and 
creating a dictionary of return values. That is cool, I'm not sure that this library will offer
benefit.

