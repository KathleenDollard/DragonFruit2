# Ordered defaults

This is based on issue #66. As the work proceeded it became clear that ordered and normal (non-ordered) defaults had enough differences that unifying them at this point would result in a worse experience, and that while we could imagine scenarios for ordered defaults, without a working use case, we were unlikely to build it correctly, and built less than correctly would result in future breaking change issues. Thus, ordered defaults are set aside for the future. This proposal captures the thinking we have now on the problem.

This complexity of ordered defaults needs to be pay for play:

- Scenarios with no validations should have minimal or no impact, including performance
- Non-ordered defaults are the common case and the existence of ordered defaults should have minimal or no impact, including performance

The goal of this issue is to ensure that scenarios with reasonable complexity can be handled. While the scenarios described here are uncommon, without special handling they would need to be done as code in a method, losing the metadata about the defaults. I also hate to think of the code that would be written if there is not an ordered default system.

Non-ordered defaults are applied `MemberDataDefinitions` and ordered defaults are applied to `CommandDataDefinitions` so that ordering can occur between different data values.

## Scenario for ordered defaults

Give a CreateOrder Args class, with these properties:

- CreateOrder
  - Customer
  - OrderDate
  - PrepStart
  - PrepDays
  - ShipTarget

The following seems a very reasonable set of defaults to minimize what the user needs to enter, where "yes" and "no" indicate whether the value was entered:

| OrderDate | PrepStart | PrepDays | ShipTarget | Defaults                          |
|-----------|-----------|----------|------------|-----------------------------------|
| no        | no        | no       | no         | OrderDate = Today                 |
| yes       | no        | --       | no         | PrepStart = OrderDate (*)            |
| yes       | yes       | no       | yes        | PrepDays = ShipTarget - PrepStart |
| yes       | ---       | no       | --         | PrepDays = (from Config **)          |
| yes       | yes       | yes      | no         | ShipTarget = PrepStart + PrepDays |
| yes       | no        | yes      | yes        | PrepStart = ShipTarget - PrepDays |

* PrepStart can only be calculated if both PrepStart and ShipTarget are entered

** The configured PrepDays maybe be based on the customer

The order in this table sets precedence, and so is very order dependent. Thus it should be expressed in code as some sort of order based naming like an `Add` or `Register` method.

The current default design accommodates everything except the ordering position of getting PrepDays from configuration.

Currently, for each `DataProvider` all values are collected. This is entirely reasonable when there are no dependencies between data values. However, defaults are dependent on other data values, by design.

## Moving toward a solution

It seems reasonable to start with the notion of _user entered_ which will be referred to as _entered_. With this perspective, there are defaults that are clearly not "entered" and there are values that are clearly "entered". 

There is a subtle assumption in the preceding paragraph and the table above that "entered" has a Boolean meaning. This _feels_ true.

Is it really a Boolean? A value parsed from the command line can be assumed to be user entered. And a constant default specified in the program can be assumed to be a default value. Where does configuration fall? And (omg) is it per CLI or per data value? And is it changeable, such as between runs? Is it really "user entered" if it comes from a script?

Tentatively, user entered value are all of those present when the ordered default provider is called, and thus is Boolean.

### Let's make the example above more complex

| OrderDate | PrepStart | PrepDays | ShipTarget | Defaults                          |
|-----------|-----------|----------|------------|-----------------------------------|
| no        | no        | no       | no         | OrderDate = Today                 |
| yes       | no        | --       | no         | PrepStart = OrderDate             |
| yes       | ---       | no       | --         | PrepDays = (from SLA config) |
| yes       | yes       | no       | yes        | PrepDays = ShipTarget - PrepStart |
| yes       | ---       | no       | --         | PrepDays = (from other config)    |
| yes       | yes       | yes      | no         | ShipTarget = PrepStart + PrepDays |
| yes       | no        | yes      | yes        | PrepStart = ShipTarget - PrepDays |

Before dismissing this as unrealistic, consider that some configuration information should carry more weight than others, with the hint at a config file that holds SLAs as just the example it took 5 seconds to think of.

Infinite flexibility is infinite complexity (maybe), so we need guidelines.

### Is the `IActiveArgsProvider` special

One and only one `DataProvider` will supply the active `CommandDef` (sorry for the temporarily inconsistent namings). Is this provider special?

It is special in that it provided instructions on what is being built. It is easy to assume this was "entered". If it was "entered" it seems reasonable to declare that anything from that `DataProvider` is entered. If that presents a problem in some scenario, the `DataProvider` could be split and any portions that were not "entered" could be in a different `DataProvider`.

_Note: I have the natural issue that I am biased to scenarios that I can imagine, and my imagination is not the breadth of the problem space, realistic scenarios are very valuable._

A simplifying model would be to declare the `DataProvider` that provided the active `CommandDef` to be special, and to be the only thing that was "entered".

