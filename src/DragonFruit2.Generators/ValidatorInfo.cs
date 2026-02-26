namespace DragonFruit2.Generators;

/// <summary>
/// Info for the generator about validators based on attributes, registered validators are created at runtime via the Register method
/// </summary>
public class ValidatorInfo
{
    /// <summary>
    /// Name of the attribute used
    /// </summary>
    public required string AttributeName { get; init; }

    /// <summary>
    /// Name of the type of the validator to use
    /// </summary>
    public required string ValidatorTypeName { get; init;  }

    /// <summary>
    /// The arguments entered for this validation attribute
    /// </summary>
    public required ValidatorArgumentInfo[] ValidatorArguments { get; init;}

}

/// <summary>
/// Info for the generator about validation arguments
/// </summary>
/// <remarks>
/// A runtime error will result if the Validator parameter type does not match the attribute type, per generator guidelines, this should be checked in an analyzer, not in the genertor.
/// </remarks>
public class ValidatorArgumentInfo
{ 
    /// <summary>
    /// Name of the argument
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The type of the paramter in the Validator class's ctor
    /// </summary>
    public required string ValidatorParameterTypeName { get; init; }

    /// <summary>
    /// The type of the argument in the attribute
    /// </summary>
    public required string AttributeArgumentTypeName { get; init; }

    /// <summary>
    /// The value of the argument, as a string. 
    /// This will appear in generated code, so should be parsable to the correct type. 
    /// Per generator guidelines, this should be checked in an analyzer, not in the genertor.
    /// </summary>
    public required string Value {  get; init; }
}
