# Design overview

Notes:

- DragonFruit2 relies on Roslyn Code generation and the args class must be a partial class. It does _not_ run during design time generation.

## High level design when using modern .NET

### The args class (move this to usage docs)

The user creates an args root _class_ that contains properties for each value that is needed by the application. This should include all values that correspond to options or arguments on the generated command.

Properties that should not appear on the command, but may be supplied by another provider, such as configuration, can be marked with the `ExcludeFromCli` attribute. This feature is expected to be used only in niche cases, such as a complex value that appears as several options and arguments, but is entered as a unit via configuration or another provider.

Additional properties can be included if marked with the `DoNotGenerate` attribute. These can be calculated properties or they can be set in a transform method.

The args class must be a partial class.

#### Execute method

Optional. Runs the behavior of the application if the `Cli.Invoke()` method is called instead of `Cli.ParseArgs()` or it can be called directly on the args object returned by `Cli.ParseArgs()`.

#### Validate method

The Validate method is intended for niche scenarios and quick and dirty apps because it makes no contribution to help. Built in validators should generally be used where applicable for consistent behavior. When custom or cross-property validation is needed, custom validators will provide the best end user experience.

The `Validate` method of the args object can be used if something makes a custom validator difficult to use. It can also be used in quick and dirty applications intended for self or team use where features like help support, error IDs are not considered important.

#### Parameterless constructors

[ I do not think this naive approach will work because I methods other than constructors can't call init scope setters. The tweaks described here are important, thus, consider whether custom setters could be used for all these tweaks. ]

The transform method is called on the args object immediately after instantiation. It allows simple tweaks to property values, such as trimming, adjusting case (upper, lower, sentence, etc.), and applying a ceiling/floor. To support immutability, this must happen during construction, so is called from the constructor.

[TODO: Confirm that methods called from ]

#### CtorFinalize method

This method is likely to be renamed.

A final step before returning the object. This is expected to be used for creating complex objects that need a setter for construction and thus the transform needs to be done during construction.

### Operation design

In addition to creating the args class (`MyArgs` in this section), the developer calls the static `TryParse` method of the `Cli` class, passing the root args class as the type parameter.

Generation uses the call to `TryParse` as an indicator to generate additional code in an `MyArgs` partial class. This adds the `IArgs` interface and implements it's two methods. `Initialize(Builder builder)` and `Create()`. At runtime, this method creates a fully hydrated and validated instance of hte `MyArgs` class.









`ParseArgs` is specific to System.CommandLine. Alternative generation can support other input mechanisms, although the basic design is for all the data to be collected and validated, making some possible entry mechanisms more difficult (WinForms or MAUI).

All DragonFruit2 args classes implement a `Create` static factory method that is accessed either via a static interface method, or a 

[[ Work on this design, as it currently requires a base class, block struct. Instead create a ArgsBuilder.cs instance and move the Args.cs and CliArgs.cs code into a ArgsBuilder.cs class]]

The `Create` method is fairly simple, it creates
just creating a DataValue for each property using the static `GetDataValue` method of the `Args` class. This can be called directly if a derived class, 

