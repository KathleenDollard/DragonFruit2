# Finding the ArgsBuilder requires generated `Cli.cs`

Each ArgsClass (RootArgsClass and all subcommands), has a corresponding `ArgsBuilder` class which performs args specific tasks, such as creating and filling the ArgsClass instance. It has proven remarkably difficult to find this RootArgsClass's ArgsBuilder based on the type parameter.

TBH, I spent an incredible amount of time trying to find a more elegant solution before going with the flawed one that at least works. I need either pairing or someone else to take a fresh look at this problem.

## Considerations and restrictions

- We have the RootArgsClass only via a type parameter
- We cannot use static members on interfaces as that is not available in .NET Standard
- We cannot create an instance of the RootArgsClass, so no virtual members are possible
  - This is because the RootArgsClass may have required members that we do not yet have
- Static constructors are not guaranteed to run early, thus ArgsBuilders can't add themselves to a dictionary or similar via static constructors

## Current approach and drawbacks

The current approach relies on a parlor trick with overload resolution. Resolution proceeds in phases, the current namespace is in a pass before all other namespaces.

There is a `DragonFruit2.Cli` class containing stub implementations of entry points (`ParseArgs`, `TryParseArgs`, and `TryExecute`). A version of this is generated into the namespace of the entry point call which contains the actual implementation. The only thing important in this generated code is finding the ArgsBuilder that corresponds to the RootArgsClass. If we solved that, we could always use the `DragonFruit2.Cli` type's code.

While the current version works, it has drawbacks:

- Initially, `using DragonFruit2;` is required for IntelliSense to display the entry points. However, after build it is unused, which will make little sense to the using developer
  - If the user cleans this up, `Cli` will be squiggled until the next build
  - This is an annoyance we could perhaps live with
- The `Cli` class appears in the user's namespace and they probably do not understand why it is there
  - It cannot be placed into another namespace or the parlor trick will not work
- If there are multiple calls to DragonFruit2 entry points in different namespaces, there will be redundant generated `Cli.cs` files
  - This will occur whether these are multiple calls to the same RootArgsClass, or different RootArgsClasses
- The generated `Cli.cs` files that are in namespaces other than the one containing the entry point call will cause an ambiguity compiler error
  - We cannot live with this long term

## Possible approaches to explore

### Dictionary of ArgsBuilders, or similar

(For each RootArgsClass, all of its subcommands ArgsBuilders will be touched, so laziness is only helpful when there are multiple RootArgsClasses, which may not be a scenario we need to optimize for.)

We could create a dictionary of ArgsBuilders during generation. However, I was unable to figure out a way to trigger creating the dictionary.

### Use the parlor trick on extensions, rather than static class

This approach appears to still be vulnerable to ambiguity, and I'm not yet seeing clever use of static extensions (and they are ugly in .NET Standard).

### Use an instance of the Cli class, rather than static methods

This would reduce the beauty of the entry point and I didn't get my head around how it triggers dictionary creation into an accessible mechanism (without the parlor trick) anyway.

Instead of 

```csharp
var  result = Cli.ParseArgs<Args>();
```

It would be

```csharp
var  result = new Cli().ParseArgs<Args>();
```

### .NET Standard has different generation and restrictions

If we restricted .NET Standard to all DragonFruit2 entry points being in a single namespace, or restricted .NET Standard to a single entry point, we could simply use different generation for it.

For .NET Modern we could use static members in interfaces - which I think is now available in all supported .NET Core versions.

### Something else

As I said, I looked at this problem so long I may be missing an obvious solution.
