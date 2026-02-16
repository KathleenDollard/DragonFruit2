# Startup problem

There is a pernicious problem getting a toehold between the generalized code and the user's specific code. This could be solved a number of ways, but these goals restrict the options:

* Work in .NET Standard/Framework and almost certainly with user code in C# 7.1
* Not add extra work for the user
* Not requiring the user to write code that is not in the IntelliSense dropdown (don't rely on generation for basic semantics)
* Not creating an instance of the `Args` class because this could restrict important instantiation rules - such as disallowing immutability

The design in use:

A generated `Cli` type in `Cli.cs` is taken preferrentially to the one in the DragonFruit2 library, because it is in the same assembly. This type finds the `DataDefinition` via its name.

Some things I have tried or considered:

* An `IArgs<TRootArgs>` interface with a static method to get the data (not valid in .NET Framework)
* Module Initializer to create a cache; the cache exists in the current design (not valid in .NET Framework)
* Using overload resolution for methods and extension methods to have a non-specific `ParseArgs` method for editing and a generated one
  * This would require changing to an instance method for .NET Framework. 

Other ideas

* Creating an extension method in DragonFruit2 which is 