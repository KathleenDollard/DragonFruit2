namespace DragonFruit2;

/// <summary>
/// Represents the definition of a data member corresponding to a property in an ArgsClass.
/// </summary>
/// <remarks>
/// This class may be frequently used by data providers that have no interest in the CLI shape.
/// </remarks>
public class MemberDataDefinition : DataDefinition
{
    public MemberDataDefinition(Type argsType, string name) 
        : base( name)
    {
    }

    public required Type DataType { get; set;  }
    public bool IsRequired { get; set; }

}