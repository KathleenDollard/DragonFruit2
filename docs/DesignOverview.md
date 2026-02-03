# Design overview

Notes:

- DragonFruit2 relies on Roslyn Code generation and the args class must be a partial class. It does _not_ run during design time generation.

## High level design when using modern .NET

Elements

- `main` or elsewhere, the implementing programmer makes a call to a DragonFruit2 entry point. See the repo's repository `ReadMe.md
- `Args` class (with any name):
  - Contains properties that correspond to data that will gathered - corresponding to options and arguments in the CLI
  - If the base class is `ArgsRootBase<Args>` (self reflected), it is a root can can be used in a DragonFruit2 entry point
  - Otherwise, the base class should be a `Args` class that will serve as it's parents in the hierarchy. IOW, derived classes represent subcommands in the CLI
  - The `Args` classes contains hints and direct calls that customize the CLI, validation, defaults, etc.
- `Result<Args>`: Returned from the DragonFruit2 entry point and contains:
  - An instance of an `Args` class
    - In single level CLI's, this is always an instance of the root `Args` class
    - If there are subcommands, this is an instance of the `Args` corresponding to the active command, which is derived from the root `Args` class
  - A collection of diagnostics (errors and warnings)
  - As a convenience, a bool `IsValid` property
  - Debugging information, depending on settings. This will include the `DataValues` and potentially the System.CommandLine `ParseResult`
- Args`DataValues`: An interim representation of each `Args` class that is used during data gathering, and will be available as part of the debugging information in the result, which contains
  - A data value for each property, which itself contains the value, what data provider set it, and diagnostics
  - A method to create the hydrated `Args` instance

  - Data providers are specific to the mechanism - System.CommandLine, configuration file, defaults, etc.
- Args`DataDefinition`: A definition built from code hints (types, keywords and attributes) and `Register` methods which contains all information available to initialize `DataProvider` instances
- `DataProvider`: a general purpose provider which forms a pipeline to retrieve data where the first provider fulfilling the value is used, which may be of one or more subtypes:
  - `IDataProvider`: Can provide data for one or more properties of `Args` classes
  - `IActiveArgsProvider`: Can provide the current active `Args` when subcommands are present
  - No interface provided: The data provider only offers diagnostic information
- `Builder`: The orchestration engine, specific to the root `Args`
- `ArgsBuilder`: Initially used but conflated other concepts that are gradually being moved to other entities. Will be removed



