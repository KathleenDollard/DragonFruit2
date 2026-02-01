# Data definition

(The word "structure" used in the general sense in this document and does not refer to a `struct`)

The initial prototype generated System.CommandLine structures directly in the ArgsBuilder. This means the structure is based on the generation information. This is not scalable across providers.

The proposal is to create a DataDefinition containing all runtime information. For the convenience of having a single structure, it will probably also contain auxiliary information such as descriptions, but they will be lazily loaded and not use runtime allocations.

## General

All commands and members will include:

- `Obsolete` (shape TBD)
- `HiddenInCli`
- `Calculated` (or `NoGeneration`)
- `DataProviderInfo`: `Dictionary<>`: Data providers need to be able to cache data that does not apply to the specific run. We need further work to determine when an entry here is appropriate and when it should be held within the data provider. Note that data associated with the run, such as ParseResult, should be included in the Result, not the definition.

## Names structure

This naming structure is used for both commands and members.

These names allow consumers to guarantee that the name is structurally correct. This should also ensure that the names are unique is the container for simple cases, or give a clear diagnostic. This uniqueness guarantee may not extend to complex scenarios, such as complex types, and will not extend to scenarios where the container of the data provider differs from the Args containers.

_The logic used here is that programmers should not need to know naming rules. The name needs to be specified for a purpose; if it is just applying a format (such as kebab) to the name, there is nothing to save. Alternatively, data provider specific naming would have a higher level of coupling, but would allow more flexibility in naming for configuration files, etc.

These will generally be calculated from the class or member name, although attributes will be available to customize them:

- `Name`: The actual class name as it will be accessed during data loading and validation
- `FullName`: The full name, including namespace and containing class(es) for nested types
- `PosixName`: The name that should be used in CLIs
- `JsonName`: [The name that should be used for JSON](https://jsonapi.org/recommendations/)
- `XmlName`: [No initial "XML" in any casing and no initial number or underscore](https://learn.microsoft.com/en-us/dotnet/visual-basic/programming-guide/language-features/xml/names-of-declared-xml-elements-and-attributes), trouble finding a good standard for reference
- `EnvironmentVariables`: Upper case, snake delimited. This is the same name that is used for arguments.
- `ConfigName`: Lower case. Determine if kebab or snake delimited.

Because there are so many,a single attribute is suggested that will have named parameters for each style that can be specified.

## Command structure

- [General data](#general)
- [Names structure](#names-structure)
- `Options`: `IEnumerable<[Option](#option-structure)>`
- `Arguments`: `IEnumerable<[Argument](#argument-structure)>`
- `SubCommands`: `[Command](#command-structure)`
- `Branch`: Used to indicate that the command cannot be executed - inferred from `abstract`
- `IsOptionStyle`: Some options such as `--help` are actually commands. DragonFruit2 models these as options with funny names

## Member structure

All members will include the following:

- `DataType`:
- `Required`:
- `Default`: Assume the type used here can be either a value, a calculated value based on environment (such as Today), or a dependent default
- `Validation`: Assume the type used here can be either a self-contained validation or a dependent valdidation

## Option structure

- `Recursive`:
- `Arguments`:

## Argument structure

- `Position`: