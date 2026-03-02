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
    public required ArgumentInfo[] ValidatorArguments { get; init;}

}
