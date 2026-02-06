using DragonFruit2.Defaults;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace DragonFruit2;

/// <summary>
/// Represents the definition of a data member corresponding to a property in an ArgsClass.
/// </summary>
/// <remarks>
/// This class may be frequently used by data providers that have no interest in the CLI shape.
/// </remarks>
public class MemberDataDefinition : DataDefinition
{

    public MemberDataDefinition(CommandDataDefinition commandDefinition, string name, bool isOption)
        : base(name)
    {
        CommandDefinition = commandDefinition;
        IsOption = isOption;
    }

    public CommandDataDefinition CommandDefinition { get; }
    public required Type DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsOption { get; }
}

public class MemberDataDefinition<TValue> : MemberDataDefinition
{
    private readonly List<DefaultDefinition<TValue>> _defaultDefinitions = [];

    public MemberDataDefinition(CommandDataDefinition commandDefinition, string name, bool isOption)
        : base(commandDefinition, name, isOption)
    { }

    public IEnumerable<DefaultDefinition<TValue>> Defaults => _defaultDefinitions;

    public void RegisterDefault(DefaultDefinition<TValue> defaultDefinition)
    {
        _defaultDefinitions.Add(defaultDefinition);
    }
    public void RegisterDefault(TValue value)
    {
        _defaultDefinitions.Add(DefaultConstant<TValue>.Create(value));
    }

    public bool TryGetDefault(DataValues dataValues, [NotNullWhen(true)] out TValue value)
    {
        foreach (var defaultDefinition in Defaults)
        {
            if (defaultDefinition.TryGetDefaultValue(dataValues, out value))
            { return true; }
        }
        value = default!;
        return false;
    }

}