If we make this assumption, we have "entered" from that one `DataProvider` and everything else is a default.

Defaults need to be ordered. That is a necessity anytime there are multiple providers or dependent defaults, and we have both.

Since properties are not ordered, attributes are not ordered. As a result, attributes can only be used where there are symmetric bilateral defaults (ShipTarget and PrepStart comply in our example).

### Single pass or multi-pass

The sample above could be directly pulled from the table, resulting in seven defaults with `n` number of dependencies. A possibly more flexible approach is to define the same thing as a multi-pass operation. This has da certain simplicity and was the original design. It is only order dependent where the same missing value has multiple entries.

From an implementation point of view, the code only evaluates missing values, then checks if the dependencies are available, and then does the operation and assignment.

Consider these dependencies:

| Missing value | Dependencies          | If met                 |
|---------------|-----------------------|------------------------|
| ShipTarget    | PrepStart, PrepDays   | PrepStart + PrepDays   |
| PrepStart     | ShipTarget , PrepDays | ShipTarget - PrepDays  |
| PrepDays      | (SLA config)          | (SLA value)            |
| PrepDays      | ShipTarget, PrepStart | ShipTarget - PrepStart |
| PrepDays      | (other configs)       | (config value)         |
| PrepStart     | OrderDate             | OrderDate              |
| OrderDate     | None                  | Today                  |

#### Simplistic approach

If PrepDays and PrepStart (only) are missing:

- Any values that are present are simply used.
- First pass: PrepDays set if in any config file. PrepStart is set to the OrderDate.

There is a good chance this is not what the programmer intended, and with the simplistic approach there is no way for the implementing programmer to indicate that the `ShipTarget - PrepDays` should be used if `PrepDays` is found.

#### Restart approach

Thi problem can be overcome with an approach that restarts from the top when a value is changed.

If PrepDays and PrepStart (only) are missing:

- Any values that are present are simply used.
- First pass: PrepDays set if in any config file. Evaluation restarts at the top.
- Second pass: PrepStart is set to `ShipTarget - PrepDays`

This sounds like it would be inefficient, but it could be implemented as something like a list where items were removed or marked for rapid skipping when they were fulfilled.

### A syntax for ordering

Time to jump in and see how we might express this complex scenario.  This is a rn ordered registration approach with extension methods that wrap some version of `RegisterDefault(new ThisKindOfDefault)` where `ThisKindOfDefault` might be constant, offset, configuration, etc.:

```csharp
partial class MyArgsDataDefinition
{
    public override void RegisterCustomizations()
    {
        ShipTarget.Default(otherValue: PrepStart, offset: PrepDays);
        PrepStart.Default(otherValue: ShipTarget, offset: - PrepDays); // Concern about this being an expression
        PrepDays.DefaultFromConfig( "SlaFile");                        // Assume name.ToJson(), with alternative of providing
        PrepDays.Default(otherValue: ShipTarget, offset: - PrepStart);
        PrepDays.DefaultConfig();           // All DataProviders, except the one identified as "entered" are queried in CLI precedence 
        PrepStart.Default(OrderDate);
        OrderDate.Default(DateTime.Today);
    }
}
```

### All defaults can be compound

Depending on the type, there may be a need to offset either by another value. This is true of numerics and, especially, dates. At present, there aren't any other compound operations I can think of. 

## Summary

The proposed solution is:

- Insert an `OrderedDefaultDataProvider` after all other providers _except_ the non-ordered `DefaultDataProvider` across the entire `CommandDefinition`
- Use the ordered default system to progress through all defaults in a multi-pass, restart on changes, manner
  - Ordering of defaults will be via when they appear in the `RegisterCustomization` method
- Call the non-ordered `DefaultDataProvider` as the final data provider

This allows the `OrderedDefaultDataProvider` and `DefaultDataProvider` to be used on the same `CommandDefinition`. This seems valuable so that adding ordering to a single data value does not force changes across all data values.

The `OrderedDefaultDataProvider` will do its work once. It should make no additional changes if it tried to do it's work multiple times, but a caching check and a Boolean indicating whether the work has been done for this run is probably appropriate. This appears to be doable since `DataValues` are passed to `DataProvider.TryGetValue`, but it certainly breaks the normal pattern. If this presents difficulties, an option would be to have special behavior in `Builder`, however this would be unfortunate because if there is any piece of this puzzle that is likely to be changed, it's this complex ordered default system.
  
Defining of defaults will be by registration or attributes

- The registration approach will be needed for complex ordering
- Attributes based defaults can be independent of other data values, or they can be symmetric pairs where ordering does not matter
- Attribute based defaults will appear first in ordering because within that group order should not matter and those values maybe used elsewhere
- Attributes appearing first in the order will not always be correct, but when it is not registration will provide a better record, so the programmer will have to switch
- Only one default attribute will be allowed per property, because ordering by file position is not a C# thing