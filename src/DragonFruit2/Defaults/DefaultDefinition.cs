using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public abstract class DefaultDefinition
{ }

public abstract class DefaultDefinition<TValue> : DefaultDefinition
{
    private readonly string[] _dependentValueNames;

    public DefaultDefinition(string valueName, string description, params string[] dependentValueNames)
    {
        ValueName = valueName;
        Description = description;
        _dependentValueNames = dependentValueNames;
    }

    public abstract bool TrySetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value);

    public string ValueName { get; }
    public string Description { get; }
}


