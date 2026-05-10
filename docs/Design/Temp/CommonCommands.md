Common commands include 

- `--help` on all commands, and perhaps in the future on options and arguments
- `--version` and `--info` on all root commands
- `add`, `remove`, and `list` on collection focused commands

Common commands may or may not be option commands/options that execute.

These can be implemented via interfaces. This assumes that they can be handled by the parent command, which is appropriate in the currently known cases.

```c#
// T is the underlying type as seen by System.CommandLine
[CommonCommand<ItemCommandShape<T>>]
[Description("Add a new {this.CollectionName} to the collection of {this.CollectionPluralName}")]
public interface IIncludeAdd<T>
{
   Result ExecuteAdd(Result result);
   string CollectionName {get;}   Result ExecuteAdd(Result result);
   string CollectionPluralName {get;}}

[CommonCommand<EmptyCommand>]
public interface IIncludeHelp
{
   Result ExecuteHelp(Result result);
}

// similar for list, remove, version, info

[CommonCommandGroup]
public interface IIncludeCollectionSupport<T> : IIncludeAdd, IIncludeRemove, IInclude
{}
 
[CommonCommandClass] // might be able to reuse CommandClass, but these are a bit special
public class ItemCommandShape<T>
{
   public T Item {get; set;}
}

// List would have more things - providing common list behavior similar to dotnet new
```

Usage

```c#
[CommandClass]
public class Project : IIncludeCollectionSupport<File>  // actually I do not remember what type is used in .NET CLI
{
   ...
  
   public Result ExecuteAdd(Result result)
    {
       /// Do the thing
    }

    // Similar for Remove and List
}
```

Common common commands, where the behavior can be determined without intervention, can be handled in a command base class, although that results in both an attribute and a base class being required. Although perhaps only if the using programmer wants default help.

```csharp
public class CommandClass : IIncludeHelp, IIncludeVersion
{
    public Result ExecuteHelp(Result result)
    {
       /// Do the thing
    }

    // Similar for version
}
```

The description could be defined in the interface as an interpolated string because it is harvested during generation and included in the generated code.

Refinement is definitely needed. Key points are:

- Common commands logically belong to their parents
- Common commands cannot execute themselves, because they have no context
- Common commands should, by default, have common help with replaceable segments
- Common commands should have common shape, where the details of shape may be generic


## Implementation

This could be implemented with the `CommandClasses` being nested derived classes in the generated code for each child common command. This would remove naming collisions and allow the `Execute` of the child common command to naturally call the parent classes `ExecuteAdd`, etc method.

## Questions

- If we remove collisions involving common commands, are there any naming collisions in known CLIs, such as the .NET CLI?
- Are there other common commands in known CLIs?
- Is it desirable to have `list` extend it's behavior, similar to `dotnet new` across all DragonFruit2 usage? This could be controlled by multiple interfaces, but might lead to the shape being indicated by the parent.
