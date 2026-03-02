namespace DragonFruit2.Generators;

/// <summary>
/// Info for the generator about validation arguments
/// </summary>
/// <remarks>
/// A runtime error will result if the Validator parameter type does not match the attribute type, per generator guidelines, this should be checked in an analyzer, not in the genertor.
/// </remarks>
public class ArgumentInfo
{ 
    /// <summary>
    /// Name of the argument
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The type of the paramter in the Validator class's ctor
    /// </summary>
    public required string ParameterTypeName { get; init; }

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
