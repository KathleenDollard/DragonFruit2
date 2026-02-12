# Design notes to confirm: Validation

[Validation documentation](docs/Valiation.md)

While simplicity is clearly a virtue, the traditionally ways to achieve simplicity in validation waste information. For example, generalized `Validation` methods that do not express the actions they perform or validators that supply only error text, not IDs or instructional text. Validation is key information to a CLI, and improve the effectiveness of the AI tools, we cannot afford to waste information. Therefore, this design has some tradeoffs between simplicity and specificity to maximize the available information. For example, there are many more validators because they are tuned to specific situations.

One of these problems is the limitations of attributes. For example, the `GreaterThanValidatorAttribute` must accept the compare with value as an object (boxing and also type failure potential), not only can it not be typed to the property type, it can't be typed to `IComparable`! The work to maintain strong typing throughout DragonFruit2 extends to validation, but it seems that to avoid boxing, we'd have to demand the implementing programmer restate the type as the attribute's generic type argument, which would also force us to double up the attributes to allow a non-generic version for C# 7.3 and lazy programmers.

Other forces also push away from the simplicity of current approaches like `System.ComponentModel.DataAnnotations`. One of the shortcomings of those systems, and all declarative (attribute based) validation systems is that any context driven validation is driven into other systems. For DragonFruit2, we want the implementing programmer to be able to ensure that behavior, including help, can be context driven. Current thinking on this is that it may be limited to configuration and environment, but I'm really interested in scenarios like [this one in the .NET CLI](https://github.com/dotnet/sdk/issues/52770). The key part of this is that file based apps work differently than MSBuild apps (@baronfel:

> "It's just annoying because the file-based app buildable thing has different semantics from the msbuild buildable thing - for file-based apps it must be the first non-binlog argument, but MSbuild will happily accept a sln/slnx/slnf/*proj at any position in the arguments array.").



## Issues to confirm

- [ ] Validate DataValues, not actual values
- [ ] Validation will be strongly typed to the type of the property - `TValue`
- [ ] Validation defined as much as possible with specifics. No Validate() method on class or DataValues, or one whose design discourages use.
- [ ] Dependent validation will be a first class citizen
- [ ] Calculated validation will be a first class citizen
- [ ] Diagnostics are held on the DataValue, unless we allow any command level validation
- [ ] Implementing programmers can define any validation programmatically (not just via declarative attributes) on the `CommandDefinition`
- [ ] Where possible, attributes will also be supported (where all info is available at compile/generation time) 
- [ ] Any declaring format will lower to a canonical form
- [ ] There will be a heavy preference to simplifying usage of existing validators over creating new validators
- [ ] We will heavily encourage implementing programmers with unique validation scenarios to create custom validators - therefore, we desire it to be as easy as possible
- [ ] Because of issues with validator parameter typing, we _must_ create analyzers for value typing and dependent property names (like xUnit)
- [ ] Because validators will often require syncing three classes, it would be highly desirable to have a fixer that will act as a template
- [ ] Because validator attributes will need to pass names correctly to avoid generation issues, it is either desirable or necessary to create an analyzer to check parameter types
- [ ] ?? Do we want to have a partial `CommandDefinition` class, or do we want to generate a `CommandDefinition` property for registrations. Currently, `CommandDefinition` is available via the DataValues property of the Result for use after parsing; do we want to give access for initialization?
- [ ] The names of the constructor arguments, or named properties much match exactly the names of the validator class's constructor parameters, and all the validator class's ctor parameters must be fulfilled
- [ ] There are problems with generation, so currently the validator (and by implication the attribute) can have only one constructor
- [ ] Should validator types use constructors or property assignment? It may be difficult to generate if we support both.
- 
- ## Current syntax

All methods are strongly typed to the property value (`TValue`) and inferred via the `MemberDefinition<TValue>` parameter.

Definitions can be via registration or attributes placed on the property.

### Location for dynamic declarations

Validators (and default values and required) can currently be defined in a CommandDefinition partial class:

```c#
public partial class MyArgs : ArgsRootBase<MyArgs>
{
    public int Age { get; init; } = 1;
   // Other properties 

   partial class MyArgsDataDefinition
   {
       public override void RegisterCustomizations()
       {
           Age.Default(22);
           Age.ValidateGreaterThan(0);
           Age.RegisterAsRequired();
       }
   }
```

Alternatively, we could generate a property on the `Args` class for the `CommandDefinition`. This could be private to avoid expanding the `Args` class API. For both approaches, we could also commit to nesting the `CommandDefinition`, allowing it to just be called `CommandDefinition`. This would look like:

```c#
public partial class MyArgs : ArgsRootBase<MyArgs>
{
    public int Age { get; init; } = 1;
    // Other properties 

    CommandDefinition.Age.Default(22);
    CommandDefinition.Age.ValidateGreaterThan(0);
    CommandDefinition.Age.RegisterAsRequired();
   }
```

### Forms

The current canonical form is:

```csharp
Age.RegisterValidator(new GreaterThanValidator<TValue>(memberDefinition.DefinitionName, compareWithValue));
```

The attribute form is:

```csharp
[GreaterThan(0)]
public int Age { get; init; } = 1;
```

The convenience form to improve readability and enhance validator discovery:

```csharp
Age.ValidateGreaterThan(0);
```
