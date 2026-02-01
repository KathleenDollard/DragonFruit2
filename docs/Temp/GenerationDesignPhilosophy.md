`Args` class is used as a simple reference

Three related questions regarding generation indicate we may have the wrong philosophy on generation. The current philosophy is that it should be totally flexible and normal code - for example, that the `Args` class have no requirement on base classes or attributes. There is also a parlor trick with overload resolution that requires a generated Cli.cs class in the namespace of the user's entry call (`ParseArgs`, etc.) which makes the `using DragonFruit2;` statement behave strangely and will be repeated in any namespace that has a DragonFruit2 entry call.

I believe the underlying philosophy is valid - as much as possible it should be normal C#. However, the current design focuses on ordinary C# _code_, rather than ordinary C# _behavior_.

The three questions are: 

- Should the `Args` class have a base class and/or a marker attribute? (#24)
- Should we put more effort in solving the gnarly problem of finding the `ArgsBuilder`?
- Should we prioritize generation performance in order to provide good design time behavior?

## Impacts on current behavior

- The user retrieves an obtuse error about the type argument not being usable

-  `Args` class in the call to `ParseArgs`, `TryParse` or `TryExecute`
  - Until generation runs, the user has a squiggle about the `Args` class not having an `IArgs` interface, which we do not want the user to implement
  - Current behavior might be mitigated by a analyzer error that would appear along with this obtuse error
- Behavior of the DragonFruit2 using statement in the user's `Program.cs` code, and 
- Generation performance including whether the generator can run in design mode of only on build.

The `Args` class defines the data that will be retrieved by DragonFruit2 and also holds the resulting data in a strongly typed format for the user. _Should we insist that this class be an 'ordinary' class, or should we require it to have a base class, and or attribute.

The class that holds the root argument is special because it's used to link the users application code to the data retrieved by DragonFruit, via the type parameter of `ParseArgs`, `TryParse` or `TryExecute`. It also accesses subcommands. The runtime relationships will remain unchanged by decisions related to this issue. 

Generation is impacted by how we identify these classes. For them to be 'ordinary' classes, with no requirements other than being a `class` (current restriction) requires generation to:

- Identify the root from the `ParseArgs`, etc. type argument
- Use the semantic model to find this type
- Remove duplicates based on multiple calls
- Retrieve derived classes recursively to find all descendant commands
- Miss the most efficient generation (attribute based)

Running in this manner, the DragonFruit2 generator cannot run in the design time environment. The impact is that the user will 