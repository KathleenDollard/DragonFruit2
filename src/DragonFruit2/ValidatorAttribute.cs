using DragonFruit2.Validators;

namespace DragonFruit2;

/// <summary>
/// Base class for DragonFruit2 validation attributes. This is used only during compile time generation.
/// </summary>
/// <remarks>
/// The type and arguments are used only during generation to create a call from user code
/// to an extension method or validator constructor. The names and types must align. Since
/// we must collect the values as objects, this creates a potential problem with the generator
/// creating code that has compiler erorrs. Since the user can't fix them directly, we will
/// instead fail to register the validator.
/// <br/>
/// Note that this object based argument ctor is protected. Typing and naming problems will
/// sometimes need to be fixed in the attribute's ctor base ctor call. At other times, the argument
/// will need to be typed to TValue of the validator, and perhaps other scnearios, where the 
/// attribute cannot force type correctness.
/// <br/>
/// Here we will take the C# design approach "if you can do it in an analyzer, do it in an analyzer".
/// With any design we come up with that users strong typing in validators and relies on a generator,
/// we will have some variation of this problem. 
/// <br/>
/// Thus, we absolutely need analzyers for validator attributes. Hopefully we can make them not too
/// painful to write. XUnit can be our pattern here. The analyzer for the attribute usage needs to check:
/// <br/>
/// - The class using the attribute is an Args class
/// - The names of any dependent properties are in the current class
/// - The actual type (such as IComparable&lt;TValue>) of any values are correct
/// <br/>
/// Optionally, we may create an analyzer to help validator maintainers
/// 
/// <br/>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class MemberValidatorAttribute : Attribute
{
    protected MemberValidatorAttribute(Type validatorType, params object?[] arguments)
    {
        ValidatorType = validatorType;
        Arguments = arguments;
    }

    // TODO: These properties are not used by DragonFruit2. Do we remove them, or leave them if the user looks at the attributes with reflection.
    public Type ValidatorType { get; }
    public object?[] Arguments { get; }

}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class MemberValidatorAttribute2<TValidator> : Attribute
{
    protected MemberValidatorAttribute2(Type validatorType, params object?[] arguments)
    {
        ValidatorType = validatorType;
        Arguments = arguments;
    }

    // TODO: These properties are not used by DragonFruit2. Do we remove them, or leave them if the user looks at the attributes with reflection.
    public Type ValidatorType { get; }
    public object?[] Arguments { get; }

}