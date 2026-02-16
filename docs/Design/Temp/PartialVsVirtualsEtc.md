# Partial vs virtual methods, interfaces vs base classes

This records some design thinking and lays it out for comment.

Extensibility is mechanisms to cross boundaries, and one of the most important aspects of those boundaries is the people, or roles, involved. It's important not to conflate the actions of these roles because that make extending difficult or over complicate the API when extensibility is not used.

In this case, at least the following roles may be involved (in addition to the end user, who we love but trust the implementing programmer to take care of):

- Implementing programmer: At least some of the time, just wants to get their CLI good enough to move on to the important parts of their app
- Extenders: Most interested in new data providers, defaulters, and validators
- DragonFruit2 and its maintainers: Who will be interested in both the pipeline and DrgaonFruit2 core elements like the `CliDataProvider`

There are three basic mechanisms available: partial methods and classes, virtual methods on base classes, and interfaces. 

## Current example

How to define `RegisterDefault `and `RegisterValidator` is currently the poster child for this general principle.

When the default of validator can be declarative (such as syntax or attributes), these methods calls are created on the user's behalf. The user only needs to call these methods when the values are not design time values (including `Today`) or there needs to be logic regarding registration (such as it being based on the end user). 

The Args cannot be instantiated until defaults have been applied and required values checked. This is to support the `required` keyword and work the implementing programmer may do in the constructor. We can't use static partial methods because we need to support the implementing programmer working in C# 7.3.

Thus, the discussion is whether the corresponding `DataDefintion` class is partial. 

## Partial classes and methods

The Args classes being partial is essential to some operations. We're trying to minimize this in design, but assuming validation remains a concern of the Args class, not the DataValues prior to Args class creation, we will have to have a partial class and it may perform additional functions.

### Side note

`DataValues` and `DataDefinitions`, which are intricately linked to the Args class are currently nested classes in this partial. This allows name isolation and allows access to private features of the Args class. However, it appears as part of the private API. The alternative is to solve the naming problem, declare that these classes should _not_ access private members of the Args class and make the scope `file`. Nested classes cannot have `file` scope. 

---

If `Register...` methods are partial methods, the implementing programmer has to have at least enough knowledge to type the word `partial`. This seems a relatively high bar.

The backing mechanism is that we generate a partial stub, and if the user provides an implementation, it is called.

## Virtual methods

If the Args class has a required eventual base class, such as it currently does with `ArgsRootBase`, virtual methods can be used. It is much more common for programmers to have encountered virtual classes and typing `override` to access available method names than `partial`. 

If we have a common base class, it seems logical to use it and the more common virtual methods.

## `ArgsRootBase<MyArgs>` vs interface

Interface implementations are not used in DragonFruit2 in order to support .NET Standard.

An interface cannot supply implementations for us. It is also very fragile, and this fragility leads to it being banned in .NET runtime API review, except for single action interfaces (such as we are using on `DataProviders`). We could provider `IProvidesDefaultRegistration` and 'IProvidesValidatorRegistration`, but that seems the least discoverable approach. The user could find this mechanism in our docs, and by see what is available in the DragonFruit1 namespace, possibly also navigating to a sub namespace. 

## Current implementation

Virtual methods are currently used, pending discussion on the common base class.



