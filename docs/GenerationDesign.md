# Generation design for DragonFruit2

DragonFruit is a moderately complex generator, and thus a potentially difficult one to understand. Aspects of the solution are designed to mitigate this.

## Key projects in the solution

- `DragonFruit2.Generation.Test.Projects`: Several projects used for testing.
- `DragonFruit2`: Library core for DragonFruit2.
- `DragonFruit2.Generators`: Generators for DragonFruit2.
- `DragonFruit2.Generators.Test`: Generators for DragonFruit2.
- `DragonFruit2.Polyfills`: Tests for DragonFruit2 generators, also used for debugging generator.
- `PrototypeConsoleApp`: _Non-generated_ app used to brainstorm the code that will be generated. Needs to say in sync with `SampleConsoleApp`.
- `SampleConsoleApp`: Generated application used to ensure that the generated code compiles and runs correctly for single layer CLIs.
- `SubCommandConsoleApp`: Generated application used to ensure that the generated code compiles and runs correctly for CLIs with subcommands.

When making changes, to generated code, get it compiling and running correctly in `PrototypeConsoleApp`, and only then alter the generator tro match.

## Generator steps

The steps defined in `DragonFruit2Generator.Initialize` are as follows, with the tracking name in parentheses:

- (Extract) Predicate: `InitialFilter` looks for `ParseArgs` (and needs to look for other variations)
- (Extract) Transform: `Transform` creates root `CommandInfo`, each containing a tree of its child subcommand `CommandInfo`s.
- (BindParents) `BindParents`: As created, parents know about children, this connects children to parents.
- (FlattenHierarchy) `SelectMany`: Creates a flattened set of all `CommandInfos`, roots and subcommands as a `...ValuesProvider` This is used to output a file for each `CommandInfo`s.
- `Collect`: Creates a `...ValueProvider` that contains a single list with all `CommandInfos`. This is used to output the local `Cli` class.
- Output sources

The predicate needs to be updated to include all `DragonFruit2` methods.

The `Transform` needs to do a `Distinct`. This might be by returning the syntax/semantic model, doing a distinct on the root args type name, and then 

## Dependencies

The packable project is `DragonFruit2`. `DragonFruit2.Generators` is a dependency _of_ `DragonFruit` and thus the generator itself can not use anything in `DragonFruit2`. A separate common library may be required later.

